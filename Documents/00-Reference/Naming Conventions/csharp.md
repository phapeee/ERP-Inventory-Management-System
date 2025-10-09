# Code Naming & Structure Guidelines

These conventions align with the ASP.NET Core + DDD modular monolith architecture documented in the C4 diagrams.

## Namespaces

```csharp
Company.Product.ModuleName.Layer
```

- `Company` → `PineConePro`
- `Product` → `Erp` (or `ErpIms` for clarity)
- `ModuleName` → e.g., `Orders`, `Inventory`, `ProductCatalog`, `Shipping`, `Returns`, `Finance`, `Analytics`
- `Layer` → `Api`, `Application`, `Domain`, `Infrastructure`, `Integration`

Example:

```csharp
PineConePro.Erp.Orders.Application.OrderApplicationService
PineConePro.Erp.Inventory.Domain.Aggregates.InventoryItemAggregate
```

## Project Layout

| Project                                 | Purpose                                                             |
| --------------------------------------- | ------------------------------------------------------------------- |
| `PineConePro.Erp.Orders.Api`            | ASP.NET Core controllers, request DTOs, API composition             |
| `PineConePro.Erp.Orders.Application`    | Application services, command/handler classes, validators, sagas    |
| `PineConePro.Erp.Orders.Domain`         | Aggregates, entities, value objects, domain policies, domain events |
| `PineConePro.Erp.Orders.Infrastructure` | EF Core repositories, projections, caches, configuration            |
| `PineConePro.Erp.Orders.Integration`    | External adapters (payment, inventory, shipping, tax)               |

Repeat pattern per module for modular isolation.

## Class Naming

- Controllers → `<Feature>Controller` (e.g., `OrderController`, `RmaController`)
- Application services → `<Feature>ApplicationService`
- Validators → `<Command/Dto>Validator`
- Sagas/workflows → `<Process>SagaHandler`, `<Process>Workflow`
- Aggregates → `<Concept>Aggregate`
- Policies → `<Decision>Policy`
- Repositories → `<Aggregate>Repository`
- Projections → `<ReadModel>Projector`, `<View>ProjectionHandler`
- Integration adapters → `<System>Client` or `<System>Facade`
- Event publisher/handlers → `<Domain>EventPublisher`, `<Domain>EventHandler`
- Background jobs → `<Process>Scheduler`, `<Process>Worker`
- Cache facades → `<Concept>Cache`

## File Organization

- One public class per file.
- Keep DTOs with their controllers or application layer.
- Group domain events under `Domain/Events/` with descriptive filenames (`OrderPaidEvent.cs`).

## Testing

- Unit tests project per module: `PineConePro.Erp.Orders.Tests.Unit`
- Integration tests project per module: `PineConePro.Erp.Orders.Tests.Integration`
- Naming: `<ClassUnderTest>Tests` or `<Scenario>Spec`

## Sequence Diagram Templates

Sequence diagrams are stored as PlantUML `.puml` files under `Documents/media/attachments/sequence_diagrams/`. Each file is prefixed with the module name and workflow (e.g., `orders_order_fulfillment_sequence.puml`).
