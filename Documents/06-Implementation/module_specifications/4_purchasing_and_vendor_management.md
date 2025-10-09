## 4. Purchasing & Vendor Management

### 4.1 Overview

This module automates the procurement process—from requisition through
purchase order issuance, vendor communication and receipt of goods. A
robust purchasing system helps reduce manual data entry, track spending
and maintain supplier relationships. This module also maintains vendor scorecards, handles
minimum order quantities and lead times and integrates with inventory
and accounts payable.

### 4.2 Functional capabilities

1. **Purchase order management**
    - Generate POs from requisitions or manually. POs include line
        items, unit costs, quantities, expected delivery dates and
        shipping terms.
    - Support `dropship` orders directly to customers.
    - Provide PO status tracking (Sent, Acknowledged, In Transit,
        Partially Received, Closed).
    - Include approval workflows for POs exceeding thresholds or
        requiring additional sign‑off.
2. **Vendor management and scorecards**
    - Maintain **Vendor** records (contact info, payment terms, lead
        times, ratings).
    - Track vendor performance through `ontime` delivery, quality, fill
        rate and cost metrics; compute scorecards.
    - Support vendor onboarding with required documentation (W‑9,
        hazmat handling agreements).
3. **Receiving and matching**
    - Integrate with Inventory module to match received quantities
        with PO lines. Support partial receipts and backorders.
    - Provide `threeway` matching for accounts payable: PO, receiving
        and vendor invoice.
4. **Spend analysis and forecasting**
    - Analyse purchasing spend by category, supplier and time period.
        Provide dashboards and alerts for spend overages.
    - Support forecasting of demand and reorder points to plan
        procurement activities.

### 4.3 Data entities

- **PurchaseOrder** – header with supplier, shipping terms, payment
    terms, status, total amounts; lines with product, unit cost,
    quantity, delivery date and cost centre.
- **Vendor** – record of supplier with address, contact, payment
    terms, MOQs and lead times; includes performance metrics fields.

### 4.4 Process flows

1. **Automated PO Creation**
    1. The Inventory module detects that a product's stock has fallen below its reorder point and publishes an `inventory.reorder-point-breached` event.
    2. The Purchasing module subscribes to this event, and a draft PO is automatically generated.
    3. The draft PO is routed for approval based on configured workflows (e.g., value thresholds).
    4. After approval, a final PO is generated and the system publishes a `purchasing.purchase-order-created` event.
    5. The PO is sent to the vendor via EDI or email; the vendor acknowledges receipt.
2. **Receiving and invoicing**
    1. Upon shipment arrival, warehouse records receipt. Received
        quantity and quality are matched to PO lines.
    2. Accounts payable matches vendor invoice to PO and receipt; any
        discrepancy triggers exception workflow.

### 4.5 API and integration

- **Inter-module Communication:** This module communicates with other internal modules exclusively by publishing and subscribing to domain events on the **in-process event bus**. Direct method calls between modules are not permitted. Key events published include `purchasing.purchase-order-created` and `purchasing.purchase-order-approved`. It subscribes to events like `inventory.reorder-point-breached` to trigger the automated reordering workflow.
- **Public API:** Functionality is exposed to external clients via the application's single, unified REST API, as defined in the `Api_specifications_and_endpoint_inventory.md` document.
- **External Integrations:** This module integrates with external systems (like vendor EDI platforms or AP systems) via dedicated **adapters**. These adapters are responsible for calling external APIs or for publishing/subscribing to integration events on an external message broker.

### 4.6 Roles and permissions

- **Owner/GM (Approve):** Approve high-value purchase orders.
- **Purchasing Lead (Approve):** Manage vendors, issue POs, send RFQs, evaluate responses, and approve POs within their authority.
- **IT Administrator (Full control):** Full control for administrative purposes.
- **Read-Only Access:** The following roles have read-only access: Operations Manager, CSR, E-Comm Mgr, Accountant, and Auditor.
- **No Access:** The following roles have no access: Warehouse Associate, B2B Account Manager.

### 4.7 Error handling

- **Budget exceed:** If requested amount exceeds department budget,
    system rejects requisition and notifies requester.
- **Over‑received goods:** If received quantity exceeds PO quantity,
    system flags a discrepancy and requires approval.
- **Vendor non‑response:** If RFQs receive no response by due date,
    system escalates to Purchasing Lead.

### 4.8 Performance and scalability

- Must handle hundreds of active POs and requisitions simultaneously.
    Use async workflows for RFQ and PO communications.
- Provide near‑`realtime` updates of vendor performance metrics to
    support decision making.

### 4.9 Non‑functional considerations

- **Compliance:** Enforce segregation of duties (requesters cannot
    approve their own requisitions). Retain audit logs of approvals and
    changes.
- **Vendor agreements:** For hazardous materials, require valid hazmat
    agreements and automatically verify before issuing POs.
- **Security:** Limit vendor payment data access to authorised roles;
    encrypt sensitive information.
