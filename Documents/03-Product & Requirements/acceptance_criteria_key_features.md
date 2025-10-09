Acceptance Criteria for Key Features of ERP/IMS
===============================================

**Document Version:** 1.0  
**Prepared By:** \[Your Name\]  
**Date:** 28 September 2025

1 Purpose and Scope
-------------------

This document defines the acceptance criteria for the key features of
PineCone Pro Supplies’ ERP/Inventory Management System (ERP/IMS).
Acceptance criteria specify the conditions that must be met for a
feature to be considered complete and ready for deployment. They are
clear, testable statements based on the requirements captured in the
product specification. These criteria apply to the Minimum Viable
Product (MVP) scope and the Phase 2 enhancements.

2 Approach to Acceptance Criteria
---------------------------------

Each feature’s acceptance criteria are derived from functional
requirements and industry best practices. Criteria are written in the
format “Given/When/Then” where appropriate and focus on measurable
outcomes such as data integrity, performance, user interaction and
integration completeness. Where quantitative targets exist (e.g., 98 %
inventory accuracy), those values are included to enable objective
testing. The criteria ensure the system supports business objectives
like eliminating oversells, meeting service‑level agreements (SLAs) and
ensuring regulatory compliance.

3 Acceptance Criteria by Feature
--------------------------------

### 3.1 Product Information Management (PIM)

The PIM module provides a single source of truth for SKU data across all
channels. Acceptance criteria ensure accuracy, consistency and timely
propagation of product information:

- **Unique and complete records:** Each SKU must have a unique
    identifier and all required attributes (name, description, category,
    unit of measure, hazard class, expiry period, price). Validation
    errors occur if required fields are missing.
- **Hazardous and expiry data:** Hazard classifications and expiry
    periods must be recorded and exportable for compliance with shipping
    regulations.
- **Kit and bundle support:** The system must allow creation and
    maintenance of kits/bundles with dynamic component quantities and
    versioned bills of material (BOM). When a kit BOM is updated, the
    change must take effect for new orders while maintaining historical
    BOMs for past orders.
- **Unit conversions:** The system must automatically convert units
    (e.g., quart to gallon) during order processing and pricing. Test by
    placing orders in different units and verifying correct conversion
    and pricing.
- **Channel synchronisation:** Product updates must be propagated to
    all sales channels (web store, Amazon, POS, B2B portal) within
    **15 minutes**. Test by modifying a product and verifying that
    changes appear across channels within the time window.
- **Audit trail and data governance:** All product modifications must
    be logged with user, timestamp and change details. Only authorised
    roles may create or edit products, as defined in the RBAC matrix.
- **Product search and filtering:**
    - Given a user searches for a product by a partial SKU, the system must return all matching products.
    - Given a user searches by a keyword in the product name or description, the system must return relevant results.
    - Given a set of search results, a user must be able to filter by category and brand.
    - Given a set of search results, a user must be able to sort by product name and price.

### 3.2 Inventory & Warehouse Management

This module ensures real‑time inventory visibility and accurate stock
levels across multiple locations. Acceptance criteria focus on accuracy,
timeliness and proper handling of lot/serial controls:

- **Inventory accuracy:** Cycle count variance must be **≤ 2 %**,
    yielding overall inventory accuracy ≥ 98 %. Cycle counts should
    detect and correct discrepancies automatically.
- **Timely updates:** Inventory transactions performed via RF scanning
    (receiving, put‑away, picking, cycle count, returns) must update
    inventory records within **1 minute**.
- **Lot and serial capture:** Lot numbers, manufacture dates and
    expiry dates must be recorded at receiving for regulated products;
    serial numbers must be captured for high‑value tools and available
    during picking and RMA.
- **Cross‑dock/3PL feeds:** Transfers through cross‑dock facilities or
    third‑party logistics (3PL) providers must update inventory within
    **15 minutes** of receipt.
- **ATP recalculation:** Available‑to‑promise (ATP) quantities must be
    recalculated upon order entry to reflect on‑hand, reserved and
    incoming stock. Test by placing orders and verifying correct ATP
    values.

### 3.3 Order Management & Payments/Fraud

Order management orchestrates sales across channels, processes payments
and handles fraud screening.

- **Unified order queue:** Orders from all channels (web, B2B portal,
    POS, Amazon) must appear in a single order queue. Test by submitting
    orders from each channel and verifying central visibility.
