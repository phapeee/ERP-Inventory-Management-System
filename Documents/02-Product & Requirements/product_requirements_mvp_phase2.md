Product Requirements Document: ERP/IMS for PineCone Pro Supplies
================================================================

**Document Version:** 1.0  
**Prepared By:** GPT-5  
**Date:** 27 September 2025

--------------------

1. Purpose and Scope

    --------------------

    PineCone Pro Supplies (PCS) has grown from a local distributor into a
    multi‑channel wholesaler and retailer with more than 12 000 active SKUs.
    This rapid expansion exposed gaps across purchasing, inventory accuracy,
    order orchestration, tax compliance and returns. The business intends to
    implement a modular ERP/Inventory Management System (IMS) that unifies
    product data, inventory control, order management, purchasing, warehouse
    operations, shipping, accounting integration and analytics. This
    document defines the detailed product requirements for the initial
    Minimum Viable Product (MVP) and outlines Phase 2 enhancements.

    This document covers functional and non‑functional requirements, user
    personas, acceptance criteria, assumptions, dependencies, constraints,
    and out‑of‑scope items. It serves as the basis for design, development,
    testing and deployment of the new ERP/IMS.

2. Objectives

    --------------------

    - Achieve **≥ 98 % inventory accuracy** through cycle counts and RF
        scanning.
    - **Meet a sub‑24 h SLA** for 95 % of orders placed during business
        days.
    - Automate purchasing using **reorder points**, economic order
        quantity (EOQ) and vendor lead‑time data.
    - Implement **lot/expiry and serial tracking** for hazardous finishing
        chemicals and high‑value tools.
    - Provide **unified financials** and tax compliance via accounting
        integration.
    - Offer a **single source of truth** for product data and pricing
        across all sales channels.
    - Lay foundations for Phase 2 capabilities: demand forecasting, EOQ
        optimisation, promotion engine, contract pricing, light
        manufacturing/kitting, 3PL integration, customer service console and
        EDI.

3. Stakeholders & Personas
    --------------------------

    | Persona                       | Role/Responsibilities                                                               |
    |-------------------------------|-------------------------------------------------------------------------------------|
    | **Owner/GM (Olivia)**         | Oversees P&L, approves high-value purchases, audits financials.                     |
    | **Ops Manager (Marcus)**      | Manages warehouse KPIs, slotting strategies and carrier performance.                |
    | **Purchasing Lead (Dina)**    | Manages vendor relationships, forecasts demand, plans POs.                          |
    | **Warehouse Associates (RF)** | Receives and puts away inventory, picks/ships orders, performs cycle counts.        |
    | **CSR (Ben)**                 | Handles order edits, returns/RMAs and customer communications.                      |
    | **E‑Commerce Manager (Tracy)**  | Manages catalog, pricing, promotions and channel synchronisation.                   |
    | **Accountant (Priya)**        | Handles reconciliation, tax filings and journal exports.                            |
    | **IT Admin (Kong)**           | Manages user access, roles, environments and deployments.                           |

    **Role Matrix (excerpt)**

    | Module         | Viewer     | Editor       | Approver | Admin |
    |----------------|------------|--------------|----------|-------|
    | Products       | CSR, Ops   | E-Com        | GM       | IT    |
    | Inventory      | CSR        | Ops          | Ops Mgr  | IT    |
    | Purchasing/PO  | CSR        | Purchasing   | GM       | IT    |
    | Orders/RMA     | CSR        | CSR/Ops      | Ops Mgr  | IT    |
    | Finance Export | Accountant | Accountant   | GM       | IT    |

