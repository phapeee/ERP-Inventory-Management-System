Integration Contracts & Service Level Agreements (SLAs)
=======================================================

Purpose & Scope
---------------

This document defines the contracts for integrating the PineCone Pro ERP/IMS platform with external systems. As the ERP/IMS is a **modular monolith** (ADR-012), it provides a single, unified integration surface for all external partners.

This document outlines two primary integration patterns:

1. **Inbound Integrations:** External systems interacting with the ERP by consuming its public **REST API**. All inbound API calls are governed by the contracts in `api_specifications_and_endpoint_inventory.md`.
2. **Outbound Integrations:** The ERP interacting with external systems, either by calling their APIs or by publishing **integration events** to **Azure Service Bus** (ADR-008).

This document specifies the responsibilities, protocols, data formats, and Service Level Agreements (SLAs) for the following key external integrations:

- **Web Store (E-commerce & POS)**
- **Third-Party Logistics (3PL) Provider**
- **Accounting System**
- **Sales Tax Service**

General Requirements
--------------------

1. **Communication Patterns:**
    - **Synchronous:** All synchronous communication is handled via RESTful APIs over HTTPS, using JSON payloads.
    - **Asynchronous:** Event-driven interactions use **integration events** published to Azure Service Bus topics. These are distinct from the internal *domain events* used for inter-module communication.

2. **Authentication:**
    - **Inbound API:** All requests to the ERP's API require an OAuth 2.0 bearer token issued by Microsoft Entra ID.
    - **Outbound API Calls:** The ERP will use client credentials or API keys provided by the external partner, stored securely in Azure Key Vault.

3. **Data Format & Versioning:** JSON is the standard data format. The ERP's public API is versioned in the URI (e.g., `/api/v1/...`). External API integrations will adhere to the versioning scheme of the partner.

4. **Security:** All communication must use TLS 1.2+. Sensitive data in payloads must be handled according to security best practices.

5. **Monitoring & Resilience:** Integrations are monitored via Azure Monitor. Outbound API calls from the ERP include retry logic with exponential backoff to handle transient failures.

Integration 1 – Web Store (E-commerce & POS)
--------------------------------------------

### Scope

This integration enables the web store to access product information, process orders, and receive status updates from the ERP.

### Interfaces and Protocols

#### **Inbound (Web Store → ERP)**

The web store acts as a client to the ERP's public REST API.

| Interaction          | Method & Path                 | Notes                                              |
| -------------------- | ----------------------------- | -------------------------------------------------- |
| Get Product Catalog  | `GET /api/v1/products`        | Fetches product details for display.               |
| Get Inventory Levels | `GET /api/v1/inventory/items` | Checks available-to-promise stock before checkout. |
| Place Order          | `POST /api/v1/orders`         | Submits a new customer order to the ERP.           |
| Initiate Return      | `POST /api/v1/returns`        | Creates a Return Merchandise Authorization (RMA).  |

#### **Outbound (ERP → Web Store)**

The ERP publishes integration events to an Azure Service Bus topic, which the web store subscribes to.

| Interaction          | Channel           | Event Schema         | Notes                                                                    |
| -------------------- | ----------------- | -------------------- | ------------------------------------------------------------------------ |
| Product Updates      | Azure Service Bus | `ProductUpdated`     | Notifies the web store of changes to product details or price.           |
| Inventory Updates    | Azure Service Bus | `InventoryAdjusted`  | Provides updates on stock levels.                                        |
| Order Status Updates | Azure Service Bus | `OrderStatusChanged` | Informs the customer of order progress (e.g., Paid, Shipped, Delivered). |

### Service Level Targets

- **ERP API Uptime:** ≥ 99.9%.
- **ERP API Latency:** < 2 seconds (95th percentile) for all endpoints consumed by the web store.
- **Event Latency:** All integration events must be published within **5 minutes** of the corresponding business action. This is part of the end-to-end 15-minute product data propagation SLA.

Integration 2 – Third-Party Logistics (3PL) Provider
----------------------------------------------------

### Scope

This integration manages order fulfillment, receiving, and inventory synchronization with an external 3PL provider.

### Interfaces and Protocols

#### **Outbound (ERP → 3PL)**

The ERP acts as a client, calling the 3PL's external API to send fulfillment requests and advance shipment notices (ASNs).

