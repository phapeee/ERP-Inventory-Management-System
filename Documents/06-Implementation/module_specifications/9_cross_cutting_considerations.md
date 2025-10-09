## 9. Cross-Cutting Implementation Guide

### 9.1 Overview

This document provides mandatory implementation standards, patterns, and library choices for all modules within the ERP/IMS. Adhering to these standards is crucial for ensuring consistency, maintainability, and compliance with the system's non-functional requirements. This guide moves beyond architectural principles to define *how* developers will implement these cross-cutting concerns.

### 9.2 Configuration Management

All module configuration must follow the ASP.NET Core `IConfiguration` pattern.

1.  **Hierarchy**: Configuration will be loaded from the following sources in order:
    1.  `appsettings.json` (for default settings)
    2.  `appsettings.{Environment}.json` (for environment-specific overrides)
    3.  Environment Variables
    4.  Command-line Arguments

2.  **Strongly-Typed Configuration**: Modules must use the Options pattern to map configuration sections to strongly-typed C# classes. This provides type safety and improves maintainability.

    *Example for a module's settings:*
    ```csharp
    public class ShippingOptions
    {
        public string ApiKey { get; set; }
        public int DefaultTimeoutSeconds { get; set; }
    }

    // In Program.cs or module startup:
    builder.Services.Configure<ShippingOptions>(builder.Configuration.GetSection("Shipping"));
    ```

### 9.3 Logging

1.  **Framework**: **Serilog** is the mandated logging framework for the entire application.
2.  **Standard Format**: All logs written to the console must be in a structured **JSON format** to be easily ingested by Azure Monitor. Each log entry must include a `CorrelationId`.
3.  **Correlation ID**: A middleware must be implemented to check for an incoming `X-Correlation-ID` header. If present, it is used; otherwise, a new `CorrelationId` is generated. This ID must be attached to the logging context and propagated through all subsequent internal operations and event publications for end-to-end tracing.

### 9.4 API Error Handling & Validation

1.  **Standard Error Response**: All API errors (e.g., 4xx, 5xx) must return a consistent JSON response body. A global exception-handling middleware will be responsible for catching exceptions and formatting this response.

    *Standard Error Model:*
    ```json
    {
      "traceId": "00-a1b2c3d4...",
      "statusCode": 400,
      "message": "One or more validation errors occurred.",
      "errors": {
        "FieldName": [ "Error message 1", "Error message 2" ]
      }
    }
    ```

2.  **Input Validation**: **FluentValidation** is the mandated library for validating all incoming API request DTOs. Validators should be automatically discovered and executed by a middleware or filter.

### 9.5 Resilience & Transient Fault Handling

1.  **Framework**: **Polly** is the mandated library for all outbound HTTP requests to external services (e.g., Tax, Shipping, Accounting APIs).
2.  **Standard Retry Policy**: A default "wait and retry" policy must be configured in a central location and applied to `HttpClient` instances used for external communication. This policy should be used unless a specific integration requires a different strategy.
    -   **Strategy**: Exponential backoff.
    -   **Retry Count**: 3 attempts.
3.  **Standard Circuit Breaker Policy**: A default circuit breaker policy should also be applied to prevent the system from repeatedly calling a failing service.
    -   **Strategy**: Break for 30 seconds after 5 consecutive failures.

### 9.6 Asynchronous Communication (Event Handling)

1.  **Idempotent Event Handlers**: As required by `event_schemas_and_message_contracts.md`, all event handlers **must** be idempotent. The following pattern is the standard for achieving this:
    -   Each module that consumes events must have an `Inbox` or `ProcessedMessages` table in its database schema. This table will store the `EventId` of every processed event.
    -   Within the event handler, the entire operation—checking for the `EventId` and executing the business logic—must be wrapped in a single database transaction.

    *Conceptual Handler Logic:*
    ```csharp
    public async Task Handle(OrderCreatedEvent @event)
    {
        await using var transaction = await _dbContext.BeginTransactionAsync();

        // 1. Check if event is already processed
        if (await _dbContext.ProcessedMessages.AnyAsync(m => m.Id == @event.Id))
        {
            // Already handled, just ignore
            return;
        }

        // 2. Perform business logic
        var newCustomer = new Customer(@event.CustomerId, @event.CustomerName);
        _dbContext.Customers.Add(newCustomer);

        // 3. Mark event as processed
        _dbContext.ProcessedMessages.Add(new ProcessedMessage(@event.Id));

        // 4. Commit the transaction
        await _dbContext.SaveChangesAsync();
        await transaction.CommitAsync();
    }
    ```

### 9.7 Security (Authentication & Authorization)

1.  **Endpoint Security**: All API endpoints must be secured by default. Anonymous access should be explicitly enabled only where absolutely necessary.
2.  **RBAC Enforcement**: Authorization must be enforced in code using `[Authorize]` attributes on controllers or individual endpoints. The `Roles` property must be populated with the roles specified in the `rbac_matrix_permissions.md` document.

    *Example for an endpoint only accessible by an Accountant:*
    ```csharp
    [ApiController]
    [Route("api/v1/accounting")]
    public class AccountingController : ControllerBase
    {
        [HttpPost("post-journal")]
        [Authorize(Roles = "Accountant")]
        public async Task<IActionResult> PostJournalEntry([FromBody] JournalDto entry)
        {
            // ... logic
        }
    }
    ```