4. In‑Scope vs Out‑of‑Scope

    --------------------

    **4.1 In‑Scope (MVP)**

    1. **Product Information Management (PIM)** – master data management
        for all SKUs.
    2. **Inventory & Warehouse Management** – multi‑warehouse control with
        bin/zone logic, RF scanning and cycle counts.
    3. **Order Management** – B2B/B2C order processing, payments, fraud
        checks.
    4. **Purchasing & Vendor Management** – vendor scorecards, lead times,
        MOQs, automated purchasing, ASN receiving and drop‑ship support.
    5. **Lot/Expiry & Serial Tracking** – for chemicals and high‑value
        tools.
    6. **Shipping & Rate Shopping** – carrier integration, hazmat rules and
        cross‑dock/3PL support.
    7. **Returns/RMA Workflow** – standardised process with disposition
        codes.
    8. **Tax Calculation & Reporting** – county‑level NC tax plus
        multi‑state readiness.
    9. **Accounting Integration** – GL/AP/AR synchronisation.
    10. **Operational Analytics & Alerts** – dashboards for KPIs and
        proactive notifications.

    **4.2 Phase 2 Enhancements**

    1. **Demand Forecasting & EOQ Optimisation**.
    2. **Promotion Engine & Contract Pricing (B2B)**.
    3. **Light Manufacturing/Kitting with BOM Versioning**.
    4. **3PL Bidirectional Integration**.
    5. **Customer Service Console with SLA Timers**.
    6. **EDI with Large Vendors**.

    **4.3 Out‑of‑Scope (Initial Release)**

    - Full manufacturing MRP/APS.
    - HR/Payroll.
    - Field Service Management.

