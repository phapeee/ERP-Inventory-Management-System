Security Requirements & Threat Model
====================================

Purpose and Scope
-----------------

This document defines the security requirements and threat model for
PineCone Pro Supplies’ Enterprise Resource Planning (ERP) / Inventory
Management System (IMS). It covers the end‑to‑end solution from the
Angular‑based user interface through the ASP.NET modular monolith,
running on Azure Kubernetes Service (AKS), and
the data stores that support procurement, inventory management, order
fulfilment, returns handling and financial reporting.

The goals are to:

- Protect sensitive data (e.g., customer information, financial
    records, hazardous‑material classifications) throughout its
    lifecycle.
- Ensure only authorised users and services can access system
    resources.
- Maintain the integrity and availability of the ERP/IMS under normal
    and adverse conditions.
- Comply with relevant regulations (e.g., tax laws, data privacy,
    hazardous‑materials handling, PCI DSS) and support auditing and
    forensic investigations.

Security Objectives and Principles
----------------------------------

The system’s security posture is built on the following principles:

1. **Confidentiality and least privilege** – access to data and
    functionality must be restricted to those who need it to perform
    their job. The ERP security best practices article stresses that
    robust access management and the principle of least privilege
    mitigate insider threats.
2. **Integrity** – prevent unauthorised modification of data and code.
    Tampering risks undermine data accuracy and can lead to incorrect
    orders, payments or tax filings.
3. **Availability and resilience** – the system must remain available
    to authorised users while withstanding denial‑of‑service (DoS)
    attacks and hardware failures. A resilient network architecture and
    disaster‑recovery plan provide barriers against cyber threats.
4. **Auditability and accountability** – record all relevant events to
    allow reconstruction of actions. Comprehensive logging and
    monitoring enable prompt detection of anomalies and support
    compliance.
5. **Defense in depth** – security controls must exist at multiple
    layers (application, network, platform, human) to prevent single
    points of failure.
6. **Secure by design and continuous improvement** – embed security
    considerations early through threat modelling and update the model
    and controls as the system evolves.

System Overview and Trust Boundaries
------------------------------------

The ERP/IMS uses a modular monolith architecture. Users interact with an
Angular single‑page application served via an API gateway. The backend is a single ASP.NET Core
application, exposing REST/GraphQL APIs and
subscribing to asynchronous events. Core data is stored in a single Azure Database for PostgreSQL
server, with separate schemas for each module. External integrations include
payment gateways, shipping carriers, vendor EDI and a tax calculation
service.

Major trust boundaries include:

- **User devices** → **Frontend (Angular)** – authentication, session
    management and cross‑site scripting (XSS) prevention.
- **Frontend → API Gateway** – TLS encryption, token propagation and
    rate limiting.
- **API Gateway → Application** – authentication and authorization.
- **Application → Database** – network restrictions and
    encryption at rest for the database.
- **Application → Third‑party services** – secure integration over
    HTTPS using API keys or OAuth tokens.

The AKS cluster is deployed in a private virtual network with no public IPs.
Secrets are injected via Azure Key Vault.

Security Requirements
---------------------

### 1 Authentication and Access Control

1. **Role‑based access control (RBAC)** – assign users to roles with
    the minimum privileges required. Regularly review and adjust
    permissions to reflect job changes. Leverage the existing RBAC
    matrix for functional roles (e.g., purchasing lead, warehouse
    associate) and enforce separation of duties (SoD) for sensitive
    actions (e.g., entering and approving purchase orders).
2. **Multi‑factor authentication (MFA)** – require MFA for all user
    accounts, using methods such as TOTP (e.g., Google Authenticator) or FIDO2/WebAuthn. The ERP security guide notes that MFA adds an additional
    layer of protection against credential theft and phishing.
3. **Single sign‑on and identity federation** – integrate with
    Microsoft Entra ID to centralise identity management. Use managed
    identities for the application to access Azure resources.
4. **Session management** – enforce secure session cookies with `HttpOnly`, `Secure`, and `SameSite=Strict` attributes, and an idle
    timeout of 15 minutes of inactivity. Prevent
    session fixation and cross‑site request forgery (CSRF) attacks with
    appropriate tokens.
5. **Inter-module communication** – Communication between modules is done via in-process calls. Access control between modules should be enforced using code-level permissions.

### 2 Data Confidentiality and Encryption

1. **Encryption at rest and in transit** – data stored on Azure Managed
    Disks is automatically encrypted at rest using Azure Disk Encryption with customer-managed keys. Sensitive data (customer
    details, payment tokens, hazardous‑material classifications) must be
    encrypted in the database (using Transparent Data Encryption - TDE) and caches. Use TLS 1.3 for all
    communications, including external
    integrations.
2. **Secret management** – store credentials, keys and connection
    strings in Azure Key Vault with Managed Identities for Azure resources. Secrets are injected into the application at runtime.
3. **Data classification and masking** – classify data according to
    sensitivity (public, internal, confidential, restricted). Apply
    masking or tokenisation for restricted data in logs or lower
    environments. Limit query access to personally identifiable
    information (PII).

