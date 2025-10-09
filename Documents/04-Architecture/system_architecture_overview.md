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

**Cloud‑native & containerized** – The application is containerized using a multi-stage Docker build. The base image comes from the official .NET SDK and ASP.NET runtime for the backend and Node/Nginx for the Angular front‑end. The container image is stored in a private Azure Container Registry and deployed on **Azure Kubernetes Service (AKS)** for elasticity and operational consistency.

**12‑Factor & DevOps** – Follow 12‑factor application principles:
configuration via environment variables, stateless application, automated
build/test/release pipelines and comprehensive telemetry. Adopt
continuous integration and continuous deployment (CI/CD) using GitHub
Actions workflows.

**Security by design** – Enforce role‑based access control (RBAC),
least‑privilege service accounts and network segmentation. Use
**Microsoft Entra ID** for user and workload identities and **Azure
Key Vault** for secrets management. All traffic is encrypted.

**Scalability & resilience** – Design for high availability and load:
run multiple pods across AKS node pools, use the Horizontal Pod Autoscaler and implement circuit
breakers/retries for external integrations. AKS and Application Gateway provide built-in support for scaling and load balancing.

**Internationalization & Localization** – The architecture is designed to support future global expansion. The front-end and back-end will be built with internationalization (i18n) in mind, allowing for easy localization (l10n) of UI text, number formats, and date/time representations. The system will support multiple currencies and region-specific tax regulations.

## 2. High‑level architecture

The system consists of a web‑based front‑end and a back‑end application deployed as a single unit. Users access the system via
browsers, POS devices or mobile apps. Internal staff (customer service,
warehouse associates and purchasing) and external actors (customers,
vendors, carriers and tax services) interact with the platform.

An **Azure Application Gateway** acts as the API gateway. Requests
from clients first hit the gateway, which terminates TLS, authenticates using
Entra ID, performs rate limiting and routes requests based on the URL
path to the application.

The application runs on **Azure Kubernetes Service (AKS)** with a managed ingress controller that exposes HTTPS endpoints. The application uses a single **Azure Database for PostgreSQL** with separate schemas for each module. Event‑driven workflows use an in-process event bus or **Azure Service Bus** for decoupling.

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
| **Front-end**                      | Angular 20 SPA                                            | Uses TypeScript, RxJS and Angular Material. Consumes REST APIs via the Angular HttpClient module and reads the API base URL from a JSON configuration file that is injected at runtime.        |
| **Back-end**                       | ASP.NET Core 9 Modular Monolith                            | Built with Clean Architecture and Domain-Driven Design. Communicates internally via an in-process event bus. Exposes public RESTful APIs.                                                      |
| **Database**                       | Azure Database for PostgreSQL; Azure Cache for Redis | A single Azure Database for PostgreSQL server with separate schemas per module. Redis provides caching; Azure Blob Storage stores binary objects. |
| **Messaging**                      | In-process event bus or Azure Service Bus                  | An in-process event bus handles internal, asynchronous communication between modules. Azure Service Bus is used for external integrations.                                                     |
| **Authentication & Authorization** | Microsoft Entra ID                                         | Provides single sign-on for users and managed identities for services. JWT bearer tokens secure API calls, and RBAC is enforced by the API gateway and aplication.                        |
| **Containerization**               | Docker multi-stage builds                                  | Use .NET SDK and runtime images to restore and publish the backend API and Node/Nginx images to build and serve the Angular app. Images are stored in Azure Container Registry.                |
| **Deployment**                     | Azure Kubernetes Service (AKS)                             | Runs the containerized modular monolith across multiple node pools; integrates with Azure networking, managed identities, and auto-scaling.                                                    |
| **CI/CD**                          | GitHub Actions                                             | Build, test and push container images to ACR. Deploy manifests/Helm charts to AKS via OIDC-authenticated workflows with environment protections.                                                |
| **Monitoring & Logging**           | Azure Monitor & Application Insights                       | Collect metrics, logs and traces. Provide dashboards and alerts for performance and business KPIs.                                                                                             |

## 4. Deployment & infrastructure

- **Container registry** – A private **Azure Container Registry**
    hosts container images for the application. AKS pulls signed images
    using managed identities.
- **AKS cluster** – The system runs on an AKS cluster with multiple
    node pools sized for front-end and background workloads; cluster
    autoscaler maintains availability across zones.
- **API gateway** – An **Azure Application Gateway**
    terminates TLS, validates JWTs, throttles requests and routes
    traffic to the AKS ingress.
- **Persistent storage** – A single managed **Azure Database for PostgreSQL** server stores transactional data. Azure Disks provide persistent
    volumes for stateful workloads. Azure Files can be used for shared
    storage if needed.
- **Networking** – The AKS cluster resides in a dedicated virtual
    network. Network policies restrict traffic; private
    endpoints secure connections to Azure Database for PostgreSQL, Blob Storage, Service Bus
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

**Product Data Synchronization** - When a product is created or updated in the PIM module, it publishes a `ProductUpdated` event. The Inventory module subscribes to this event to update its local product data, ensuring that all modules have the latest product information.

