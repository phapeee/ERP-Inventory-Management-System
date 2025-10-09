Architecture Decision Records
=============================

This document logs the key architectural decisions made for the
PineCone Pro Supplies ERP/IMS project. Each record captures the context,
the decision taken, alternatives considered, the rationale and the
consequences. The log is maintained to provide traceability for future
team members and to avoid revisiting the same questions.

ADR‑001: Adopt a Microservices Architecture
-------------------------------------------

**Date**: 2025‑09‑28  
**Status**: Rejected (Superseded by ADR-012)

### Context

The ERP/IMS platform must support a wide range of functions—product
information management, inventory, order management, purchasing,
shipping, tax and accounting—and must evolve quickly as the business
grows. A monolithic application would be complex to develop, deploy and
scale across these diverse domains. We therefore need to decide how to
structure the system.

### Decision

The solution will be built as a suite of domain‑focused microservices.
Each domain (e.g., Product Information, Inventory, Orders, Purchasing,
Shipping) will be implemented as an independent service running in its
own container and owning its own data store. Services communicate
through RESTful APIs for synchronous requests and through asynchronous
events for workflow orchestration.

### Alternatives

- **Monolithic application**: Build a single codebase with all modules
    deployed together.
- **Modular monolith**: Use modules within a single deployment but
    without network boundaries.
- **Service‑oriented architecture (SOA)**: Implement shared services
    with an enterprise service bus.

### Rationale

- Microservices allow **independent scaling** of each domain. If one
    service experiences heavy seasonal demand, it can be scaled up
    without affecting others. This improves resource utilisation and
    reduces cost.
- The architecture improves **fault isolation**; a failure in one
    service is less likely to cascade across the system. Developers can
    deploy new modules without redesigning the entire application,
    enabling **faster time‑to‑market**.
- Microservices are **language‑agnostic**, allowing each service to
    use the most suitable technology stack. In our case, we have chosen
    ASP.NET Core for all services to maintain consistency but future
    extensions could use other languages.
- Each service manages its own database, improving **data security**
    and allowing compliance boundaries to be clearly defined.

### Consequences

- Microservices introduce operational complexity (container
    orchestration, service discovery, network policies).
- Testing and debugging distributed systems is more complex than with
    a monolith. Integration testing requires orchestrating multiple
    services.
- Managing multiple deployment pipelines and APIs demands robust
    DevOps practices and governance.

ADR‑002: Choose Angular for the Front‑End
-----------------------------------------

**Date**: 2025‑09‑28  
**Status**: Accepted

### Context

The system requires a responsive, feature‑rich web user interface that
can serve both B2B and B2C customers, as well as internal staff. The
front‑end must be modular, maintainable and capable of rapid development
by multiple developers.

### Decision

Angular (version 14+) will be the primary front‑end framework for the
ERP/IMS. The application will be a single‑page application (SPA) that
consumes RESTful APIs and uses Angular’s CLI for build and deployment.
The UI will use Angular Material and the component‑based architecture to
deliver a consistent experience across web, tablet and mobile.

### Alternatives

- **React**: A popular component library with more flexibility but a
    less opinionated structure.
- **Vue.js**: Lightweight framework suitable for smaller applications.
- **Razor pages/Blazor**: Use server‑side rendering or WebAssembly
    with .NET.

### Rationale

- Angular’s **component‑based architecture** and **modular design**
    help break down complex UIs into reusable pieces, which suits our
    multi‑module ERP.
- The framework provides **high‑performing, adaptable single‑page
    applications** and includes TypeScript, which adds static typing and
    improves maintainability.
- Angular CLI and its opinionated structure make it easier for larger
    teams to collaborate; modules can be shared as NPM libraries, which
    streamlines development across teams.
- Angular Material supplies ready‑made UI components and supports
    responsive design, reducing the time to build a polished interface.

### Consequences

- Angular has a steeper learning curve than some alternatives and
    requires developers to be comfortable with TypeScript and RxJS.
- The framework is opinionated and may be less flexible for
    unconventional UX patterns.
- Major upgrades (e.g., Angular 15, 16) might require refactoring
    parts of the codebase.

ADR‑003: Use ASP.NET Core for Microservices
-------------------------------------------

**Date**: 2025‑09‑28  
**Status**: Superseded by ADR-013