- **Payment processing:** Payments must be authorised or captured at
    order entry and recorded in the accounting integration. Failed
    payments must trigger appropriate error messages and statuses.
- **Fraud screening:** Fraud check results must be recorded; orders
    flagged by the fraud service require CSR or managerial approval
    before fulfilment.
- **Fulfilment SLA:** **95 %** of orders must be shipped within
    **24 hours** of receipt during business days. Test by measuring
    processing times over a sample of orders.
- **Order modifications:** CSRs must be able to modify orders (address
    changes, item swaps, cancellations) prior to picking. Modifications
    must update pricing, taxes and availability accordingly.

### 3.4 Purchasing & Vendor Management

This module automates procurement and monitors vendor performance.

- **Reorder configuration:** Reorder points and EOQ must be defined
    for all stocked SKUs. When on‑hand plus on‑order quantity drops
    below reorder point, the system generates a draft purchase order
    (PO).
- **Approval workflows:** Purchase orders exceeding defined monetary
    thresholds must require approval. Approvals and rejections must be
    logged with user and timestamp.
- **ASN matching:** Received quantities must be matched against
    supplier Advance Shipping Notices (ASNs); variances must trigger
    discrepancy reports.
- **Vendor scorecards:** Delivery performance metrics (on‑time,
    complete, quality) must be updated with every receipt and available
    for reporting.
- **Drop‑ship accuracy:** For drop‑ship orders, vendor shipments must
    update cost, revenue and inventory records within **24 hours**.

### 3.5 Lot/Expiry & Serial Tracking

Lot and serial tracking enables compliance and recall management.

- **Lot capture:** For each lot‑controlled item, the system must
    capture lot number, manufacture date and expiry date at receiving.
    FIFO/FEFO rules must enforce proper picking sequence.
- **Serial capture:** Serial numbers must be recorded at receipt and
    shipment for high‑value tools; scanning ensures the correct serial
    is shipped.
- **Recall reporting:** The system must generate a recall report
    within **30 minutes** identifying all shipments and customers
    associated with a specified lot or serial number. Test by creating a
    lot and verifying recall results.
- **Warranty integration:** Serial numbers must link to warranty
    status and service history; warranty expirations must trigger
    notifications.

### 3.6 Shipping & Rate Shopping

Shipping functionality handles carrier integration, hazmat compliance
and cost optimisation.

- **Label & document generation:** Shipping labels, packing slips and
    hazmat documents must be generated for all orders where applicable.
    Hazmat paperwork must include required placards and classification
    codes.
- **Rate shopping:** The system must provide configurable options to
    choose the lowest cost or fastest service; selected rates must be
    logged for audit.
- **3PL integration:** For shipments handled by 3PL providers,
    tracking information and inventory status must be updated within
    **30 minutes**.
- **Hazmat selection:** Carriers that do not support hazmat shipments
    must be filtered out automatically when the order contains hazardous
    items.

### 3.7 Returns/RMA Workflow

Returns management standardises the process of handling returned goods
and refunds.

- **RMA initiation:** The system must generate a unique RMA number for
    every return request and send return instructions to the customer.
- **Inspection recording:** Returned items must be inspected, and
    their condition, lot/serial numbers and reason codes recorded.
- **Disposition handling:** Returned items must be processed according
    to disposition codes (restock, refurbish or scrap); inventory
    movements must reflect disposition.
- **Refund processing:** Refunds or credits must be posted
    automatically in the accounting system and visible in the customer’s
    account. Replacement orders must be linked to the original RMA.
- **Return reporting:** RMA volumes, reasons and dispositions must be
    reported and used to identify quality issues and reduce returns.

### 3.8 Tax Calculation & Reporting

Tax functionality must ensure accurate tax assessment and compliance
across jurisdictions.

- **Accurate calculation:** Tax must be calculated correctly for each
    order based on shipping address and product taxability. Test by
    placing orders with different tax jurisdictions and verifying
    calculations.
- **Multi‑state compliance:** The system must support nexus rules and
    tax registrations for each state; tax‑exempt customers must be
    handled via certificate management.
- **Reporting:** Users must be able to generate tax reports showing
    tax collected per jurisdiction (city, county, state).
- **Hazmat documentation:** For hazardous shipments, required shipping
    papers must be included and flagged for compliance.
- **Audit trail:** All tax calculations and filings must be logged for
    audit purposes; adjustments must be traceable.

