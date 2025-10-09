### Phase 1: Foundational Setup (Azure & Project Configuration)

This phase involves provisioning the necessary cloud resources and configuring your application to communicate with them.

* [x] **Provision Azure Resources:**
  * [x] Create an **Azure Log Analytics Workspace** to serve as the central repository for logs and metrics.
  * [x] Create an **Application Insights** resource. During creation, link it to the Log Analytics Workspace you just created for unified data storage.
  * [x] Securely store the **Instrumentation Key / Connection String** from the Application Insights resource. You will need this to configure your application.

* [x] **Configure the .NET Project:**
  * [x] Add the required NuGet packages to your `.csproj` file:
    * `Microsoft.ApplicationInsights.AspNetCore`
    * `Serilog.Sinks.ApplicationInsights` (or a similar sink if you use a different logging framework).
  * [x] In your `Program.cs`, register the Application Insights service: `builder.Services.AddApplicationInsightsTelemetry();`.
  * [x] In your `appsettings.json`, add the Application Insights connection string. Use different settings for each environment (Development, Staging, Production) to send telemetry to different Application Insights instances if needed.

        ```json
        "ApplicationInsights": {
          "ConnectionString": "Your-Connection-String-Goes-Here"
        }
        ```

  * [x] If deploying to Azure, ensure the application's **Managed Identity** has the `Monitoring Metrics Publisher` role on the Application Insights resource to authorize it to send telemetry.

### Phase 2: Instrumentation (Code-Level Implementation)

This phase involves adding the code to your application to generate the logs, metrics, and traces defined in the catalog.

* [ ] **Implement Structured Logging:**
  * [ ] Configure a logging framework like **Serilog** to write logs in a structured JSON format.
  * [ ] Configure the logging sink to send all logs to the Application Insights instance configured in Phase 1.
  * [ ] **Code Review:**
    * [ ] Go through the codebase and ensure all `Log` statements are structured and include meaningful context (e.g., `Log.Information("Processing order {OrderId}", order.Id);`).
    * [ ] Verify that log messages are specific and avoid generic statements like "An error occurred."
  * [ ] Implement a middleware to generate a unique **Correlation ID** for each incoming request and attach it to all log entries for that request's lifetime.
  * [ ] **Security:** Scan the codebase to ensure no Personally Identifiable Information (PII) or other sensitive data is being written to logs.

* [ ] **Implement Metrics & KPIs:**
  * [ ] **Golden Signals:** Auto-instrumentation from the Application Insights SDK will capture the four golden signals (Latency, Traffic, Errors, Saturation) out of the box.
  * [ ] **Custom KPIs:**
    * [ ] For each module, identify the custom business KPIs listed in the alert catalog (e.g., `inventory_accuracy`, `rma_issuance_rate`).
    * [ ] In your code, inject the `TelemetryClient`.
    * [ ] Use `_telemetryClient.TrackMetric("MetricName", value)` to record these custom metrics at the appropriate points in your business logic.

* [ ] **Implement Distributed Tracing:**
  * [ ] The Application Insights SDK enables distributed tracing by default. Verify that it is active.
  * [ ] For critical or complex business workflows, add custom spans to get more detailed traces. You can do this using `System.Diagnostics.Activity` and `ActivitySource`.
  * [ ] Verify that the `traceId` and `spanId` are automatically included in your structured logs to allow for easy correlation between traces and logs.

### Phase 3: Alerting & Dashboarding (Azure Monitor Configuration)

This phase involves using the data you're now collecting to create the dashboards and alerts defined in the catalog.

* [ ] **Create Dashboards (Azure Monitor Workbooks):**
  * [ ] In the Azure portal, navigate to your Application Insights resource and create a new **Workbook**.
  * [ ] Create a "Golden Signals" dashboard with visualizations for Latency, Traffic, Errors, and Saturation using the KQL queries from the catalog.
  * [ ] Create separate sections or a new workbook for the module-specific KPIs you instrumented in Phase 2.
  * [ ] For each chart, add titles and descriptions to provide context.

* [ ] **Configure Alerts:**
  * [ ] For **each alert** defined in the catalog's tables:
    * [ ] In Azure Monitor, create a new **Alert Rule**.
    * [ ] Set the scope to your Application Insights resource.
    * [ ] Under "Condition," write the **KQL query** that defines the alert's logic based on the metric and threshold from the catalog.
    * [ ] Configure the alert **severity** (Sev 1, Sev 2, etc.).
    * [ ] Create an **Action Group** to define who gets notified (e.g., an email to the engineering team, a webhook to Slack, or a notification to PagerDuty).
    * [ ] In the alert's details, add a link to the corresponding runbook for that alert.

* [ ] **Create Runbooks:**
  * [ ] For each alert you configured, create a corresponding runbook as a Markdown file in your source control (e.g., in a `/runbooks` directory).
  * [ ] Each runbook should contain clear, step-by-step instructions for the on-call engineer to diagnose and mitigate the issue.

### Phase 4: Lifecycle & Continuous Improvement

Observability is not a one-time setup. This phase covers the ongoing processes to maintain and improve your observability posture.

* [ ] **Onboarding & Training:**
  * [ ] Hold a session with the development and operations teams to walk through the new dashboards.
  * [ ] Train all on-call engineers on the incident response process, how to interpret alerts, and how to use the runbooks.

* [ ] **Establish Regular Reviews:**
  * [ ] Schedule a recurring weekly or bi-weekly meeting to review the alerts that fired in the previous period.
  * [ ] Use this meeting to identify "flappy" or noisy alerts and create tasks to tune their thresholds or logic.
  * [ ] Track metrics on alert fatigue (e.g., number of alerts per on-call shift).

* [ ] **Integrate into Development Workflow:**
  * [ ] Update your team's "Definition of Done" for new features to include "Update the observability catalog and implement necessary instrumentation."
  * [ ] Conduct post-mortems after every production incident to identify gaps in observability (e.g., "What metric could have alerted us to this sooner?").
  * [ ] Periodically run chaos engineering experiments to proactively test that your alerts fire as expected and that your runbooks are accurate.