### Context

We need a reliable, high‑performance backend technology for building
microservices that can handle thousands of requests per second and run
cross‑platform on Linux containers. The framework should have strong
tooling, robust security and a large ecosystem.

### Decision

All backend services will be implemented using ASP.NET Core 6 or later.
Services will expose RESTful APIs, implement dependency injection, and
use Entity Framework Core for database access. Each service will be
packaged into a Docker container for deployment on Kubernetes.

### Alternatives

- **Node.js with Express**: Lightweight, event‑driven JavaScript
    runtime.
- **Java Spring Boot**: A mature, feature‑rich framework for building
    microservices.
- **Python Django/Flask**: Rapid development frameworks but less
    performant for high‑traffic APIs.

### Rationale

- ASP.NET Core is **cross‑platform**; it runs on Windows, Linux and
    macOS. Its Docker integration allows deployment in any environment
    and fits well with Kubernetes.
- The framework delivers **high performance** through a lightweight
    request pipeline and the Kestrel web server, outperforming many
    alternatives in benchmarks.
- It includes **built‑in support for dependency injection**,
    configuration management and environment targeting, which simplifies
    testability and clean architecture.
- ASP.NET Core integrates with **Entity Framework Core** for data
    access, supports multiple databases (SQL Server, PostgreSQL, MySQL)
    and can also connect to NoSQL stores.
- Security is a first‑class citizen: the framework provides
    authentication and authorization support for JWT, OAuth2 and OpenID
    Connect and includes middleware for enforcing HTTPS, CSRF and data
    protection.

### Consequences

- The development team must maintain proficiency in C# and .NET.
- While ASP.NET Core is open source, it is primarily stewarded by
    Microsoft; strategic changes may be influenced by vendor priorities.
- Some libraries and tools may be less mature than in other ecosystems
    (e.g., Node.js or Java), although the .NET ecosystem has grown
    substantially.

ADR‑004: Adopt Domain‑Driven Design & Bounded Contexts
------------------------------------------------------

**Date**: 2025‑09‑28  
**Status**: Accepted

### Context

To define module boundaries, we need a systematic approach to
decompose the business domain. The ERP/IMS spans diverse functions
(product management, orders, purchasing, warehousing). Without proper
boundaries, modules could end up too coarse or too coupled.

### Decision

We will apply **Domain‑Driven Design (DDD)** principles to define
bounded contexts. Each module in our application corresponds to a bounded context
that aligns with a domain area (e.g., Product, Inventory, Orders).
Within each context we will model aggregates, entities and value objects
that encapsulate business rules and invariants.

### Alternatives

- **Layered architecture without DDD**: Decompose by technical layers
    (e.g., presentation, business, data) but without explicit domain
    boundaries.
- **Anemic domain model**: Use data transfer objects and services
    without rich domain models.
- **Event storming only**: Use event discovery to identify bounded
    contexts without fully adopting DDD.

### Rationale

- DDD advocates modeling based on the reality of business domains and
    uses **bounded contexts** to define clear boundaries where a model
    applies. Each context correlates to a module and emphasises a
    common business language.
- Using DDD helps align the software with business processes and
    reduces coupling between modules; the ubiquity of the model improves
    communication between developers and domain experts.
- DDD encourages rich domain models (aggregates, entities, value
    objects) and encapsulates business rules behind aggregate roots,
    which supports clean and maintainable code.
- Bounded contexts help identify when modules should remain
    independent and when a function should not be split (to avoid tight coupling
    and poor autonomy).

### Consequences

- DDD has a learning curve and requires collaboration with domain
    experts.
- It may introduce complexity where simple CRUD operations could
    suffice; thus we will apply DDD primarily to modules with
    significant business rules (e.g., Orders, Purchasing) and use
    simpler patterns for CRUD‑heavy modules.
- The process of defining bounded contexts and ubiquitous language
    will require workshops and ongoing refinement as the business
    evolves.

ADR‑005: Containerization & Azure Container Registry
----------------------------------------------------

**Date**: 2025‑09‑28  
**Status**: Accepted

### Context

We need a consistent runtime environment across development, testing and
production and a way to package our application's components along with their
dependencies. We also need a secure repository to store and manage
container images.

### Decision