5. Functional Requirements (MVP)

    --------------------

    **5.1 Product Information Management (PIM)**

    **Description:** Provide a single source of truth for more than 12 000
    SKUs across web storefront, B2B portal, POS and Amazon channels. Support
    complex product structures (kits/bundles), hazardous classifications,
    unit conversions (quart/gal/5‑gal) and pricing tiers.

    **Key Features & Functionalities**

    - **Master product catalog:** Create and manage SKU records with
        attributes (name, description, category, brand, dimensions, weight,
        hazard class, expiry period, images).
    - **Variant & bundle management:** Support parent/child relationships,
        kits and bundles, and BOM (bill of materials) references for kit
        assembly.
    - **Unit conversion:** Automatically convert units (e.g., quart to
        gallon) for ordering and pricing.
    - **Channel synchronisation:** Publish and update product data across
        web store, Amazon, POS and B2B portal.
    - **Data governance:** Roles & permissions for product
        creation/editing; audit trails of changes.

    **Acceptance Criteria**

    - All SKUs have unique IDs and required attributes; data is consistent
        across all channels.
    - Hazardous classifications and expiry periods are stored and
        exportable for shipping compliance.
    - Kits/bundles can be created with dynamic component quantities and
        updated BOM versioning.
    - Unit conversions are handled automatically during order processing
        and pricing.
    - Product updates are propagated to channels within 15 minutes.

    **5.2 Inventory & Warehouse Management**

    **Description:** Maintain real‑time inventory visibility across multiple
    warehouses (Greensboro DC, Charlotte cross‑dock, Reno 3PL) with bin/zone
    location management and RF scanning. Aim for ≥ 98 % inventory accuracy
    and reliable available‑to‑promise (ATP).

    **Key Features & Functionalities**

    - **Multi‑location inventory tracking:** Real‑time quantities and
        statuses per SKU, per location (stock, reserved, on order).
    - **Bin/zone management:** Define zones (receiving, forward pick,
        overstock, returns) and bins within each warehouse. Slot items based
        on velocity and hazard class.
    - **RF scanning workflows:** Receiving, put‑away, picking, cycle count
        and RMA processing via handheld devices.
    - **Cycle counting:** Configurable frequency by ABC class with
        automatic variance adjustments and recount workflows.
    - **Cross‑dock & 3PL integration:** Manage trans‑shipments from
        Greensboro to cross‑dock and update inventory based on 3PL feeds.
    - **Expiry and serial control:** Enforcement of FIFO/FEFO picking for
        lot‑controlled items; tracking serial numbers for high‑value tools.
    - **ATP calculation:** Real‑time available‑to‑promise across all
        channels including pending POs and reserved stock.

    **Acceptance Criteria**

    - Inventory accuracy measured by cycle count variance is ≥ 98 %.
    - RF scanning transactions update inventory within 1 minute.
    - Serial and lot numbers are captured at receiving and available for
        look‑up during shipping and RMA.
    - Cross‑dock and 3PL transfers update inventory within 15 minutes of
        receipt.
    - ATP is recalculated upon order entry and displays accurate available
        quantities to prevent oversells.

    **5.3 Order Management & Payments/Fraud**

    **Description:** Orchestrate orders from B2B/B2C channels (web, B2B
    portal, POS, Amazon FBM/FBA), including payment processing and fraud
    checks. Achieve a sub‑24‑hour SLA for 95 % of orders.

    **Key Features & Functionalities**

    - **Channel integration:** Consolidate orders from e‑commerce, Amazon,
        B2B portal and POS via APIs or file exchanges.
    - **ATP & back‑order management:** Reserve stock upon order creation;
        manage partial shipments; allocate replenishment to back orders.
    - **Payments and fraud screening:** Integrate payment gateways (credit
        card, ACH) and fraud services; support deposit holds for B2B orders.
    - **Order workflow:** Support statuses from pending → processing →
        picking → packed → shipped; allow CSR to edit orders (address
        changes, item swaps) before shipment.
    - **Shipment & tracking:** Create shipments, print labels, assign
        carriers (lowest rate or fastest), and update customers with
        tracking numbers.
    - **Customer communication:** Send order confirmations, shipping
        notifications and out‑of‑stock alerts.

    **Acceptance Criteria**

    - Orders from all channels are visible in a single queue.
    - Payment is authorised or charged at order capture and logged in the
        accounting integration.
    - Fraud check results are recorded; flagged orders require CSR
        approval.
    - 95 % of orders are shipped within 24 hours of receipt (business
        days).
    - CSR can modify orders prior to picking; modifications update pricing
        and taxes accordingly.

    **5.4 Purchasing & Vendor Management**

    **Description:** Provide vendor scorecards, handle minimum order
    quantities and lead‑time data, and automate purchasing using reorder
    points and EOQ. Support ASN (Advanced Shipping Notice) receiving and
    vendor drop‑ship.

    **Key Features & Functionalities**

    - **Vendor master data:** Maintain vendor profiles with contact
        information, terms, lead time, minimum order quantities (MOQs) and
        pricing.
    - **Reorder point & EOQ:** Calculate reorder points per SKU based on
        safety stock and lead time; recommend order quantities using EOQ.
    - **Purchase order automation:** Generate POs automatically when
        on‑hand + on‑order falls below reorder point; route POs for approval
        based on monetary thresholds.
    - **Vendor scorecard:** Track delivery performance (on‑time, complete,
        accurate), quality incidents and price variance; display vendor
        ratings.
    - **ASN & receiving:** Accept supplier ASNs; book in quantities by
        lot/serial; update inventory and notify purchasers of discrepancies.
    - **Drop‑ship & vendor‑managed inventory:** Send orders directly to
        vendor for drop‑ship fulfilment; record cost and revenue.

    **Acceptance Criteria**

    - Reorder points and EOQ are configured for all stocked SKUs; the
        system generates draft POs automatically.
    - POs require approval if above defined thresholds; approvals are
        logged.
    - ASN lines match receipts; discrepancies trigger variance reports.
    - Vendor performance metrics are updated with each receipt and
        available for reporting.

    **5.5 Lot/Expiry & Serial Tracking**

    **Description:** Track lot numbers and expiration dates for finishing
    chemicals and serial numbers for high‑value tools, enabling recalls and
    warranty/service support.

    **Key Features & Functionalities**

    - **Lot control:** Capture lot number, manufacture date and expiry
        date at receiving; enforce FIFO/FEFO picking for lot‑controlled
        items.
    - **Serial number tracking:** Record serial numbers for high‑value
        tools at receipt and shipment; maintain warranty start dates.
    - **Recall management:** Provide search/report to identify all
        customers and shipments associated with a lot or serial number.
    - **Warranty & service integration:** Provide lookup for serial
        numbers to check warranty status and service history.

    **Acceptance Criteria**

    - Lot numbers and expiry dates are recorded for all regulated
        products.
    - Serial numbers are captured and stored for high‑value tools;
        scanning ensures correct serial number is shipped.
    - Recall report can be generated within 30 minutes identifying all
        shipments containing a specified lot/serial.

    **5.6 Shipping & Rate Shopping**

    **Description:** Integrate with carriers to compare shipping rates,
    manage hazardous‑materials documentation and support cross‑dock/3PL
    workflows.

    **Key Features & Functionalities**

    - **Carrier integration:** Connect to carrier APIs (UPS, FedEx, USPS,
        LTL) for rate quotes, label printing and tracking.
    - **Rate shopping:** Determine best shipping method based on cost,
        destination and delivery time; present options to CSR or pick
        automatically.
    - **Hazmat compliance:** Support hazmat labels and documentation;
        maintain material safety data sheet (MSDS) links; ensure carriers
        that handle hazardous goods are selected.
    - **Cross‑dock & 3PL support:** Generate trans‑shipment documents;
        coordinate shipments from DC to cross‑dock or 3PL and capture
        carrier pickups/deliveries.

    **Acceptance Criteria**

    - Shipping labels and documents are produced for all orders, including
        hazmat paperwork where required.
    - Rate shopping is configurable to choose lowest cost or fastest
        service; logs decisions for audit.
    - 3PL shipments update tracking info and inventory statuses within
        30 minutes.

    **5.7 Returns/RMA Workflow**

    **Description:** Provide a standardised returns management process with
    disposition codes (restock, refurbish, scrap) to reduce friction and
    improve inventory accuracy.

    **Key Features & Functionalities**

    - **RMA initiation:** Generate RMA numbers for customer returns; send
        return instructions.
    - **Receiving & inspection:** Capture returned goods, record
        condition, lot/serial numbers and reason codes.
    - **Disposition & inventory update:** Apply disposition codes to
        returned items (restock to available stock, refurbish to inspection
        queue, scrap to disposal); adjust inventory accordingly.
    - **Refunds & replacements:** Integrate with accounting to issue
        refunds or credits; link replacement orders to original RMA.
    - **Reporting:** Track RMA volumes, reasons and dispositions to
        identify quality issues and reduce returns.

    **Acceptance Criteria**

    - RMAs are created for all returns and tracked through resolution.
    - Disposition codes determine inventory movements and triggers for
        refurbishment or scrap.
    - Refunds/credits are posted in the accounting system and visible in
        customer accounts.

    **5.8 Tax Calculation & Reporting**

    **Description:** Automate county‑level sales tax for North Carolina;
    prepare for expansion to Virginia and South Carolina. Provide hazmat
    documentation and audit trail.

    **Key Features & Functionalities**

    - **Tax engine:** Apply appropriate tax rates based on customer
        address (city, county, state); update rates when new jurisdictions
        are added.
    - **Multi‑state compliance:** Maintain nexus rules and tax
        registration details for each state; support tax‑exempt customers
        with certificate management.
    - **Hazmat documentation:** Produce shipping papers required for
        hazardous goods; capture driver and placard information.
    - **Reporting & filing:** Generate monthly/quarterly tax reports;
        summarise taxable vs non‑taxable sales; produce audit trails of tax
        calculations.

    **Acceptance Criteria**

    - Tax is calculated accurately for all orders based on ship‑to address
        and product taxability.
    - Reports can be generated showing tax due per jurisdiction.
    - Hazmat shipments include required documents and are flagged for
        compliance.

    **5.9 Basic Accounting Integration**

    **Description:** Synchronise general ledger, accounts payable and
    accounts receivable data to provide unified financials.

    **Key Features & Functionalities**

    - **Journal entry generation:** Create journal entries for sales
        invoices, returns, POs, AP bills and inventory adjustments; map to
        appropriate GL accounts.
    - **GL/AP/AR integration:** Export transactions to or synchronise with
        the accounting system; update balances daily.
    - **Bank reconciliation:** Import bank statements; match deposits and
        payments; reconcile differences.
    - **Revenue recognition & cost of goods:** Recognise revenue upon
        shipment; calculate COGS based on inventory valuation method.
    - **Audit trails:** Maintain detailed transaction logs for audits.

    **Acceptance Criteria**

    - Journal entries are generated for all financial transactions with
        correct GL mapping.
    - AP/AR data is synchronised daily without manual intervention.
    - All sales orders and POs are reflected in the accounting system
        within 24 hours.

    **5.10 Operational Analytics & Alerts**

    **Description:** Provide dashboards and alerts for key performance
    indicators such as cycle‑count accuracy, order fulfilment times, vendor
    performance and tax liabilities.

    **Key Features & Functionalities**

    - **Dashboards:** Customisable dashboards by persona (GM, Ops Manager,
        Purchasing Lead, etc.), showing current KPIs, charts and trends.
    - **Alerts & notifications:** Configurable threshold alerts (e.g., low
        stock, order delays, vendor lead time variance, tax liability
        approaching threshold).
    - **Ad‑hoc reporting:** Allow users to build and save reports; export
        data to CSV/PDF.
    - **Data warehouse integration:** Provide API/ETL access for advanced
        analytics tools.

    **Acceptance Criteria**

    - Dashboards display real‑time data; each persona can access relevant
        metrics.
    - Alerts notify the appropriate user when thresholds are exceeded;
        logs are available for audit.
    - Reports can be generated and exported without technical assistance.