### 3 Network Security and Segmentation

1. **Private virtual network** – deploy the AKS cluster within a
    private subnet without public IP addresses; restrict SSH access. Use
    Azure VPN or ExpressRoute for on‑premises connectivity.
2. **Network security groups (NSGs) and firewalls** – define inbound
    and outbound rules at subnet level to allow only required ports.
3. **API gateway** – terminate TLS and enforce
    rate limiting, IP whitelisting, and WAF (Web Application Firewall)
    rules at the edge using Azure Application Gateway with WAF. For internet‑facing services, use a WAF to
    protect against OWASP Top 10 attacks.

### 4 Application and Code Security

1. **Secure software development lifecycle (SSDLC)** – follow secure
    coding practices, including input validation, output encoding, and
    parameterised queries to prevent SQL injection and XSS. Use static
    and dynamic application security testing (SAST/DAST) tools like SonarQube and OWASP ZAP, and automated
    dependency scanning.
2. **API security** – implement strong authentication for API calls
    (OAuth2, JWT). Use schema validation, rate limiting, and
    object‑level access checks. Document APIs and ensure they do not
    expose sensitive fields by default.
3. **Patch management** – apply security patches promptly using a tool like Dependabot or Snyk to automatically create pull requests for vulnerable dependencies. ERP security
    guidance warns that delaying patches increases the risk of
    exploitation. Automate image rebuilds and rolling updates within AKS.
4. **Configuration management** – follow vendor recommendations for
    secure configuration. Disable unused services and enforce strong
    password policies.

### 5 Container Security

1. **Least privilege for containers** – avoid running containers as
    root.
2. **Image scanning and provenance** – scan container images for
    vulnerabilities using Azure Defender for Containers or Trivy and sign images to verify provenance before
    deployment. Use Azure Container Registry Content Trust or Notary.
3. **Runtime protection** – enable Microsoft Defender for Containers or
    a tool like Falco to detect and restrict attacks on the container.
4. **Host security** – The AKS control plane is managed by Azure, while node pools are secured through baseline hardening and Azure Policy.

### 6 Logging, Monitoring and Auditing

1. **Comprehensive logging** – capture authentication attempts,
    privilege escalations, configuration changes, data access events and
    administrative actions. ERP best practices highlight the value of
    detailed logs for detecting anomalies.
2. **Centralised log management** – forward logs to Azure Monitor and
    Application Insights. Integrate with a Security Information and
    Event Management (SIEM) system to correlate events and generate
    alerts.
3. **Audit trails and change logs** – maintain immutable audit records
    for compliance. The regulatory compliance guide notes that ERP
    systems should provide audit trails and change logs as part of their
    compliance features.
4. **Real‑time monitoring and alerting** – implement dashboards and
    alerts for KPIs and security events. Monitor resource utilisation to
    detect DoS attacks or misbehaving processes.

### 7 Compliance and Regulatory Controls

1. **Data protection regulations** – adhere to GDPR, CCPA and other
    privacy laws by implementing data subject rights (e.g., right to be
    forgotten) and cross‑border transfer safeguards. The compliance
    article stresses the importance of data privacy and security
    obligations.
2. **Financial and tax compliance** – ensure accurate financial
    reporting (SOX, GAAP) by enforcing segregation of duties, logging
    financial transactions and maintaining immutable ledgers. Automate
    tax calculations and record county‑level tax data as required by the
    primary feature list.
3. **Industry regulations** – support hazardous‑material reporting,
    chemical lot tracking, and vendor compliance as needed for regulated
    products. Provide robust audit trails for shipments and returns.
4. **PCI DSS and payment security** – store payment tokens securely,
    avoid storing full card numbers and use tokenisation with a
    PCI‑compliant payment gateway. Implement quarterly vulnerability
    scans and annual penetration tests.

### 8 Backup, Disaster Recovery and Business Continuity

1. **Regular backups and off‑site storage** – schedule automated
    backups of databases and application state. ERP best practices
    recommend storing backups in secure offsite locations or cloud
    services with encryption.
2. **Disaster recovery plan** – maintain a disaster‑recovery runbook
    with recovery time objectives (RTO) and recovery point objectives
    (RPO). Regularly test failover to secondary regions.
3. **Resilient deployment** – deploy the application to multiple instances; use autoscaling to
    handle load spikes.

### 9 People and Process Controls

1. **Security awareness training** – educate employees on phishing,
    social engineering, password hygiene and incident reporting. Ongoing
    training fosters a security culture and reduces human errors.
2. **Incident response** – establish a response team, define procedures
    for detection, containment, eradication and recovery, and conduct
    regular drills.
3. **Offboarding protocols** – promptly revoke departing employees’
    access to the ERP system to mitigate insider threats.
4. **Third‑party risk management** – assess the security posture of
    vendors (e.g., payment processors, shipping providers). Use
    contracts and security questionnaires to ensure they meet equivalent
    security standards.

Threat Model
------------

### Methodology