The backend and front‑end applications will be containerized using
Docker. We will use **multi‑stage Docker builds**: for the backend,
the first stage will restore and publish the .NET code, and the final
stage will copy the output into a lightweight runtime image; for the
front‑end, the first stage will build the Angular app using a Node image
and the second stage will host it with an Nginx server. Images will be
stored in a private **Azure Container Registry (ACR)**.

### Alternatives

- **Run code directly on VMs**: Deploy the application to virtual
    machines without containers.
- **Use Azure App Service**: Deploy the application as an App Service app.
- **Publish Docker images to Docker Hub**: Use a public registry.

### Rationale

- Containers ensure that the application runs consistently on any environment
    (developer workstation, CI server, production cluster). Multi‑stage builds
    reduce image size by separating build and runtime stages, improving
    security and performance.
- A private ACR secures images and integrates with Azure AD for
    authentication. The production environment can pull images using its managed identity
    without exposing credentials.
- Containerization makes it easy to replicate and scale the application on
    our chosen deployment platform, enabling rapid deployments and rollbacks.

### Consequences

- Developers must learn Docker basics and manage Dockerfiles for the
    application.
- Image storage and scanning must be managed (ACR tasks, vulnerability
    scanning).
- ACR introduces additional cost compared with public registries but
    improves security.

ADR‑006: Deployment Platform – Azure Kubernetes Service
-------------------------------------------------------

**Date**: 2025‑09‑28  
**Status**: Accepted

### Context

Our containerized application requires an orchestration platform that can
handle high availability, scaling, rolling upgrades, service discovery
and secure networking. Azure offers multiple compute options including
AKS, App Service and Container Apps. We must choose the deployment
platform.

### Decision

The backend and front-end containers will be deployed on **Azure
Kubernetes Service (AKS)**. An **Azure Application Gateway** will
handle incoming HTTP(S) traffic and route it to the backend service.
AKS will run across multiple availability zones for high availability
and will integrate with Azure Load Balancer for external connectivity.

### Alternatives

- **Azure App Service**: Deploy the application as a separate web app.
- **Azure Container Apps**: Use a serverless container platform.
- **Self‑managed Kubernetes**: Operate Kubernetes clusters on virtual
    machines.

### Rationale

- AKS is a **managed Kubernetes** platform, offloading cluster
    management tasks while providing full Kubernetes APIs and features.
- An **Azure Application Gateway** implements the API gateway pattern; it
    terminates TLS, validates JWT tokens and routes traffic based on URL
    paths. This simplifies client interaction and reduces duplication of
    cross‑cutting concerns.
- AKS integrates with Azure AD for cluster authentication and with ACR
    for pulling container images via managed identities.
- AKS supports autoscaling, rolling upgrades, node pools and
    multi‑zone deployments, which are critical for a production ERP
    system.

### Consequences

- Kubernetes adds operational complexity; the team must manage
    manifests, Helm charts and cluster policies.
- AKS incurs management costs; cost optimization is required.
- There is a learning curve for Kubernetes, but it provides powerful
    primitives for scaling and resilience.

ADR‑007: Persistence & Data Stores
----------------------------------

**Date**: 2025‑09‑28  
**Status**: Superseded by ADR-014

### Context

Each microservice must persist data and maintain its own state. The
ERP/IMS system deals with transactional data (orders, inventory levels),
reference data (products, vendors) and potentially large volumes of
semi‑structured data. We need to choose appropriate data stores and
determine whether services share databases.

### Decision

Each microservice will have its own **Azure Database for PostgreSQL** instance to
store its transactional data. Where appropriate (e.g., product
attributes with varying schemas), **Azure Cosmos DB** may be used.
**Azure Cache for Redis** will be used for caching frequently accessed
data. Files and attachments will be stored in **Azure Blob Storage**.

### Alternatives

- **Shared database**: All services share a single database instance.
- **NoSQL only**: Use a document database such as Cosmos DB for all
    services.
- **Self‑hosted SQL**: Run a central SQL Server in a VM.

### Rationale

- The microservices reference architecture suggests using **external
    data stores** such as Azure Database for PostgreSQL, Cosmos DB and Redis for stateful
    microservices. This ensures data is persisted outside of the cluster
    and can scale independently.
