## 6. Tax Calculation & Compliance

### 6.1 Overview

This module is responsible for calculating sales taxes for each order based on jurisdiction (county, state, country) and generating the data necessary for tax filing and compliance. To ensure accuracy, it integrates with a third-party tax service that must meet the project's defined SLA of **≥ 99.99% uptime** and **< 100ms latency**, as specified in the `integration_contracts_and_sla.md` document. Accurate tax calculation is critical for ensuring compliance across North Carolina, Virginia, South Carolina, and any future jurisdictions. The reporting capabilities of this module are strictly limited to generating tax liability reports and audit trails required for financial compliance.

### 6.2 Functional capabilities

1.  **Sales Tax Calculation**
    -   Determine tax rates based on `shipto` address, `shipfrom` address, and product taxability by calling a dedicated third-party tax API.
    -   Support tax exemptions for certified customers (e.g., resale certificates) and record exemption details against customer profiles and transactions.
    -   Cache frequently used tax rates (e.g., by zip code) to minimize API calls and provide resilience in case of API failure.

2.  **Tax Filing and Reporting**
    -   Generate monthly, quarterly, and annual tax liability reports by jurisdiction, summarizing taxable sales, non-taxable sales, and collected tax amounts.
    -   Provide detailed **audit trails** for every transaction, showing how taxes were calculated (input addresses, tax codes, rates, and API responses).
    -   Export tax data in formats compatible with state filing systems.

3.  **Hazmat Tax & Fee Compliance**
    -   Identify any products flagged as hazardous materials within an order.
    -   Calculate and report on any specific taxes or fees associated with the sale or transport of those hazardous materials, based on product attributes and shipping jurisdiction.

### 6.3 Data entities

-   **TaxTransaction** – Captures tax details for each sale: transaction ID, jurisdiction codes, taxable amount, tax amount, exemption code, API response ID, and timestamp.
-   **TaxRateCache** – Stores tax rates by zip code, state, and county with effective dates to be used as a fallback.
-   **ExemptionCertificate** – Stores customer tax exemption data, including certificate ID, jurisdiction, and expiry date.

### 6.4 Process flows

1.  **Tax Calculation During Order Placement**
    1.  The Order module calls the Tax module with the `shipto`/`shipfrom` addresses and line items.
    2.  The Tax module calls the external tax service. It then records the `TaxTransaction` and returns the final tax amount to the Order module.
    3.  If the API call fails, the system uses cached rates as a fallback, logs the event, and flags the transaction for review.

2.  **Filing Preparation**
    1.  A scheduler triggers a job to aggregate tax data at the end of a filing period.
    2.  The system aggregates `TaxTransaction` data by jurisdiction and generates the required filing reports.
    3.  The report is made available for the Accountant to review and submit to the relevant tax authority.

### 6.5 API and integration

-   **Inter-module Communication:** This module communicates with other internal modules exclusively by publishing and subscribing to domain events on the **in-process event bus**. For example, it consumes `orders.order-shipped` events to finalize and log tax transactions for reporting.
-   **Public API:** Tax-related functionality is exposed to external clients via the application's single, unified REST API, as defined in the `Api_specifications_and_endpoint_inventory.md` document.
-   **External Integrations:** This module integrates with a dedicated external tax calculation service via a dedicated **adapter**.

### 6.6 Roles and permissions

-   **Accountant (Full control):** Manage tax settings, run tax filings, and access financial reports.
-   **Owner/GM (Approve):** Approve tax filings and view reports.
-   **IT Administrator (Full control):** Full control for administrative purposes.
-   **Read-Only Access:** The following roles have read-only access: Operations Manager, Purchasing Lead, E-Comm Mgr, B2B Account Mgr, and Auditor.
-   **No Access:** The following roles have no access: Warehouse Associate, CSR.

### 6.7 Error handling

-   **API Failure:** If the live tax API returns an error or times out, the system uses cached rates to avoid blocking the order and marks the transaction for subsequent review and reconciliation.
-   **Invalid Address:** Returns an error to the calling module, requiring the user to provide a valid address before tax can be computed.

### 6.8 Performance and scalability

-   **Low Latency:** Tax calculations must meet the SLA of returning within **100 ms** on average, relying on a high-performance API provider and local caching.
-   **High Concurrency:** The module must support concurrent tax calculations for hundreds of orders per minute during peak periods.

### 6.9 Non‑functional considerations

-   **Compliance:** Keep transaction data and audit trails for regulatory retention periods (e.g., 7 years). Follow Sarbanes-Oxley (SOX) and relevant tax authority guidelines.
-   **Security:** Restrict access to sensitive tax data and reports. Encrypt API keys for the external tax service.
-   **Audit:** Provide reproducible calculation details for each transaction, including event logs and the raw API response data from the external service.
