## 7. Accounting Sync

### 7.1 Overview

The Accounting Sync module integrates ERP operational data with external
accounting systems (e.g. QuickBooks, NetSuite, general ledger). It
ensures that all financial transactions resulting from purchases, sales,
inventory movements and returns are correctly posted to the general
ledger and sub‑ledgers. An ERP accounting system integrates financial
management across HR, supply chain logistics, manufacturing, sales and
CRM, providing `realtime` data visibility. Modern accounting software
modules include general ledger, accounts payable, accounts receivable,
payroll, expense tracking, bank reconciliation and financial reporting.
The Accounting Sync module aims to synchronise these functions with the
ERP’s operational data.

### 7.2 Functional capabilities

1. **General ledger posting**
    - Map operational transactions to accounting journals. For
        example, when goods are received, debit inventory and credit
        accounts payable; when orders are shipped and invoiced, debit
        cost of goods sold and credit inventory.
    - Support different accounting methods (accrual vs. cash) and
        multi‑currency transactions.
    - Provide **GL mappings** configuration to assign ledger accounts
        by product category, location or transaction type.
2. **Accounts payable (AP) integration**
    - Synchronise vendor invoices from purchasing module to AP ledger.
        Include PO reference, receipt data, due date, discounts and
        approval status.
    - Support partial receipts and partial invoice matching.
    - Publish payment approvals and payment status updates back to
        ERP.
3. **Accounts receivable (AR) integration**
    - Synchronise customer invoices and credit memos. Include tax and
        shipping charges, payment terms and due dates.
    - Update customer balances and record payment receipts via the
        payment gateway.
4. **Bank reconciliation and cash management**
    - Import bank transactions; match them with recorded payments and
        receipts.
    - Create journal entries for bank fees and interest; reconcile
        cash accounts.
5. **Payroll and expense integration**
    - Optionally, import payroll journal entries from HR/payroll
        systems and allocate labour expenses to cost centres.
    - Import corporate card expenses and employee reimbursements into
        the GL and expense tracking.
6. **Data synchronization and reconciliation**
    - Provide two‑way synchronisation: push ERP transactions to
        accounting software and retrieve ledger balances for
        reconciliation. Handle data mapping differences (e.g. chart of
        accounts, customers vs. vendors definitions).
    - Implement batch and incremental sync; allow manual sync
        triggers.
    - Provide dashboards showing sync status (success, pending,
        failed) and reconciliation discrepancies.

### 7.3 Data entities

- **JournalEntry** – header with batch number, date, description and
    status; lines with account, debit/credit amounts, currency and
    reference ID.
- **LedgerAccountMapping** – links ERP transaction types to accounting
    ledger accounts.
- **SyncJob** – logs each sync run with source system, target system, start/end times, status and error details.
- **SyncError** – records individual failed records with reason and resolution flag.

### 7.4 Process flows

1. **Transaction posting**
    1. When an operational transaction occurs (e.g. PO receipt, order
        shipment), the ERP triggers creation of corresponding journal
        entries via mapping rules.
    2. Journal entries are stored in ERP and queued for
        synchronisation.
    3. Sync job processes queued entries, transforms them to the
        accounting system’s schema and calls the accounting API. If
        successful, marks the journal as posted.
    4. Accounting system returns a response with posted entry ID or
        error; errors are logged and require review.
2. **Reconciliation**
    1. At regular intervals, the sync job requests account balances
        from the accounting system.
    2. System compares ERP sub‑ledger totals with accounting system
        balances; any discrepancies generate reconciliation tasks.
    3. Users investigate and correct mapping or transaction errors.

### 7.5 API and integration

- **Inter-module Communication:** This module communicates with other internal modules exclusively by publishing and subscribing to domain events on the **in-process event bus**. It subscribes to events such as `orders.order-shipped`, `orders.order-cancelled`, and `purchasing.purchase-order-received` to trigger its workflows. Direct method calls between modules are not permitted.
- **Public API:** Functionality is exposed to external clients via the application's single, unified REST API, as defined in the `Api_specifications_and_endpoint_inventory.md` document.
- **External Integrations:** This module integrates with external accounting systems (e.g., QuickBooks, NetSuite) via dedicated **adapters**. These adapters are responsible for calling external APIs.

### 7.6 Roles and permissions

- **Accountant (Full control):** Configure ledger mappings, review and approve journal entries, initiate sync, and resolve sync errors.
- **Owner/GM (Approve):** Approve financial reports and view sync dashboards.
- **IT Administrator (Full control):** Full control for administrative purposes.
- **Read-Only Access:** The following roles have read-only access: Operations Manager, Purchasing Lead, E-Comm Mgr, B2B Account Mgr, and Auditor.
- **No Access:** The following roles have no access: Warehouse Associate, CSR.

### 7.7 Error handling

- **Mapping error:** If no mapping is defined for a transaction type,
    system logs error and notifies accountant; transaction remains
    unposted until mapping is added.
- **Sync failure:** If external API is unavailable or returns error,
    system retries with backoff; after repeated failures, marks sync as
    failed.
- **Reconciliation discrepancy:** System flags mismatch; user must
    resolve before closing period.

### 7.8 Performance and scalability

- Sync jobs must process all transactions within 30 minutes of the end
    of day. Use incremental sync to avoid processing the entire dataset
    daily.
- Support thousands of journal lines per day; batching and pagination
    are required for API calls.

### 7.9 Non‑functional considerations

- **Data integrity:** Ensure that no transactions are lost or
    duplicated. Use idempotent operations and unique reference IDs.
- **Security:** Secure connection to accounting systems via TLS; store
    API credentials securely. Restrict access to sync configuration.
- **Compliance:** Follow GAAP/IFRS standards and audit requirements;
    provide comprehensive audit trail of posted entries.