6. Phase 2 Requirements

    --------------------

    The following requirements are planned for Phase 2. They are not part of
    the MVP but are included for future planning and should be considered in
    the architecture.

    **6.1 Demand Forecasting & EOQ Optimisation**

    - **Description:** Use historical sales data, seasonality and
        promotions to predict future demand by SKU; recommend EOQ and
        reorder points accordingly.
    - **Acceptance Criteria:** Forecast accuracy within ±10 % for the top
        100 SKUs; reorder suggestions generated monthly; user can override
        forecasts.

    **6.2 Promotion Engine & Contract Pricing (B2B)**

    - **Description:** Configure promotions (discounts, buy‑one‑get‑one,
        free shipping) and customer‑specific contract pricing tiers for B2B
        customers.
    - **Acceptance Criteria:** Promotions and contract prices are applied
        automatically during order entry; promotion stacking rules are
        supported; expiry dates enforced.

    **6.3 Light Manufacturing/Kitting with BOM Versioning**

    - **Description:** Support light manufacturing assembly and
        disassembly of kits; manage bill of materials (BOM) versions and
        track component consumption.
    - **Acceptance Criteria:** Kits can be assembled/disassembled; BOM
        versions updated and effective‑dated; inventory for components and
        finished kits updates accordingly.

    **6.4 3PL Bidirectional Integration**

    - **Description:** Integrate with third‑party logistics providers via
        API or EDI to synchronise inventory, orders, shipments and returns.
    - **Acceptance Criteria:** Inventory quantities at 3PL align with ERP;
        shipments created by 3PL update status and tracking automatically;
        returns processed via 3PL update inventory.

    **6.5 Customer Service Console with SLA Timers**

    - **Description:** Provide a unified console for customer service
        representatives to manage tickets, order issues and returns; track
        response and resolution times against SLAs.
    - **Acceptance Criteria:** Tickets are logged and assigned; SLA timers
        displayed; escalations triggered when SLA thresholds are exceeded.

    **6.6 EDI with Large Vendors**

    - **Description:** Exchange purchase orders, invoices, ASNs and other
        documents via EDI with key vendors.
    - **Acceptance Criteria:** EDI documents are transmitted and
        acknowledged automatically; error handling alerts users; trading
        partner requirements are configurable.

