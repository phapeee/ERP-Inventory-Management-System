System Architecture Overview
============================

Purpose
-------

This document provides a high‑level system architecture overview for
**PineCone Pro Supplies’** next‑generation Enterprise Resource Planning
(ERP) and Inventory Management System (IMS). The goal is to replace
legacy spreadsheets and siloed systems with a unified, modular platform
that scales with the business. The overview summarizes the architecture
principles, major components, deployment model and operational
considerations.

## 1. Architecture vision & principles

**Event-Driven Modular Monolith** – The ERP/IMS is designed as a modular monolith. The application is a single deployable unit, but it is internally structured into well-defined modules corresponding to business domains (e.g., Product, Inventory, Order). To ensure maximum decoupling, modules communicate with each other exclusively through an **in-process event bus** using a publish-subscribe pattern. Direct function calls between modules are disallowed.

**Cloud‑native & containerized** – The application is containerized using a multi-stage Docker build. The base image comes from the official .NET SDK and ASP.NET runtime for the backend and Node/Nginx for the Angular front‑end. The container image is stored in a private Azure Container Registry and deployed on **Azure App Service** for simplicity and scalability.

**12‑Factor & DevOps** – Follow 12‑factor application principles:
configuration via environment variables, stateless application, automated
build/test/release pipelines and comprehensive telemetry. Adopt
continuous integration and continuous deployment (CI/CD) using Azure
Pipelines or GitHub Actions.

**Security by design** – Enforce role‑based access control (RBAC),
least‑privilege service accounts and network segmentation. Use
**Microsoft Entra ID** for user and workload identities and **Azure
Key Vault** for secrets management. All traffic is encrypted.

**Scalability & resilience** – Design for high availability and load:
run multiple instances of the application, use auto‑scaling, and implement circuit
breakers/retries for external integrations. Azure App Service provides built-in support for scaling and load balancing.

## 2. High‑level architecture

The system consists of a web‑based front‑end and a back‑end application deployed as a single unit. Users access the system via
browsers, POS devices or mobile apps. Internal staff (customer service,
warehouse associates and purchasing) and external actors (customers,
vendors, carriers and tax services) interact with the platform.

An **Azure Application Gateway** acts as the API gateway. Requests
from clients first hit the gateway, which terminates TLS, authenticates using
Entra ID, performs rate limiting and routes requests based on the URL
path to the application.

The application runs on **Azure App Service**, which can be scaled out to multiple instances. The application uses a single **Azure SQL Database** with separate schemas for each module. Event‑driven workflows use an in-process event bus or **Azure Service Bus** for decoupling.

### Domain modules

- **Product Information Module (PIM)** – maintains a single source of
    truth for SKUs, attributes, units of measure, kits/bundles and
    hazardous classifications; exposes APIs to other modules and
    the front‑end.
- **Inventory & Warehouse Module** – tracks stock across multiple
    warehouses and bins, updates available‑to‑promise, performs cycle
    counts, manages RF scanning and handles lot/serial tracking.
- **Order Management Module** – accepts orders from the web store,
    B2B portal, point‑of‑sale and Amazon, performs payment and fraud
    checks and orchestrates fulfilment via shipping and warehouse
    modules.
- **Purchasing & Vendor Module** – manages suppliers, vendor
    scorecards, purchase order (PO) workflows and automatically
    generates POs using reorder points and economic order quantity.
- **Shipping & Logistics Module** – integrates with carriers for rate
    shopping, hazmat documentation and cross‑dock/3PL workflows;
    coordinates pick‑pack‑ship tasks and produces shipping labels.
- **Tax & Accounting Module** – calculates multi‑state sales tax,
    records journal entries and synchronizes with the general ledger;
    provides audit trails and ensures compliance.
- **Returns & RMA Module** – standardizes return authorization and
    disposition codes, manages refunds or exchanges and updates
    inventory accordingly.
- **Analytics & Alerting Module** – aggregates operational data into
    a reporting data store, delivers real‑time dashboards and sends KPI
    alerts to stakeholders.

The application uses a single database to ensure data consistency. Caching relies on **Azure
Cache for Redis** for read‑heavy operations; files and attachments
reside in **Azure Blob Storage**.

## 3. Technology stack

| Layer                              | Technology                                                 | Details                                                                                                                                                                                        |
| ---------------------------------- | ---------------------------------------------------------- | ---------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| **Front-end**                      | Angular 14+ SPA                                            | Uses TypeScript, RxJS and Angular Material. Consumes REST APIs via the Angular HttpClient module and reads the API base URL from a JSON configuration file that is injected at runtime.        |
| **Back-end**                       | ASP.NET Core 6 Modular Monolith                            | Built with Clean Architecture and Domain-Driven Design. Communicates internally via an in-process event bus. Exposes public RESTful APIs.                                                      |
| **Database**                       | Azure SQL Database; Azure Cosmos DB; Azure Cache for Redis | A single Azure SQL database with separate schemas per module. Cosmos DB can be used for high-volume or semi-structured data. Redis provides caching; Azure Blob Storage stores binary objects. |
| **Messaging**                      | In-process event bus or Azure Service Bus                  | An in-process event bus handles internal, asynchronous communication between modules. Azure Service Bus is used for external integrations.                                                     |
| **Authentication & Authorization** | Microsoft Entra ID                                         | Provides single sign-on for users and managed identities for services. JWT bearer tokens secure API calls, and RBAC is enforced by the API gateway and the application.                        |
| **Containerization**               | Docker multi-stage builds                                  | Use .NET SDK and runtime images to restore and publish the backend API and Node/Nginx images to build and serve the Angular app. Images are stored in Azure Container Registry.                |
| **Deployment**                     | Azure App Service                                          | Host the application. App Service supports auto-scaling, deployment slots, and integration with Azure DevOps.                                                                                  |
| **CI/CD**                          | Azure Pipelines or GitHub Actions                          | Build, test and push container images to ACR. Deploy the application to Azure App Service.                                                                                                     |
| **Monitoring & Logging**           | Azure Monitor & Application Insights                       | Collect metrics, logs and traces. Provide dashboards and alerts for performance and business KPIs.                                                                                             |