### 3.9 Basic Accounting Integration

Accounting integration synchronises financial transactions and ensures
accurate reporting.

- **Journal entries:** Journal entries must be generated automatically
    for all sales invoices, purchase orders, returns, AP bills and
    inventory adjustments. Each entry must map to the appropriate GL
    account.
- **AP/AR synchronisation:** Accounts payable and accounts receivable
    data must be synchronised daily without manual intervention. Failed
    synchronisations must raise alerts.
- **Timely integration:** Sales orders and purchase orders must be
    reflected in the accounting system within **24 hours**.
- **Bank reconciliation:** The system must allow import of bank
    statements and automatic matching of deposits and payments.
    Unmatched transactions must be flagged for review.
- **COGS and revenue recognition:** COGS must be calculated based on
    the configured inventory valuation method (e.g., FIFO, average
    cost). Revenue must be recognised at the time of shipment.
- **Audit logs:** Detailed logs must record every financial
    transaction for audit and compliance.

### 3.10 Operational Analytics & Alerts

This module provides dashboards, alerts and reporting across the system.

- **Real‑time dashboards:** Dashboards must display up‑to‑date KPIs
    tailored to each persona (GM, Ops Manager, Purchasing Lead, CSR,
    etc.).
- **Alert notifications:** When thresholds (e.g., low stock, order
    delays, vendor lead time variance, tax liability) are exceeded,
    alerts must be sent to the appropriate users and logged for audit.
- **Ad‑hoc reporting:** Users must be able to build, save and run
    reports without technical assistance. Export to CSV or PDF must be
    supported.
- **Data warehouse integration:** An API or ETL interface must allow
    extraction of data for advanced analytics tools.

4 Acceptance Criteria for Phase 2 Enhancements
----------------------------------------------

The following criteria apply to enhancements planned for Phase 2 of the
ERP/IMS:

### 4.1 Demand Forecasting & EOQ Optimisation

- **Forecast Accuracy:**
  - Given historical sales data for a SKU with clear seasonality, when the forecasting model is run, then the generated forecast for the next period should reflect the seasonal trend with a Mean Absolute Percent Error (MAPE) of less than 15%.
  - The system's forecast for the top 100 SKUs by sales volume must be accurate within ±10% when compared to actual sales over a 3-month period.
- **Automated Recommendations:**
  - Given a SKU's forecast, lead time, and carrying cost, when the EOQ/reorder point job runs, then it must generate a purchase suggestion if the (on-hand + on-order) quantity is below the calculated reorder point.
  - A Purchasing Lead must be able to review, modify, or approve the system-generated reorder suggestions.
  - Users must be able to manually override the forecast for any SKU, and the system must use the manual forecast for its reorder calculations.
- **Scenario Simulation:**
  - Given a set of SKUs, a user must be able to run a "what-if" simulation by adjusting lead times by +10% and see the projected impact on safety stock levels and inventory value.
  - The simulation must allow users to compare at least two different scenarios against the current state.

### 4.2 Promotion Engine & Contract Pricing (B2B)

- **Automatic Application:**
  - Given a "Buy One, Get One Free" promotion for SKU-A, when a customer adds two SKU-A to their cart, then the price of one SKU-A is automatically deducted from the total.
  - Given a B2B customer is assigned to a specific contract price list, when that customer logs in and views a product page, then they must see their contract price, not the standard retail price.
- **Stacking & Exclusivity:**
  - Given two active promotions (e.g., "10% off" and "Free Shipping over $50"), if they are configured to be stackable, when a customer's order qualifies for both, then both promotions are applied.
  - Given two active promotions configured as mutually exclusive, when a customer's order qualifies for both, then only the promotion providing the larger discount is applied.
- **Contract & Promotion Management:**
  - An E-commerce Manager must be able to create, edit, and set effective start/end dates for all promotions and B2B contracts.
  - Expired contracts or promotions must not be applied to new orders.
- **Audit & Reporting:**
  - Every application of a promotion or contract price must be logged against the sales order line item.
  - A report must be available to show the total discount value applied per promotion code over a given date range.

### 4.3 Light Manufacturing/Kitting with BOM Versioning

- **Assembly & Disassembly:**
  - Given a work order to assemble 10 units of Kit-A, when the work order is completed, then the inventory of Kit-A increases by 10, and the inventory of its components decreases according to the BOM.
  - A warehouse user must be able to process a disassembly work order, which consumes the parent kit and increases the inventory of its components.
