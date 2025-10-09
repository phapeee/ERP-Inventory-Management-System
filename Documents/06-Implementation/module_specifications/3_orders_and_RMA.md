## 3. Orders & Return Merchandise Authorization (RMA)

### 3.1 Overview

The Orders & RMA module manages the entire lifecycle of customer orders
(B2B and B2C), from entry through fulfilment, invoicing and returns. It
must centralize orders from multiple channels (web store, Amazon, POS, B2B
portal) and ensure that fulfilment meets the organisation’s
`servicelevel` targets. The module also handles the reverse logistics
process—returns management—to streamline authorisation, inspection,
disposition and refund processing. Efficient returns management improves
customer satisfaction and reduces costs, while ensuring that returned
items are reintegrated into inventory where possible.

### 3.2 Functional capabilities

1. **Order capture and validation**
    - Accept orders via API or UI, capturing customer details,
        shipping addresses, items, quantities, and chosen shipping
        methods.
    - Validate inventory availability through the Inventory module;
        allocate stock immediately or place items on backorder if
        insufficient.
    - Calculate taxes and shipping costs using Tax & Shipping modules.
    - Perform **fraud screening** using integrated payment processor
        services. Orders flagged for fraud are placed on hold for manual review and approval by the Owner/GM before fulfillment.
2. **Order workflows**
    - **New → Allocated → Picked → Shipped → Invoiced → Closed**.
        Orders can also be **Cancelled** or **Returned**.
    - Support partial shipments and partial invoices.
    - Provide actions for Customer Service Representatives (CSR) to
        edit orders before shipment (e.g. change quantities, shipping
        address) within defined time limits.
    - Automatically generate **shipment records** with tracking
        numbers and integrate with the Shipping module.
3. **Payment processing**
    - Integrate with payment gateways to authorise and capture
        payments. Securely store tokens; do not retain full card data.
    - Handle `preauthorisation` at order placement and capture upon
        shipping. Support refunds and partial refunds.
4. **Returns and RMA management**
    - Implement an **RMA issuance workflow**: customer requests
        return, CSR validates reason and issues RMA number with
        expiration date.
    - Track return status: *Authorised*, *Received*, *Inspected*,
        *Completed*.
    - Record inspection results and determine disposition (restock,
        refurbish, scrap) as per requirements.
    - Generate refund or store credit depending on disposition; update
        financial records and inventory.
    - Provide reverse logistics labels via Shipping module; track
        return shipments and update status accordingly.

### 3.3 Data entities

- **Order** – header with customer info, status, total amounts,
    payment status, timestamps.
- **OrderLine** – product, quantity ordered, quantity allocated, unit
    price, tax and discount details.
- **Payment** – stores payment authorisations, captures and refunds;
    includes gateway transaction IDs, status and amount.
- **Shipment** – records pick and pack details, carrier, service
    level, tracking numbers and dates.
- **RMA** – header with customer, order reference, reason, status,
    issue date and expiration date.
- **RmaLine** – returned items, quantity, condition, disposition and
    refund amount.

### 3.4 Process flows

1. **Order placement**
    1. Order is submitted via web store or API. System performs data
        validation and calculates totals.
    2. Inventory module reserves stock; if not available, sets
        backorder flag.
    3. Payment is pre‑authorised. Order status becomes *Allocated* and
        events `orders.order-created` and `orders.order-paid` are published.
2. **Picking and shipping**
    1. Warehouse module creates pick lists; after items are picked,
        shipments are created and shipping labels generated.
    2. Order status moves to *Picked*; after shipping, tracking numbers
        are recorded and status changes to *Shipped*. `orders.order-shipped`
        event is published.
    3. Payment is captured; invoice is issued.
3. **Returns process**
    1. Customer requests a return through the portal or CSR. CSR issues
        an RMA number and instructs the customer to send items back.
    2. When returned goods arrive, they are inspected. Inspection
        results determine disposition: restock, refurbish or scrap.
    3. Inventory and financial adjustments are made; refund or credit
        is processed.
    4. `orders.return-completed` event is published.
4. **Cancel Order**
    1. User (CSR) initiates order cancellation via UI or API before the order is shipped.
    2. System validates that the order is in a cancellable state (e.g., not shipped).
    3. Order status is updated to *Cancelled*. The payment pre-authorisation is voided.
    4. System publishes an `orders.order-cancelled` event, allowing other modules (e.g., Inventory) to release reserved stock.

### 3.5 API and integration

- **Inter-module Communication:** This module communicates with other internal modules exclusively by publishing and subscribing to domain events on the **in-process event bus**. Direct method calls between modules are not permitted. Key events published include `orders.order-created`, `orders.order-shipped`, and `orders.return-completed`. It subscribes to events from other modules to drive its workflow, such as `warehouse.picking-completed` (to update order status to *Picked*) and `shipping.shipment-created` (to record tracking information).
- **Public API:** Functionality is exposed to external clients via the application's single, unified REST API, as defined in the `Api_specifications_and_endpoint_inventory.md` document.
- **External Integrations:** This module integrates with external systems (like payment gateways and shipping carriers) via dedicated **adapters**. These adapters are responsible for calling external APIs or for publishing/subscribing to integration events on an external message broker.

### 3.6 Roles and permissions

- **Owner/GM (Approve):** Approve high-value cancellations and refunds.
- **Operations Manager (Create/Edit):** Manage order fulfillment and edit order details pre-shipment.
- **CSR (Create/Edit):** Create orders, edit orders until shipped, issue RMAs, process returns.
- **B2B Account Manager (Create/Edit):** Create and manage orders for B2B clients.
- **IT Administrator (Full control):** Full control for administrative purposes.
- **Read-Only Access:** The following roles have read-only access: Purchasing Lead, Warehouse Associate, E-Comm Mgr, Accountant, and Auditor.

### 3.7 Error handling

- **Payment failure:** If payment pre‑authorisation fails, order is
    not created; return error and ask user to retry.
- **Allocation failure:** If inventory cannot be reserved, system
    places items on backorder and notifies Purchasing.
- **Invalid RMA:** If return is attempted without a valid RMA or after
    RMA expiration, system rejects return.

### 3.8 Performance and scalability

- **Concurrent orders:** Support at least 200 concurrent users and
    order placements, meeting the SLA defined in non‑functional
    requirements.
- **Order volume:** System must handle spikes (e.g. seasonal sales) of
    thousands of orders per hour; order processing tasks should run
    asynchronously where possible.
- **Shipping SLA:** The system will track the time from order creation to shipment and provide data to the Analytics module to report on the 24-hour shipping SLA.

### 3.9 Non‑functional considerations

- **Security:** All payment interactions must comply with PCI DSS.
    Sensitive data (cards, personal info) should be tokenised.
- **Audit:** Record all status changes and user actions; include
    `OrderHistory` and `RmaHistory` tables.
- **Customer experience:** Provide order tracking and status
    notifications to customers via email/SMS.