## 4. Deployment & infrastructure

- **Container registry** – A private **Azure Container Registry**
    hosts the Docker image for the application.
    Azure App Service authenticates to ACR via managed identity and pulls the image at
    deploy time.
- **App Service Plan** – The system runs on an Azure App Service Plan with multiple
    instances for high availability.
- **API gateway** – An **Azure Application Gateway**
    terminates TLS, validates JWTs, throttles requests and routes
    traffic to the application.
- **Persistent storage** – A single managed **Azure SQL database** is provisioned for the application. Azure Disks provide persistent
    volumes for stateful workloads. Azure Files can be used for shared
    storage if needed.
- **Networking** – The App Service is deployed in a dedicated virtual
    network. Network policies restrict traffic; private
    endpoints secure connections to Azure SQL, Blob Storage, Service Bus
    and Redis. Only the Application Gateway is exposed on a public IP.

## 5. Data flow & integration patterns

**Synchronous flow** – When a user submits an order, the Angular
front‑end sends a POST request to the Order Management module. The
Application Gateway routes the request to the application, which validates the order, reserves inventory and authorizes
payment. It returns a confirmation response.

**Asynchronous event flow** – After acceptance, the Order module
publishes an `OrderPlaced` event to the in-process event bus.
Subscriber modules (Inventory, Shipping, Analytics) consume the event. Inventory decrements stock levels;
Shipping creates a shipment; Analytics updates dashboards. This pattern
decouples modules and improves resilience and throughput.

**Batch & integration workflows** – The Purchasing module periodically
generates purchase orders based on reorder points and economic order
quantity. It integrates with suppliers via EDI or REST to send POs and
receive ASNs. Integrations with tax services, carriers and Amazon FBA
use dedicated adapters within the respective module.

## 6. Security & compliance

- **Identity & access management** – Microsoft Entra ID provides SSO
    for users and managed identities for the application. The Application Gateway validates tokens and forwards claims to the application.
- **RBAC enforcement** – Fine‑grained permissions defined in the RBAC
    matrix are enforced in each module. The gateway performs
    coarse checks (e.g., verifying the user belongs to a particular
    role), while modules enforce domain‑specific access controls.
- **Data protection** – All API calls use TLS 1.2+. Data at rest is
    encrypted using platform keys or customer‑managed keys. Secrets are
    stored in Azure Key Vault and injected into the application as environment
    variables.
- **Compliance & auditing** – The architecture supports
    hazardous‑materials handling regulations, multi‑state tax
    requirements and financial audit trails. Logs and change history are
    captured and retained for regulatory review.

## 7. Scalability, availability & performance

- **Horizontal scaling** – The application scales by
    adjusting its instance count in the App Service Plan. The App Service autoscaler monitors CPU, memory
    and queue metrics to add or remove instances automatically.
- **High availability** – The application is deployed to multiple instances.
    Azure App Service health checks restart unhealthy instances. Deployment slots are used for blue-green deployments to minimize downtime.
- **Performance considerations** – The database is provisioned with
    adequate capacity. Caching via Redis reduces latency.
    Resource‑intensive tasks (e.g., cycle counts, forecasting) run as
    background jobs.

## 8. Monitoring & observability

- **Metrics & logs** – Azure Monitor collects application and
    infrastructure metrics. Application Insights gathers request traces,
    dependencies, failures and performance metrics. Dashboards visualise
    key indicators such as order throughput, API latency, inventory
    accuracy and error rates.
- **Tracing & diagnostics** – Distributed tracing is enabled using
    Application Insights or OpenTelemetry. Correlation IDs propagate
    across modules to follow a request end‑to‑end. Log analytics
    surfaces anomalies and supports incident investigations.

## 9. Future considerations & Phase 2

- **Evolution to Microservices**: The modular monolith architecture allows for future evolution to microservices. As the application grows and certain modules become more complex or require independent scaling, they can be extracted into separate services.
- **Demand forecasting & analytics** – Introduce a forecasting module
    that uses historical sales and seasonality to predict future demand.
    The module may leverage Azure Machine Learning and publish forecast
    data into the Analytics module.
- **Promotion engine & contract pricing** – Add a module to manage
    promotional campaigns, coupon codes and customer‑specific pricing.
    These integrate with the Order module to apply discounts at order
    time.
- **Light manufacturing & kitting** – Introduce a kitting module that
    manages bills of materials and assembly instructions. The module
    interfaces with the Inventory and Order modules to deduct
    components and produce finished goods.
- **3PL integration & EDI** – Develop adapters for third‑party
    logistics providers (3PLs) to exchange shipment and inventory
    updates. Implement EDI for large suppliers to automate purchase
    orders, invoices and ASNs.
- **Customer service console** – Build a console for customer service
    representatives that aggregates order history, shipment tracking,
    returns status and SLA timers. The console subscribes to events and
    queries the underlying modules in real time.

This architecture provides a scalable, secure and extensible foundation
for PineCone Pro Supplies’ ERP/IMS. It leverages proven Azure services
and a modular monolith approach to deliver a unified platform that
meets current requirements and supports future growth.