- **BOM Version Control:**
  - An engineer must be able to create a new version of a BOM (BOM-v2) with an effective start date.
  - Given that BOM-v2 for Kit-A becomes effective on Oct 1, when a work order for Kit-A is created on Oct 2, then the system must use BOM-v2 for component allocation. Work orders created before Oct 1 must continue to use BOM-v1.
- **Costing and Inventory Valuation:**
  - The cost of a finished kit must be calculated as the sum of the costs of its components at the time of assembly.
  - Component inventory must be backflushed from the correct bin locations upon work order completion. Any scrap reported must be moved to a scrap-holding location and deducted from inventory value.

### 4.4 3PL Bidirectional Integration

- **Inventory Alignment:**
  - Given the 3PL reports 100 units of SKU-A are on-hand, the ERP system's inventory for the 3PL location must reflect 100 units within 5 minutes of the sync.
  - If an inventory reconciliation job detects a discrepancy of more than 2 units between the ERP and 3PL, an alert must be automatically sent to the Operations Manager.
- **Order & Shipment Sync:**
  - Given a new sales order is ready for fulfillment, it must be transmitted to the 3PL via API within 10 minutes.
  - When the 3PL ships an order and updates the status via API, the ERP must update the order status to "Shipped" and store the tracking number within 15 minutes.
- **Return Handling:**
  - Given a return is received and processed at the 3PL, the RMA status in the ERP must be updated to "Received at 3PL" and inventory must be adjusted (e.g., moved to an inspection location) within 24 hours.
- **Error Handling:**
  - If an API call to the 3PL fails, the system must automatically retry up to 3 times. If it still fails, a critical alert must be sent to the IT Admin.

### 4.5 Customer Service Console with SLA Timers

- **Ticket & Case Management:**
  - Given a customer sends an email to support@, a new ticket must be created automatically in the CS Console with the email content.
  - A CSR must be able to manually create a ticket from a phone call and link it to a customer and a specific sales order.
- **SLA Timers & Escalations:**
  - Given a new high-priority ticket is created, its "Time to First Response" SLA timer must start immediately.
  - If a ticket's response time SLA is within 30 minutes of expiring, its color must change to amber in the queue. If it breaches the SLA, it must turn red and an alert must be sent to the Ops Manager.
- **Unified View:**
  - When a CSR opens a ticket, they must see the customer's complete order history, recent RMAs, and a log of all previous communications in a single view without navigating to other modules.

### 4.6 EDI with Large Vendors

- **EDI Document Exchange:**
  - Given a new PO is approved for an EDI-enabled vendor, the system must automatically generate and transmit an EDI 850 (Purchase Order) document to that vendor within 30 minutes.
  - When an EDI 856 (Advance Ship Notice) is received from a vendor, it must be automatically parsed, and the corresponding PO in the ERP must be updated with the shipment details.
  - When an EDI 810 (Invoice) is received, it must be matched against the PO and receipt data, flagging any price or quantity discrepancies for review by the accounting team.
- **Error Handling & Alerts:**
  - If an outgoing EDI 850 is not acknowledged (via an EDI 997) within 4 hours, an alert must be sent to the Purchasing Lead.
  - If an incoming EDI document fails to parse due to a schema error, it must be moved to an error queue and an alert sent to the IT Admin.
- **Configuration & Auditing:**
  - An IT Admin must be able to configure new EDI trading partners and their specific document mapping rules through a UI.
  - All inbound and outbound EDI transactions must be logged with their status, timestamps, and a copy of the raw data for auditing.

5 Acceptance and Verification Process
-------------------------------------

1. **Test planning:** For each feature, tests will be designed to
    verify all acceptance criteria. Functional tests, integration tests
    and performance tests will be included.
2. **Test execution:** Tests will be executed in a staging environment
    that mirrors production. Automated test suites will be used where
    possible; manual tests will validate user interactions and edge
    cases.
3. **Criteria evaluation:** A feature is accepted when all criteria are
    met and test results are documented. Failures will be logged and
    must be resolved before go‑live.
4. **Stakeholder sign‑off:** Product owners or business stakeholders
    must sign off on accepted features, confirming that the system meets
    business needs and compliance requirements.
5. **Regression and non‑functional tests:** Acceptance will also
    consider non‑functional requirements such as performance,
    availability, security and usability to ensure the overall system
    remains stable.