| Interaction              | Method & Path (3PL's API)  | Notes                                                     |
| ------------------------ | -------------------------- | --------------------------------------------------------- |
| Send Fulfillment Request | `POST /fulfillment-orders` | Sends an order to the 3PL for picking and packing.        |
| Send Inbound ASN         | `POST /receiving/asns`     | Notifies the 3PL of an incoming shipment from a supplier. |

#### **Inbound (3PL → ERP)**

The 3PL system calls the ERP's public API to provide updates on shipments, receiving, and inventory.

| Interaction      | Method & Path (ERP's API)                           | Notes                                                                                                       |
| ---------------- | --------------------------------------------------- | ----------------------------------------------------------------------------------------------------------- |
| Confirm Shipment | `POST /api/v1/shipping/labels` (or similar)         | Confirms an order has shipped and provides tracking info. This triggers an `OrderShipped` event internally. |
| Confirm Receipt  | `POST /api/v1/inventory/receipts`                   | Confirms goods from a PO have been received at the warehouse.                                               |
| Adjust Inventory | `POST /api/v1/inventory/adjustments` (hypothetical) | Reports adjustments from cycle counts or damages. The API should support this.                              |

### Service Level Targets

- **3PL API Uptime:** ≥ 99.8%.
- **Fulfillment Speed:** ≥ 98% of orders received by 12 PM must be shipped the same day.
- **Data Accuracy:** Pick/pack accuracy must be ≥ 99.9%. Inventory data accuracy must be ≥ 98%.

Integration 3 – Accounting System
---------------------------------

### Scope

This integration ensures financial data is synchronized between the ERP and an external accounting system.

### Interfaces and Protocols

This is primarily an outbound integration where the ERP calls the accounting system's API.

#### **Outbound (ERP → Accounting System)**

| Interaction         | Method & Path (Accounting API)    | Notes                                                     |
| ------------------- | --------------------------------- | --------------------------------------------------------- |
| Post Journal Entry  | `POST /journal-entries`           | Posts entries for sales, COGS, and inventory adjustments. |
| Create Invoice/Bill | `POST /invoices` or `POST /bills` | Creates customer invoices and vendor bills.               |

#### **Inbound (Accounting System → ERP)**

| Interaction        | Method & Path (ERP's API)                        | Notes                              |
| ------------------ | ------------------------------------------------ | ---------------------------------- |
| Reconcile Payments | `POST /api/v1/payments/reconcile` (hypothetical) | Updates payment status in the ERP. |

### Service Level Targets

- **Accounting API Uptime:** ≥ 99.9%.
- **Data Consistency:** Financial data must be 100% accurate. Failed postings must be retried and logged for manual intervention if necessary.
- **Sync Window:** Synchronization should occur at least hourly.

Integration 4 – Sales Tax Service
---------------------------------

### Scope

This integration provides real-time sales tax calculation by calling an external, specialized tax service.

### Interfaces and Protocols

This is an outbound integration where the ERP calls the tax service's API.

#### **Outbound (ERP → Tax Service)**

| Interaction        | Method & Path (Tax Service API) | Notes                                          |
| ------------------ | ------------------------------- | ---------------------------------------------- |
| Calculate Tax      | `POST /tax/calculate`           | Calculates tax for a given order and address.  |
| Commit Transaction | `POST /tax/commit`              | Commits a completed transaction for reporting. |

### Service Level Targets

- **Tax Service API Uptime:** ≥ 99.99%.
- **Tax Service API Latency:** < 100ms (95th percentile).
- **Accuracy:** Calculations must be backed by an accuracy guarantee from the provider.
- **Resilience:** The ERP must have a fallback mechanism (e.g., cached rates) if the service is unavailable.

Integration 5 – Multi-Carrier Shipping API
------------------------------------------

### Scope

This integration provides real-time rate shopping, shipping label generation, address validation, and shipment tracking by connecting to a specialized multi-carrier API provider.

### Interfaces and Protocols

This is primarily an outbound integration where the ERP calls the provider's REST API. The provider may also call back to the ERP via webhooks to provide asynchronous status updates.

#### **Outbound (ERP → Shipping API)**

| Interaction         | Method & Path (Shipping API) | Notes                                                               |
| ------------------- | ---------------------------- | ------------------------------------------------------------------- |
| Get Shipping Rates  | `POST /rates`                | Fetches real-time rates from multiple carriers based on package details. |
| Create Shipment/Label | `POST /shipments`            | Purchases a label and generates shipment documents.                 |
| Validate Address    | `POST /addresses/validate`   | Confirms the validity of a shipping destination address.            |
| Track Shipment      | `GET /track/{tracking_id}`   | Polls for the latest shipment status (if webhooks are not used).    |

#### **Inbound (Shipping API → ERP)**

| Interaction      | Method & Path (ERP's API)         | Notes                                                              |
| ---------------- | --------------------------------- | ------------------------------------------------------------------ |
| Status Webhook   | `POST /api/v1/shipping/webhooks`  | Receives asynchronous updates on shipment status from the provider. |

### Service Level Targets

- **Shipping API Uptime:** ≥ 99.9%.
- **API Latency:** < 1 second (95th percentile) for rate requests and label generation.
- **Accuracy:** Label and hazmat documentation generation must be 100% compliant with carrier standards.
- **Resilience:** The ERP must implement retry logic for transient API failures and provide a manual fallback process if the integration is down.

Conclusion
----------

This document outlines the contracts for integrating the PineCone Pro ERP/IMS with key external systems. By defining clear boundaries, responsibilities, and communication patterns, the architecture ensures that integrations are reliable, scalable, and decoupled from the core business logic of the modular monolith.