- Having a **database per service** avoids tight coupling and allows
    each service to evolve its schema independently. It supports
    independent scaling and deployment.
- Azure Database for PostgreSQL provides managed, highly available relational
    storage with PostgreSQL compatibility; Cosmos DB provides schema flexibility for semi‑structured
    data; Redis provides low‑latency caching for read‑heavy workloads.
- Storing binary objects in Blob Storage decouples large file storage
    from database operations.

### Consequences

- Managing multiple databases increases administrative overhead;
    automated provisioning and migration scripts are required.
- Data must be synchronised across services via events or explicit
    APIs, leading to eventual consistency.
- Developer tooling must support multiple connection strings and
    database contexts.

ADR‑008: Asynchronous Messaging & Service Bus
---------------------------------------------

**Date**: 2025‑09‑28  
**Status**: Accepted

### Context

Synchronous API calls can result in tight coupling between modules and
can degrade performance when a downstream service is unavailable or
slow. The ERP/IMS requires asynchronous workflows (e.g., order
processing, inventory updates) and event‑driven patterns to decouple
modules and improve resilience. This ADR focuses on asynchronous communication with external systems.

### Decision

We will use **Azure Service Bus** topics and queues for asynchronous
messaging with external systems. The application will publish events (e.g., OrderPlaced,
InventoryReserved, ShipmentCreated) to topics. Other systems will
subscribe to these events using the **publisher‑subscriber** and
**competing consumer** patterns. Service Bus will also be used for
command queues when appropriate.

### Alternatives

- **Synchronous REST for all interactions**: Systems call each other
    directly via HTTP.
- **Kafka**: Use Apache Kafka for messaging.
- **RabbitMQ**: Self‑hosted message broker.

### Rationale

- The reference architecture uses Service Bus to queue delivery
    requests and illustrates the **publisher‑subscriber** and
    **competing consumers** patterns for decoupled workflows. Using
    Service Bus ensures at‑least‑once delivery and durable messaging.
- Events allow systems to react to changes without tight coupling;
    this improves scalability and fault tolerance.
- Azure Service Bus is fully managed and integrates with Azure AD; it
    scales horizontally and supports topics, subscriptions, dead‑letter
    queues and message sessions.

### Consequences

- Additional complexity: message contracts and versioning must be
    managed carefully.
- Systems become eventually consistent; consumers must handle
    idempotency and duplication.
- Operational monitoring of queues and handling of poison messages is
    required.

ADR‑009: Authentication & Authorization
---------------------------------------

**Date**: 2025‑09‑28  
**Status**: Accepted

### Context

The ERP/IMS is accessed by multiple user roles (e.g., customers,
warehouse associates, purchasing leads) and services. We need to secure
APIs and implement role‑based access control across the system.

### Decision

We will use **Microsoft Entra ID** (formerly Azure Active Directory) for
user authentication, single sign‑on and role management. The backend application
will validate JSON Web Tokens (JWTs) issued by Entra ID.
AKS and service pods will use **managed identities** to access Azure
resources (e.g., PostgreSQL databases, Service Bus). Role‑based access control
(RBAC) will be enforced at the API gateway and within the application.

### Alternatives

- **Custom identity provider**: Implement our own token issuing and
    management.
- **Auth0/Okta**: Use a third‑party identity service.
- **Basic authentication**: Use simple username/password for each
    service.

### Rationale

- The reference architecture requires a **managed
    identity** for the cluster to access Azure Container Registry and
    other resources. Using Entra ID provides unified identity management
    and integrates with RBAC.
- Entra ID supports OpenID Connect, OAuth2 and multi‑factor
    authentication; it provides enterprise security features such as
    conditional access and identity protection.
- Managed identities simplify service‑to‑service authentication and
    allow secure access to Azure services without storing secrets.

### Consequences

- Requires configuration of Azure AD tenants, app registrations and
    service principals.
- External partners may need guest accounts or B2B integration.
- If the organisation uses another identity provider, integration with
    Entra ID must be established.

ADR‑010: Continuous Integration & Deployment (CI/CD)
----------------------------------------------------

**Date**: 2025‑09‑28  
**Status**: Superseded by ADR-015

### Context