7. Non‑Functional Requirements

    --------------------

    - **Performance:** The system must support up to 200 concurrent users;
        order processing latency ≤ 2 seconds; inventory updates propagate
        within 1 minute.
    - **Scalability:** Modular architecture to add new modules (e.g.,
        manufacturing) without system downtime; support additional
        warehouses and 3PLs.
    - **Availability:** 99.9 % uptime with scheduled maintenance windows.
    - **Security:** Role‑based access control (RBAC) for all modules; data
        encryption at rest and in transit; audit logs.
    - **Compliance:** Adhere to OSHA and DOT regulations for hazardous
        materials; comply with tax laws; support audit trails.
    - **Internationalisation:** Initially US‑only; architecture should
        allow future localisation (currency, language, tax).

8. Assumptions

    --------------------

    - Existing web storefront, Amazon and POS systems provide API access
        for integration.
    - Users will be trained on new workflows and new roles will be defined
        where needed.
    - The accounting system supports API integration for GL/AP/AR
        synchronisation.
    - Carrier APIs and EDI specifications are available for shipping and
        vendor integration.

9. Dependencies & Risks

    --------------------

    - **Vendor data accuracy:** Success depends on accurate lead time, MOQ
        and cost data from suppliers.
    - **Integration complexity:** Multiple third‑party systems (web store,
        Amazon, POS, 3PL, accounting) may increase integration effort.
    - **Change management:** Users may resist new processes; training and
        change management are critical.
    - **Regulatory changes:** New tax jurisdictions or hazmat regulations
        could impact design.
    - **Data migration:** Legacy data quality must be assessed; migration
        plan required.

10. Acceptance & Success Metrics

    --------------------

    The project will be considered successful when the following criteria
    are met:

    - Inventory accuracy maintained at or above 98 % for three consecutive
        months.
    - 95 % of orders processed and shipped within 24 hours (business days)
        for three consecutive months.
    - 80 % reduction in purchase order lead‑time variance through
        automation and vendor management.
    - All products available to promise across channels with no oversells.
    - Tax reports filed accurately on time; no compliance penalties.