We adopt the **STRIDE** threat modeling framework. STRIDE was created by
Microsoft to classify threats into six categories: Spoofing, Tampering,
Repudiation, Information Disclosure, Denial of Service and Elevation of
Privilege. It uses data flow diagrams to systematically identify threats
and encourages proactive mitigation. Each category maps to a violation
of a core security property (e.g., spoofing violates authentication).

### Assets

- **User identities and credentials** – employee accounts, customer
    login details, API keys.
- **Financial data** – invoices, payments, purchase orders, tax
    records.
- **Inventory data** – product SKUs, stock levels, lot/serial numbers,
    hazardous‑material classifications.
- **Business process data** – order statuses, vendor performance
    metrics, RMA dispositions.
- **System infrastructure** – container images, secrets, configuration
    files, underlying OS and network.

### Entry Points and Attack Surfaces

- Public web interface (Angular app) and REST APIs.
- API gateway and load balancers.
- In-process event bus.
- Database connections and data stores.
- DevOps pipelines (CI/CD), container registry and version control
    system.
- Third‑party APIs (payment processors, shipping carriers, vendor
    EDI).

### Threat Analysis and Mitigations

| STRIDE Category | Potential Threats | Mitigations |
|---|---|---|
| Spoofing (Authentication) | • Attackers use stolen credentials or bypass authentication to impersonate users.• Man‑in‑the‑middle (MITM) attacks on API calls. | • Enforce MFA and strong password policies.• Use TLS for all communications and validate certificates.• Employ token‑based authentication (OAuth2/JWT) and short‑lived access tokens.• Monitor failed login attempts and rate‑limit authentication endpoints. |
| Tampering (Integrity) | • Injection attacks (SQL/NoSQL injection) alter queries or commands.• Malicious modification of data in transit or at rest.• Unauthorized image modification in container registry. | • Use parameterised queries and input validation.• Implement integrity checks (hashes, digital signatures) on critical files and images.• Enforce RBAC on container registries; sign images and validate before deployment.• Encrypt data at rest (disks, databases) and in transit. |
| Repudiation (Accountability) | • Users deny performing actions, or logs are tampered with.• Inadequate logging leads to incomplete forensic evidence. | • Maintain immutable audit logs with timestamped records.• Digitally sign critical transactions (e.g., approval of purchase orders).• Store logs in a secure, write‑once medium (e.g., append‑only storage).• Integrate logs with SIEM for correlation and alerting. |
| Information Disclosure (Confidentiality) | • Data breaches expose PII, hazardous‑material classifications or financial data.• Unencrypted traffic or misconfigured backups leak secrets.• Excessive permissions allow employees to view sensitive records. | • Encrypt sensitive data at rest and in transit.• Implement field‑level encryption or masking in the data model.• Use least‑privilege access controls and role segregation.• Apply network segmentation and firewall rules.• Secure backups and offsite storage. |
| Denial of Service (Availability) | • Distributed denial‑of‑service (DDoS) overwhelms public endpoints.• Resource exhaustion attacks on database connections or message queues.• Application errors cause cascading failures. | • Use Azure DDoS Protection and Web Application Firewall.• Implement autoscaling and resource limits on the application.• Apply rate limiting at the API gateway.• Use circuit breakers for external integrations.• Continuously monitor health and resource utilisation; alert on anomalies. |
| Elevation of Privilege (Authorisation) | • Attackers exploit flaws to gain higher privileges or run arbitrary commands.• A vulnerability in one module can be used to compromise other modules. | • Enforce RBAC and SoD controls; restrict high‑risk actions to specific roles.• Harden the application and its dependencies.• Apply SAST/DAST scanning to detect vulnerabilities enabling privilege escalation. |

### Additional Threat Considerations

- **Third‑party integrations** – insecure APIs from vendors or payment
    providers may lead to data leakage or transaction tampering. Require
    vendors to use TLS and strong authentication; perform periodic
    security assessments.
- **Insider threats** – disgruntled employees could misuse their
    access. Mitigate via least‑privilege, real‑time monitoring, SoD and
    offboarding protocols.
- **Supply chain attacks** – malicious code or dependencies inserted
    into containers or dependencies. Mitigate by using signed images,
    verifying software supply chain provenance, and scanning
    dependencies.
- **Physical security** – ensure physical security of Azure
    datacentres is handled by the provider; for on‑premises components
    (e.g., local cross‑dock warehouses), implement surveillance, access
    control and environmental controls.

### Risk Prioritisation and Continuous Improvement

Assess each threat’s likelihood and impact. Use frameworks like FAIR to
assign risk scores and prioritise mitigations. The STRIDE process should
be revisited whenever new features, integrations or architecture changes
are introduced. Combine design‑time threat modelling with runtime
monitoring to detect zero‑day exploits.

Conclusion
----------

Implementing these security requirements and maintaining a living threat
model enables PineCone Pro Supplies to protect critical data, maintain
compliance and deliver reliable services. By applying defense‑in‑depth
controls—ranging from robust identity management and encryption through
network segmentation, secure coding, container hardening, logging, and
user training—the organisation can mitigate the risks associated with
running a complex ERP/IMS. Continual threat modelling and security
reviews ensure the architecture adapts to evolving threats and
regulatory requirements.