To ensure reliable releases and reduce manual errors, the ERP/IMS
project needs automated processes for building, testing and deploying
microservices and the front‑end. Multiple teams will contribute to
different services, so the pipeline must support independent
deployments.

### Decision

We will adopt **Azure Pipelines** for CI/CD. Each microservice and the
front‑end application will have its own pipeline that performs linting,
unit tests, integration tests, builds Docker images and publishes them
to Azure Container Registry. We will use **Helm** charts to package
Kubernetes manifests and deploy to AKS. Pipelines will promote releases
through dev, staging and production environments with approvals and
automated smoke tests.

### Alternatives

- **GitHub Actions**: Use GitHub‑hosted runners for CI/CD.
- **Jenkins**: Self‑hosted CI server.
- **Manual deployment**: Build and deploy images manually.

### Rationale

- Azure Pipelines provides a cloud‑hosted CI/CD service tightly
    integrated with Azure services and AKS; it can run automated builds,
    tests and deployments and supports multiple repositories and stages.
- Using Helm standardises the deployment process and allows versioned
    releases; charts bundle all Kubernetes objects into a single unit
    that can be deployed and rolled back.
- Pipelines allow each microservice team to own its pipeline and
    release cadence, supporting parallel development.

### Consequences

- Requires configuration and maintenance of YAML pipeline definitions
    and Helm charts.
- Additional compute cost for pipeline execution.
- Teams must monitor and respond to pipeline failures and maintain
    secrets for deployment (handled via Azure Key Vault).

ADR‑011: Observability & Monitoring
-----------------------------------

**Date**: 2025‑09‑28  
**Status**: Accepted

### Context

Operating a distributed application requires comprehensive
observability to detect and diagnose issues, monitor performance and
ensure reliability. The ERP/IMS must track system health as well as
business‑level metrics (e.g., order processing times, inventory
accuracy).

### Decision

We will use **Azure Monitor** and **Application Insights** to collect
metrics, logs and traces from all of its modules and the AKS
infrastructure. Each module will emit structured logs and telemetry
that can be correlated via a common operation identifier. Dashboards
will be set up to visualize key indicators and alerts will notify the
on‑call team of anomalies.

### Alternatives

- **Prometheus + Grafana**: Deploy open‑source monitoring stack.
- **ELK Stack**: Use Elasticsearch, Logstash and Kibana for logging.
- **Third‑party APM**: Use tools such as Datadog or New Relic.

### Rationale

- Azure Monitor integrates with AKS to collect metrics from
    controllers, nodes and containers; it stores logs centrally and
    supports querying via Kusto.
- **Application Insights** monitors the application and provides
    distributed tracing, dependency tracking and availability tests,
    enabling end-to-end visibility of request flows within the application.
- Using native Azure tools reduces operational overhead and seamlessly
    integrates with Azure RBAC and identity management.

### Consequences

- Azure Monitor and Application Insights incur usage‑based costs;
    careful retention and sampling policies are necessary.
- Additional instrumentation code must be added to modules to capture
    custom metrics and traces.
- Development teams must learn Kusto Query Language (KQL) for querying
    logs and metrics.

ADR‑012: Adopt a Modular Monolith Architecture
---------------------------------------------

**Date**: 2025-09-29
**Status**: Accepted

### Context

The initial proposal was to adopt a microservices architecture (ADR-001). However, after further consideration, the team has decided that a modular monolith is a more appropriate starting point for the PineCone Pro ERP/IMS project. The project requires a wide range of functionalities, and a microservices architecture, while scalable, introduces significant operational complexity that may not be necessary at this stage.

### Decision

The solution will be built as a modular monolith. The application is a single deployable unit, but it is internally structured into well-defined modules corresponding to business domains (Product Information, Inventory, etc.).

All inter-module communication will be handled exclusively through an **in-process event bus** using a publish-subscribe pattern. Direct, in-process function calls between modules are disallowed to enforce strict decoupling. Modules interact by publishing and consuming domain events.

### Alternatives

- **Hybrid approach**: Allow both in-process calls and events for communication. This was rejected to enforce stricter decoupling.
- **Microservices architecture**: As described in ADR-001.
- **Traditional monolith**: A single, tightly coupled application with no clear module boundaries.

### Rationale

Choosing an exclusively event-driven approach for inter-module communication provides several key advantages over direct calls:

- **Maximum Decoupling**: Modules do not hold direct references to each other. They only need to be aware of the event contracts, which reduces coupling to an absolute minimum. This allows individual modules to be developed, modified, and tested with greater independence.
- **Improved Resilience**: The event bus can act as a buffer. If a consumer module is temporarily unavailable, the system can be designed with retries or queues to process the event later, preventing the failure from cascading to the producer.
- **Enhanced Evolvability and Scalability**: This architecture makes it significantly easier to extract a module into a separate microservice in the future. The asynchronous, event-based communication pattern is already in place, so the transition would not require a major refactoring of how other modules interact with it.
- **Clearer Business Workflows**: Modeling interactions as a series of events often provides a clearer and more explicit representation of business processes (e.g., `OrderPlaced` -> `InventoryReserved` -> `ShipmentInitiated`).

### Consequences

- **Scalability**: The application will scale as a single unit. If one module requires more resources, the entire application needs to be scaled up. However, for the current scale of PineCone Pro, this is not expected to be a major issue.
- **Technology Stack**: The entire application will be built using a single technology stack (ASP.NET Core and Angular). This limits the flexibility to use different technologies for different modules, but it also simplifies development and maintenance.
- **Deployment**: The entire application needs to be deployed for any change in any module. This can slow down the development and release cycle compared to microservices. However, with a good CI/CD pipeline, this can be mitigated.

These Architecture Decision Records capture the key decisions and their
justifications for the PineCone Pro ERP/IMS project. This log will be
maintained as the system evolves. New decisions should follow this
template, and existing records should not be altered once accepted;
instead, superseding ADRs should be created if a decision changes.

ADR-013: Backend Technology for the Modular Monolith
-----------------------------------------------------

**Date**: 2025-09-30
**Status**: Accepted

### Context

With the decision to adopt a modular monolith architecture (ADR-012), we need to confirm our choice of backend technology. The technology must be reliable, high-performance, and suitable for building a large, modular application that can be deployed as a single unit. It must also support cross-platform deployment in Linux containers and have a robust ecosystem.

### Decision

The backend application will be implemented using ASP.NET Core 6 or later. It will expose RESTful APIs, implement dependency injection, and use Entity Framework Core for database access. The entire application will be packaged into a single Docker container for deployment.

### Alternatives

- **Node.js with Express**: Lightweight, event‑driven JavaScript runtime. While suitable for smaller services, it can be less structured for a large monolithic application.
- **Java Spring Boot**: A mature, feature‑rich framework that is also a strong candidate for building modular monoliths. However, the team has more experience with the .NET ecosystem.
- **Python Django/Flask**: Rapid development frameworks, but generally less performant for the high-throughput requirements of our ERP system.

### Rationale

- **Performance:** ASP.NET Core is known for its high performance, which is critical for a responsive ERP system that needs to handle thousands of requests per second.
- **Unified Framework:** Using a single, comprehensive framework like ASP.NET Core for the entire backend simplifies development, maintenance, and dependency management.
- **Modularity Support:** ASP.NET Core's support for dependency injection and its overall design make it well-suited for building a clean, modular architecture within a single application. This aligns perfectly with our modular monolith approach.
- **Cross-Platform and Containerization:** Its cross-platform nature and excellent Docker integration allow for consistent and easy deployment to our Linux-based cloud environment.
- **Ecosystem and Tooling:** The .NET ecosystem provides a rich set of libraries, tools, and community support, which will accelerate development.

### Consequences

- The development team must maintain proficiency in C# and the .NET ecosystem.
- The entire application is tied to the .NET technology stack, which limits the flexibility to use other languages for different modules (though this is an accepted trade-off of the monolith architecture).

ADR-014: Persistence Strategy for the Modular Monolith
-------------------------------------------------------

**Date**: 2025-09-30
**Status**: Accepted

### Context

With the adoption of a modular monolith architecture (ADR-012), the persistence strategy needs to be defined. Unlike microservices, where each service has its own database, a monolith typically uses a single database. We need to decide how to manage data for the different modules within this single database to maintain modularity and avoid tight coupling.

### Decision

The application will use a **single Azure Database for PostgreSQL** server. Within this database, each module will have its own dedicated **database schema**. This approach, often called "schema-per-module," provides a good balance between the simplicity of a single database and the logical separation of a multi-database approach.