**Warehouse Operations (RF Scanning)** - Warehouse associates use RF scanners for receiving, put-away, picking, and cycle counts. The RF scanner application communicates with the Inventory & Warehouse Module via REST APIs to validate data and update inventory levels in real-time.

**Return Merchandise Authorization (RMA) Processing** - When a customer requests a return, a CSR creates an RMA in the Returns & RMA Module. This action publishes an `RmaCreated` event. The Inventory module subscribes to this event to update the stock level of the returned item based on its disposition (e.g., restock, scrap).

**Financial Data Synchronization** - At the end of each day, a batch job in the Tax & Accounting Module generates journal entries for all transactions (e.g., sales, returns, purchases). These entries are then exported to the external accounting system via a file-based integration.

**Analytics & Alerting** - The Analytics & Alerting Module subscribes to events from all other modules to build a near real-time data warehouse. This data is used to generate persona-based dashboards and reports. The module also runs scheduled jobs to check for KPI thresholds and sends alerts to stakeholders via email or other notification channels when a threshold is breached.

## 6. Security & compliance

For a detailed breakdown of security requirements and the threat model, see the [Security Requirements & Threat Model](./security_requirements_and_threat_model.md) document.

- **Identity & access management** – Microsoft Entra ID provides SSO and MFA for users and managed identities for the application. The Application Gateway validates tokens and forwards claims to the application.
- **RBAC enforcement** – Fine‑grained permissions defined in the RBAC matrix are enforced in each module. The gateway performs coarse checks (e.g., verifying the user belongs to a particular role), while modules enforce domain‑specific access controls.
- **Data protection** – All API calls use TLS 1.2+. Data at rest is encrypted using platform keys or customer‑managed keys. Secrets are stored in Azure Key Vault and injected into the application as environment variables. Sensitive data is masked in logs and lower environments.
- **Network Security** - The AKS cluster resides in a private virtual network with Network Security Groups (NSGs) to restrict traffic. The Application Gateway includes a Web Application Firewall (WAF) to protect against common web vulnerabilities.
- **Application Security** - The development process follows a Secure Software Development Lifecycle (SSDLC), including static and dynamic application security testing (SAST/DAST) and automated dependency scanning.
- **Container Security** - Container images are scanned for vulnerabilities, and runtime protection is enabled in the AKS cluster.
- **Compliance & auditing** – The architecture supports hazardous‑materials handling regulations, multi‑state tax requirements, PCI DSS, and financial audit trails. Logs and change history are captured and retained for regulatory review.

## 7. Scalability, availability & performance

For a detailed breakdown of non-functional requirements, see the [Non-Functional Requirements Document](../03-Product%20&%20Requirements/non_functional_requirements.md).

- **Horizontal scaling** – The application is designed to scale horizontally by adjusting the number of replicas (pods) in the AKS cluster. The Horizontal Pod Autoscaler (HPA) will be used to automatically scale the application based on CPU and memory utilization, as well as custom metrics such as the number of messages in a queue.
- **High availability** – The system is deployed across multiple availability zones in Azure to ensure high availability. AKS nodes are distributed across these zones, and the Application Gateway provides load balancing and health checks to automatically route traffic to healthy pods. In the event of a zone failure, traffic will be automatically redirected to the remaining zones.
- **Performance considerations** – The application is designed to meet the performance targets outlined in the non-functional requirements, including support for 200 concurrent users and order processing latency of less than 2 seconds. Caching with Azure Cache for Redis is used to reduce latency for read-heavy operations. Resource-intensive tasks, such as reporting and batch jobs, are offloaded to background workers to avoid impacting the main application threads.
- **Disaster Recovery** - The disaster recovery plan includes regular backups of the database and application data, with a Recovery Time Objective (RTO) of less than 1 hour and a Recovery Point Objective (RPO) of less than 5 minutes. The database is configured with point-in-time recovery, and backups are stored in a geo-redundant storage account.

## 8. Monitoring & observability

For a detailed breakdown of the observability strategy, see the [Observability, Logging & Alert Catalog](../08-Operations/observability_and_alert_catalog.md) and the [Observability Implementation Checklist](../08-Operations/observability_implementation_checklist.md).

- **Metrics & logs** – The system collects the four golden signals (latency, traffic, errors, and saturation) for all services. Structured JSON logs are generated for all requests and events, and are centralized in Azure Monitor Logs. Dashboards are created using Azure Monitor Workbooks to visualize key indicators such as order throughput, API latency, inventory accuracy, and error rates.
- **Tracing & diagnostics** – Distributed tracing is enabled using OpenTelemetry and Application Insights. Correlation IDs are propagated across all services to allow for end-to-end tracing of requests. Log analytics is used to surface anomalies and support incident investigations.
- **Alerting** - A comprehensive alerting strategy is in place, with alerts categorized by severity (P1, P2, P3) and routed to the appropriate on-call engineers. Alerts are symptom-based, actionable, and include links to runbooks for faster resolution.

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
