## 5. Shipping & Hazmat

### 5.1 Overview

This module manages the integration with shipping carriers and ensures
that outbound and return shipments comply with hazardous materials
(hazmat) regulations. It provides functionality for rate shopping, label
generation, tracking, address validation and compliance documentation. As
defined in the project's requirements, the system leverages a
multi-carrier shipping API to act as a bridge between the ERP and
various shipping providers. This automates tasks such as rate
calculation, label generation, and tracking updates, which are critical
for meeting the business objective of a sub-24-hour order fulfillment
SLA. For hazardous materials, carriers and shippers must comply with
federal regulations (DOT and IATA) and use carrier-approved shipping
solutions.

### 5.2 Functional capabilities

1. **Carrier rate shopping**
    - Integrate with multiple carriers (UPS, FedEx, USPS, DHL, local
        carriers). Use a multi‑carrier API to request `realtime` rates
        based on package dimensions, weight, origin, destination and
        service levels.
    - Compare rates and transit times; apply business rules (preferred
        carrier, lowest cost, fastest service) to select the best option
        for each shipment.
    - Provide shipping cost estimates during checkout.
2. **Label generation and documentation**
    - Automatically generate shipping labels (PDF, ZPL) containing
        sender, recipient, weight, dimensions and tracking information.
        This aligns with shipping APIs that automatically create orders
        and AWB IDs and generate labels.
    - Support printing of **hazardous materials documentation**
        (shippers’ declaration, material safety data sheets) and
        compliance labels for regulated products. Only authorised
        personnel can print hazmat documents.
    - Store label and documentation files for reference in the
        `Shipment` entity.
3. **Shipment Tracking and Notifications**
    - Subscribe to carrier webhooks or poll for status updates to update
        shipment status within **15 minutes** of a carrier scan event. Publish
        `shipping.shipment-delivered` events upon final delivery.
    - Provide tracking links for customers and send email/SMS
        notifications when shipments are created, in transit, or
        delivered.
    - Include support for non‑delivery reporting (NDR) and manage
        `redelivery` or return to sender.
4. **Address Validation and EDD (Estimated Delivery Date)**
    - Validate destination addresses using the integrated carrier's API
        at the time of rate request to avoid misdeliveries.
    - Calculate and display estimated delivery dates as provided by the
        carrier APIs during rate shopping.
5. **Hazardous materials compliance**
    - Flag shipments containing hazmat products. Before generating
        labels, verify that carriers support the hazardous
        classification.
    - Generate proper hazmat documentation (e.g. UN number, hazard
        class, packaging group) and include required markings and labels
        on packages.
    - Use carrier‑approved shipping software, as required by UPS and
        other carriers, and ensure shipments comply with **federal
        Hazardous Materials Regulations (HMR)** and **IATA Dangerous
        Goods Regulations**.
    - Maintain signed hazmat agreements for each carrier and verify
        expiration dates.
6. **Returns and RMA shipping**
    - Generate prepaid return labels; track return shipments and
        update return status in the Orders module.
    - Manage pickup scheduling for 3PL returns when necessary.

### 5.3 Data entities

- **Carrier** – defines shipping providers; includes services, API
    credentials, hazmat capabilities and transit time profiles.
- **Shipment** – links to Order, contains carrier, service level,
    package details (weight, dimensions), hazmat flag, tracking number,
    label file and status.
- **ShipmentRate** – temporary entity capturing rate quotes from
    carriers.
- **HazmatDocument** – file record storing hazardous goods
    declarations and supporting data (UN number, hazard class, packaging
    group).

### 5.4 Process flows

1. **Rate shopping and label purchase**
    1. When order is ready for shipping, system requests rate quotes
        from carriers via multi‑carrier API.
    2. System applies business rules to select carrier and service; user
        may override.
    3. System purchases label from chosen carrier; receives tracking
        number and label file.
    4. Shipment record is created with tracking information; label is
        sent to printer.
    5. `shipping.shipment-created` event is published.
2. **Hazmat shipment**
    1. If any line on the order is flagged as hazardous, system
        verifies hazmat agreement and obtains required packaging
        instructions and documentation.
    2. Hazmat documents are generated and printed along with the
        shipping label.
    3. Shipments are booked only with carriers that handle the specific
        hazard class.
3. **Tracking and notifications**
    1. Carrier sends status updates; system updates shipment status and
        notifies customers.
    2. If NDR occurs, system alerts CSR to take corrective action.

### 5.5 API and integration

- **Inter-module Communication:** This module communicates with other internal modules exclusively by publishing and subscribing to domain events on the **in-process event bus**. Direct method calls between modules are not permitted. Key events published include `shipping.shipment-created` and `shipping.shipment-delivered`.
- **Public API:** Functionality is exposed to external clients via the application's single, unified REST API, as defined in the `Api_specifications_and_endpoint_inventory.md` document.
- **External Integrations:** This module integrates with external systems (like multi-carrier shipping API providers and hazmat regulation services) via dedicated **adapters**. These adapters are responsible for calling external APIs.

### 5.6 Roles and permissions

- **Operations Manager (Full control):** Configure carrier accounts, set business rules for rate selection, and approve hazmat shipments.
- **Warehouse Associate (Create/Edit):** Request rates, print labels, and schedule pickups.
- **IT Administrator (Full control):** Full control for administrative purposes.
- **Read-Only Access:** The following roles have read-only access: Owner/GM, Purchasing Lead, CSR, E-Comm Mgr, B2B Account Mgr, Accountant, and Auditor.

### 5.7 Error handling

- **Carrier API failure:** If rate or label purchase fails, system
    retries; after multiple failures, escalates to Operations Manager
    and allows manual processing.
- **Address validation error:** Return 422 Unprocessable Entity with
    validation errors; user must correct address.
- **Hazmat non‑compliance:** If hazmat agreement is missing or product
    class is unsupported, system blocks shipment and logs error.

### 5.8 Performance and scalability

- **Concurrent shipments:** Support hundreds of shipments per hour;
    use asynchronous requests for rate shopping to avoid blocking user
    sessions.
- **Tracking Updates:** Carrier status updates (from webhooks or polling)
    must be processed with minimal latency to ensure customer-facing
    notifications are sent in a timely manner.
- **3PL Integration:** For shipments handled by 3PL providers, tracking information and inventory status must be updated within **30 minutes**.

### 5.9 Non‑functional considerations

- **Compliance:** Maintain up‑to‑date hazmat training and
    documentation; implement controls to ensure packaging and labelling
    compliance.
- **Security:** API credentials for carriers must be encrypted. Access
    to hazmat information is restricted.
- **Audit:** Log all label purchases, rate selections and hazmat
    document generations.