- **Data Access:** Modules are not allowed to directly access the tables of another module. All inter-module data access must happen through the public API of the owning module or by listening to its published events.
- **Shared Kernel:** A small, shared "kernel" schema may be used for data that is truly shared across all modules, such as user and role information. This should be kept to a minimum.
- **Other Data Stores:** For specific needs, other data stores can be used. For example, **Azure Cache for Redis** for caching, and **Azure Blob Storage** for storing large files and attachments.

### Alternatives

- **One schema for all modules:** This is the traditional monolith approach, but it leads to a highly coupled database where changes in one module can easily break another.
- **One database per module:** This is the microservices approach (as described in the now-superseded ADR-007). It provides strong isolation but adds significant operational overhead for a monolithic deployment.

### Rationale

- **Simplicity:** A single database is easier to manage, back up, and restore than multiple databases.
- **Transactional Integrity:** It allows for atomic transactions across modules when necessary, which can be complex to achieve in a distributed system.
- **Logical Separation:** Using separate schemas for each module provides a clear boundary between the data of different modules. It prevents "back-door" access to a module's data and encourages communication through well-defined interfaces.
- **Evolvability:** If a module needs to be extracted into a separate microservice in the future, its data is already logically separated in its own schema, which makes the migration process much easier.

### Consequences

- There is still a risk of developers bypassing the rules and creating cross-schema queries, which would lead to tight coupling. This needs to be enforced through code reviews and conventions.
- The database is a single point of failure. However, by using a managed service like Azure Database for PostgreSQL with high availability features, this risk is mitigated.
- The database schema for the entire application needs to be managed and evolved together.

ADR-015: CI/CD Pipeline for the Modular Monolith
-------------------------------------------------

**Date**: 2025-09-30
**Status**: Accepted

### Context

With the adoption of a modular monolith architecture (ADR-012), our CI/CD strategy needs to be adapted. Unlike microservices, where each service can have its own independent pipeline, a monolith is deployed as a single unit. We need a unified pipeline that builds, tests, and deploys the entire application.

### Decision

We will adopt **GitHub Actions** to create a single, unified CI/CD pipeline for the entire modular monolith application (backend and front-end).

The workflow will run on GitHub-hosted runners and authenticate to Azure using OpenID Connect to avoid long-lived secrets. It will execute the following stages:

1. **Build & Unit Test:** On every commit, workflows build the backend and front-end, run unit tests, and perform static analysis (linting, SCA).
2. **Package & Publish:** Successful builds produce a Docker image, tag it with the commit SHA, and push it to **Azure Container Registry (ACR)**.
3. **Integration Test:** The pushed image is deployed to an ephemeral namespace in **AKS** using Helm; automated integration and k6 performance suites run against this environment.
4. **Deploy to Staging:** Upon passing tests, the workflow promotes the image to the staging AKS namespace via a gated GitHub Environment for stakeholder UAT.
5. **Deploy to Production:** After manual approval, the workflow performs a blue-green deployment on AKS by rolling out the new revision alongside the previous one and switching traffic once health checks pass.

### Alternatives

- **Multiple workflows:** Separate pipelines for backend and front-end. Rejected because coordinating releases of a single deployable would add overhead.
- **Azure Pipelines:** Native Azure DevOps service. Rejected to reduce tool sprawl and keep CI/CD within GitHub where the code is hosted.
- **Jenkins:** A self-hosted CI/CD server. Rejected due to higher operational burden for maintenance, scaling, and security patching.

### Rationale

- **Repository-native automation:** GitHub Actions keeps pipeline configuration, reusable workflows, and required checks alongside the codebase, improving traceability and change reviews.
- **Secure Azure integration:** Using GitHub's OIDC federation with Azure AD eliminates stored secrets while still allowing the workflow to push images to ACR and update AKS clusters.
- **Environment protections:** GitHub Environments provide built-in approvals, concurrency limits, and secrets scoping for staging and production promotions.
- **Continuous delivery:** Automating test, packaging, and promotion steps reduces manual effort and shortens lead time to release while keeping AKS rollouts consistent.

### Consequences

