# ERP / Inventory Management System — Business Scenario

**Company:** **PineCone Pro Supplies, LLC**  
**Industry:** B2B/B2C Wholesale & Retail — Professional woodworking, finishing, and shop safety supplies  
**Headquarters:** Greensboro, North Carolina (primary DC)  
**Warehouses:** Greensboro, NC (DC-01), Charlotte, NC (XDock-CLT), Reno, NV (3PL)  
**Sales Channels:** Web storefront, B2B portal, Amazon (FBM/FBA), in-store POS (Greensboro showroom)  
**Fiscal Calendar:** Monthly; sales tax nexus in NC (county-level rates), expanding to VA/SC within 12 months

---

## 1) Executive Summary

PineCone Pro Supplies (PCS) has grown from a local distributor to a multi-channel seller with ~12,000 active SKUs, seasonal kits, and vendor drop-ship programs. Rapid growth exposed gaps across purchasing, inventory accuracy, order orchestration, tax compliance, and returns. The goal is to implement a modular **ERP/IMS** that unifies product data, inventory control, order management, purchasing, warehouse operations, shipping, accounting integration, and analytics.

    **Primary outcomes**

- 98%+ inventory accuracy (cycle counts + RF scanning)
- Sub-24h order SLA for 95% of orders (weekday)
- Automated purchasing (reorder points, EOQ, vendor lead-time)
- Lot/expiry tracking for finishing chemicals; serial tracking for high-value tools
- Unified financials via accounting integration and tax compliance reporting
- Single source of truth for product data and pricing

---

## 2) Business Context & Pain Points

- **Stockouts & Inaccurate ATP:** Manual adjustments and paper pick lists cause oversells and delayed shipments.
- **Fragmented Systems:** Web store, 3rd-party WMS, and spreadsheets don’t sync reliably.
- **Purchasing Guesswork:** No unified view of vendor lead times, MOQs, and historical demand.
- **Complex Catalog:** Kits/bundles, unit conversions (quart/gal/5-gal), hazardous shipping rules.
- **Returns Handling (RMA):** No standard workflow or disposition codes (restock/refurbish/scrap).
- **Compliance:** County-level sales tax (NC), hazmat shipping documentation, audit trails.

---

## 3) Scope (MVP → Phase 2)

    **In-Scope (MVP)**

- Product Information Management (PIM)
- Inventory & Warehouse (bin/zone, RF picking)
- Orders (B2C/B2B), payments, fraud checks
- Purchasing, Vendor management, ASN receiving
- Shipping & Rate shopping, hazmat rules
- Returns/RMA workflow
- Tax calculation & reporting (US w/ NC focus)
- Basic Accounting integration (GL/AP/AR sync)
- Operational Analytics & Alerts

    **Phase 2**

- Demand forecasting, EOQ optimization
- Promo engine & contract pricing (B2B)
- Light manufacturing/kitting with BOM versioning
- 3PL bidirectional integration
- Customer service console with SLA timers
- EDI with large vendors

    **Out-of-Scope (initially)**

- Full manufacturing MRP/APS
- HR/Payroll
- Field service

---

## 4) Personas & Roles (RBAC)

- **Owner/GM (Olivia):** P&L, approvals, audit.
- **Ops Manager (Marcus):** Warehouse KPIs, slotting, carrier performance.
- **Purchasing Lead (Dina):** Vendor scorecards, PO planning.
- **Warehouse Associates (RF):** Receiving, putaway, pick/pack/ship, cycle count.
- **CSR (Ben):** Order edits, RMAs, customer comms.
- **E-Commerce Manager (Tracy):** Catalog, pricing, promotions, channel sync.
- **Accountant (Priya):** Reconciliation, tax filings, journal exports.
- **IT Admin (Kong):** Users, roles, environments, deployments, observability.

    **Role Matrix (excerpt)**

| Module         | Viewer     | Editor     | Approver | Admin |
| -------------- | ---------- | ---------- | -------- | ----- |
| Products       | CSR, Ops   | E-Com      | GM       | IT    |
| Inventory      | CSR        | Ops        | Ops Mgr  | IT    |
| Purchasing/PO  | CSR        | Purchasing | GM       | IT    |
| Orders/RMA     | CSR        | CSR/Ops    | Ops Mgr  | IT    |
| Finance Export | Accountant | Accountant | GM       | IT    |
