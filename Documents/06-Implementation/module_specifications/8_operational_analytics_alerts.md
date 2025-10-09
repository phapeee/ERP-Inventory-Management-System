## 8. Operational Analytics & Alerts

### 8.1 Overview

The Analytics & Alerting module provides visibility into the operational health of the ERP/IMS. It aggregates data from all other modules into a dedicated reporting data store to deliver persona-based dashboards, key performance indicators (KPIs), and proactive alerts. This module enables data-driven decision-making for stakeholders at all levels, from warehouse associates to the general manager. The primary goal is to surface actionable insights, identify bottlenecks, and flag anomalies before they impact business operations.

### 8.2 Functional capabilities

1.  **Real-time Dashboards**
    -   Provide customizable dashboards tailored to each user persona.
    -   Visualize data using charts, graphs, and tables with drill-down capabilities.
    -   Display key KPIs specific to user roles. Examples include:
        -   **Owner/GM:** Overall Sales Revenue, Profit Margin, Top 10 Products by Revenue, Inventory Value.
        -   **Ops Manager:** Order Fulfillment Time (Order-to-Ship), Inventory Accuracy (Cycle Count Variance %), Shipping Costs vs. Budget.
        -   **Purchasing Lead:** Vendor On-Time Delivery Rate, Purchase Price Variance (PPV), Stock-out Rate by Vendor.

2.  **Alerts & Notifications**
    -   Configure threshold-based alerts for critical business metrics (e.g., low stock levels, order delays, vendor lead time variance, tax liability approaching threshold).
    -   Send notifications to the appropriate users or roles via email, SMS, or in-app notifications.
    -   Maintain a log of all triggered alerts for auditing and analysis.

3.  **Ad-hoc Reporting**
    -   Allow users with appropriate permissions to build, save, and run custom reports.
    -   Provide a user-friendly interface for selecting data fields, applying filters, and defining report layouts.
    -   Support exporting reports to various formats, including CSV and PDF.

4.  **Data Warehouse Integration**
    -   Provide a documented API or ETL interface to extract aggregated data for use in external business intelligence (BI) tools.

### 8.3 Data Entities

-   **Dashboard** - Stores the configuration for each user-defined dashboard, including layout, widgets, and data sources.
-   **Widget** - Represents a single visualization on a dashboard (e.g., a chart, a KPI card).
-   **AlertRule** - Defines the conditions for triggering an alert, including the metric, threshold, and notification settings.
-   **AlertLog** - Records every instance of a triggered alert, including the timestamp, severity, and associated data.
-   **ReportDefinition** - Metadata for saved ad-hoc reports (name, description, data query, parameters).

### 8.4 Data Architecture

This module operates on a dedicated **reporting data store**, which is physically separate from the operational database to ensure that analytical queries do not impact transactional performance. The architecture follows an ELT (Extract, Load, Transform) pattern:

1.  **Extract/Load:** The module includes event handlers that subscribe to domain events from all other modules. When an event is received (e.g., `orders.order-shipped`), its raw data is loaded into staging tables within the reporting data store.
2.  **Transform:** Scheduled transformation jobs run periodically (e.g., every 15 minutes) to process the raw event data from the staging tables into aggregated, query-optimized models (e.g., fact and dimension tables).
3.  **Serve:** Dashboards, reports, and alerts query these aggregated models, ensuring fast response times.

For the MVP, this data store will be implemented as a separate schema within the main PostgreSQL database. The design allows for it to be migrated to a dedicated data warehouse solution (e.g., Azure Synapse Analytics) in the future without impacting the module's core logic.

### 8.5 Process Flows

1.  **Dashboard Rendering**
    1.  A user navigates to their dashboard.
    2.  The system retrieves the dashboard configuration and queries the pre-aggregated models in the **reporting data store** for each widget.
    3.  The dashboard is rendered with the latest aggregated data.

2.  **Alert Triggering**
    1.  A scheduled job queries the aggregated models in the reporting data store to check conditions for each active alert rule.
    2.  If a condition is met, an alert is logged, and notifications are sent.

3.  **Ad-hoc Report Generation**
    1.  A user creates or runs a report.
    2.  The system executes the report's query against the reporting data store.
    3.  The results are formatted and presented to the user for viewing or download.

### 8.6 API and Integration

-   **Inter-module Communication:** This module is a net consumer of events. It subscribes to domain events from all other modules on the **in-process event bus** to populate its reporting data store. It does not publish any events.
-   **Public API:** Exposes a REST API for managing dashboards, alert rules, and reports, as defined in `api_specifications_and_endpoint_inventory.md`.
-   **External Integrations:** Provides a data export API for integration with external BI tools.

### 8.7 Roles and Permissions

-   **Full Control:** Owner/GM, Operations Manager, Purchasing Lead, E-Comm Mgr, Accountant, IT Administrator.
-   **View Only:** CSR.
-   **Read-Only (Auditor):** The Auditor role has read-only access to all data for compliance purposes.
-   **No Access:** Warehouse Associate, B2B Account Manager.

### 8.8 Error Handling

-   **Data Source Unavailable:** If the reporting data store is unavailable, a system-wide banner is shown. Individual widgets or reports will show a clear error state.
-   **Alerting Failure:** If an alert notification fails to send, the system retries up to 3 times and logs the final failure for review.
-   **Reporting Failure:** If a report fails to generate due to a data or query error, the system logs the error and notifies the user who initiated the report.

### 8.9 Performance and Scalability

-   **Dashboard Performance:** Dashboards must load in under 5 seconds. The data latency (from operational event to dashboard visibility) should be under 15 minutes.
-   **Alerting Latency:** Alerts should be triggered within 5 minutes of the underlying condition being met in the data store.
-   **Asynchronous Reporting:** Ad-hoc reports on large datasets must be run as asynchronous background jobs to prevent timeouts. The user should be notified when the report is ready for download.

### 8.10 Non-functional considerations

-   **Data Accuracy:** The data in the reporting store must be verifiable against the operational data. Reconciliation jobs should run periodically to ensure consistency.
-   **Security:** Access to data, reports, and dashboards must be strictly enforced based on user roles.
-   **Usability:** The interface for creating dashboards and reports should be intuitive for non-technical users.