- A failure in any part of the workflow (e.g., a failing unit test) blocks deployment of the entire application, which enforces quality but can delay releases.
- Teams must maintain GitHub Actions workflows, runners, and caching strategies; complex jobs may require tuning or self-hosted runners for performance.
- Azure role assignments and GitHub environment rules must stay in sync; misconfiguration of identity federation or secrets could halt deployments.

ADR-016: Adopt Clean Architecture
----------------------------------

**Date**: 2025-10-04
**Status**: Accepted

### Context

Our modular monolith (ADR-012) with its Domain-Driven Design (DDD) approach (ADR-004) requires a software architecture that enforces a strong separation of concerns. This ensures that core business logic is independent of external frameworks and technologies like databases, UI, and APIs. A traditional layered architecture often leads to an "anemic domain model" where business logic leaks into UI or infrastructure layers, creating a tightly coupled system that is hard to test and maintain. We need an architecture that protects our domain model and allows it to evolve independently.

### Decision

We will adopt **Clean Architecture** as our primary architectural pattern. This pattern organizes the codebase into a series of concentric layers, each with its own responsibilities, governed by the **Dependency Rule**: source code dependencies can only point inwards.

The layers will be:

1. **Domain Layer (Innermost):** Contains enterprise-wide business rules, modeled as rich aggregates, entities, and value objects as per our DDD approach. This layer has zero dependencies on any other layer.
2. **Application Layer:** Contains application-specific business rules. It orchestrates the flow of data from the domain layer to the outer layers and defines interfaces for infrastructure concerns (e.g., repositories, email services) that are implemented by the outer layers.
3. **Infrastructure Layer:** Implements the interfaces defined in the Application layer. It contains all the details about the database (e.g., Entity Framework Core repositories), file system, and network calls to other systems.
4. **Presentation/API Layer (Outermost):** This layer is the entry point to the application. For our system, this will be the ASP.NET Core Web API controllers. It handles HTTP requests and responses.

To enforce these boundaries, all data crossing into or out of the application will be mapped through **Data Transfer Objects (DTOs)**. Controllers will receive DTOs, which are then mapped to commands or queries and processed by the Application layer. The Application layer, in turn, returns DTOs to the Presentation layer, ensuring the domain model is never exposed directly to the outside world.

### Alternatives

- **Traditional N-Tier Layered Architecture:** In this model, dependencies flow downwards from UI -> Business Logic -> Data Access. This often results in the business logic becoming dependent on data access concerns (e.g., ORM entities), making it difficult to test in isolation and coupling it to a specific database technology.
- **Anemic Domain Model:** This approach uses domain objects as simple data bags with no behavior. All business logic resides in service classes, which leads to procedural code and makes it hard to enforce business invariants. This was rejected as it contradicts our DDD decision (ADR-004).
- **No formal architecture (Ad-hoc):** Allowing developers to place logic wherever seems convenient. This leads to a "big ball of mud" that is impossible to maintain, test, or scale.

### Rationale

Adopting Clean Architecture provides several significant advantages over other approaches:

- **Testability:** The core business logic in the Domain and Application layers is completely independent of any UI, database, or external service. This makes it incredibly easy to write fast, reliable unit tests for the most critical parts of our system.
- **Independence from Frameworks:** The core business rules are not tied to ASP.NET Core, Entity Framework, or any other framework. This makes the application more resilient to technological changes and vendor lock-in.
- **Decoupling and Maintainability:** The strict dependency rule forces a clean separation of concerns. This makes the codebase easier to understand, navigate, and maintain. Changes in one layer (e.g., swapping the database from PostgreSQL to SQL Server) have minimal impact on other layers.
- **Alignment with DDD and Modular Monolith:** Clean Architecture is a natural fit for our existing decisions. The Domain layer is the perfect home for our DDD aggregates, and the clear boundaries help enforce the modularity of our monolith.

### Consequences

- **Increased Upfront Complexity:** The initial setup is more complex than a traditional layered architecture due to the strict separation and the need for dependency inversion.
- **Mapping Overhead:** We must write code to map data between layers (e.g., DTOs to Domain Entities). While tools like AutoMapper can help, this adds a layer of abstraction that needs to be maintained and tested.
- **Learning Curve:** Developers need to understand the principles of Clean Architecture and the Dependency Rule to work effectively within this structure.
