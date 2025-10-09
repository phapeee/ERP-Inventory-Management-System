Event Schemas & Message Contracts
=================================

Purpose & Scope
---------------

This document defines the schemas for the **domain events** used within the PineCone Pro Supplies ERP/IMS. In accordance with the system's event-driven modular monolith architecture (ADR-012), an **in-process event bus** is the exclusive method for communication between internal modules. This ensures maximum decoupling and allows modules to evolve independently.

The scope of this document includes:

- The design principles for domain events, aligned with Domain-Driven Design (DDD) and Clean Architecture (ADR-016).
- Conventions for naming, versioning, and structuring events.
- A standardized event envelope containing metadata for routing and observability.
- A catalog of domain-specific event schemas for all modules.

This document focuses on the **internal, asynchronous communication** that orchestrates workflows within the application. It complements the `api_specifications_and_endpoint_inventory.md`, which defines the external synchronous REST API.

Architectural Context
---------------------

The ERP/IMS is built as a **modular monolith** that follows **Clean Architecture**. The core business logic is encapsulated in the Domain and Application layers.

- **Domain Events:** When an action in the Application layer results in a state change to a Domain Aggregate (e.g., an order is created), the aggregate raises a domain event. These events are simple data structures that represent a fact that has occurred in the past.
- **In-Process Event Bus:** The Application layer dispatches these events to an in-process event bus after the original transaction is successfully committed.
- **Event Handlers:** Other modules subscribe to these events. An event handler (located in the Application layer of the subscribing module) processes the event, typically by executing a command that updates its own aggregates.

This event-driven approach enables **eventual consistency** and loose coupling between modules. Direct, in-process function calls between modules are disallowed.

For integration with **external systems**, a separate mechanism is used. Specific services may listen for internal domain events and publish a corresponding **integration event** to an external message broker like **Azure Service Bus** (ADR-008). The structure of those external events should follow the principles laid out here but are outside the primary scope of this document.

Event Design Guidelines & Conventions
-------------------------------------

### Domain Events vs. Commands

- **Domain Events:** Represent something that **has already happened** in the business domain. They are immutable facts named in the past tense (e.g., `OrderCreated`). In Clean Architecture, domain events are raised by Aggregates within the Domain Layer.
- **Commands:** Represent an intent to perform an action. They are named in the imperative tense (e.g., `CreateOrder`). Commands are received by the Application Layer and executed on an Aggregate, which may then raise one or more domain events.

### Naming Conventions

To ensure clarity and consistency, events are named using conventions inspired by Domain-Driven Design and the CloudEvents specification:

- **Namespace:** `{module}.{event-name}` (e.g., `orders.order-created`). This provides a unique, routable identifier.
- **Format:** Use lowercase letters and hyphens for the event name.
- **Tense:** Use a past-tense verb to reflect that the event is a historical fact (e.g., `product-price-changed`).
- **Specificity:** Avoid generic verbs like `updated`. Be specific about what changed (e.g., `product-price-changed` is better than `product-updated`).
- **Versioning:** If a breaking change is required, create a new event version and include it in the type (e.g., `order-created.v2`).

### Event Envelope & Metadata

All domain events use a standard envelope containing two parts: `metadata` for routing and context, and `data` for the event-specific payload.

| Field         | Description & Guidance                                         |
| ------------- | -------------------------------------------------------------- |
| eventId       | A unique UUID for the event instance.                          |
| eventType     | The fully qualified event name (e.g., `orders.order-created`). |
| eventVersion  | The semantic version of the event schema (e.g., "1.0").        |
| source        | The module that published the event (e.g., "OrdersModule").    |
| timestamp     | The UTC timestamp (ISO 8601) of when the event occurred.       |
| correlationId | An ID to trace a business process across multiple events.      |
| userId        | (Optional) The ID of the user who initiated the action.        |

#### Example Envelope

```json
{
  "metadata": {
    "eventId": "4f81c9b0-9e1d-11ed-93f3-5bd98734f1ab",
    "eventType": "orders.order-created",
    "eventVersion": "1.0",
    "source": "OrdersModule",
    "timestamp": "2025-09-27T14:32:15Z",
    "correlationId": "ccd5bf2c-5e32-4f11-8a22-0a1cbbf2c7e4",
    "userId": "U123456"
  },
  "data": {
    "orderId": 100123,
    "customerId": "C1000",
    "orderDate": "2025-09-27",
    "totalAmount": 36.99,
    "items": [
      {"productId": 20045, "quantity": 2, "unitPrice": 10.99},
      {"productId": 20067, "quantity": 1, "unitPrice": 25.00}
    ]
  }
}
```

### Autonomy & Shared Contracts

To maintain module autonomy, **domain event contracts are not shared in a common library**. Each module defines the events it consumes as simple, local data structures. This prevents a change in one module from forcing a recompilation of another. The in-process event bus serializes and deserializes events, so the producer and consumer only need to agree on the JSON structure.

### Idempotence

Event handlers MUST be idempotent. The combination of `eventId` and `source` can be used to detect and discard duplicate events, as the event bus may deliver an event more than once in certain failure scenarios.

Domain-Specific Events
----------------------

The following sections list the key domain events for each module. Payloads use camelCase and contain only primitive types or simple data objects.

### 1. Product Module

| Event Type                       | Description                                   | Payload Fields                                        |
| -------------------------------- | --------------------------------------------- | ----------------------------------------------------- |
| `products.product-created`       | A new product has been defined in the system. | `productId`, `sku`, `name`, `categoryId`, `listPrice` |
| `products.product-price-changed` | The list price of a product has been updated. | `productId`, `oldPrice`, `newPrice`, `effectiveDate`  |
| `products.product-discontinued`  | A product has been marked as discontinued.    | `productId`, `discontinuedDate`                       |

### 2. Inventory Module

| Event Type                    | Description                                             | Payload Fields                                                    |
| ----------------------------- | ------------------------------------------------------- | ----------------------------------------------------------------- |
| `inventory.stock-received`    | Stock for a product has been received into a warehouse. | `productId`, `warehouseId`, `quantityReceived`, `lotNumber` (opt) |
| `inventory.stock-adjusted`    | An inventory adjustment was made.                       | `productId`, `warehouseId`, `adjustmentQuantity`, `reason`        |
| `inventory.stock-transferred` | Stock was transferred between two locations.            | `productId`, `fromWarehouseId`, `toWarehouseId`, `quantity`       |

### 3. Order Module

| Event Type                 | Description                                          | Payload Fields                                                       |
| -------------------------- | ---------------------------------------------------- | -------------------------------------------------------------------- |
| `orders.order-created`     | A new order has been successfully placed.            | `orderId`, `customerId`, `orderDate`, `totalAmount`, `items` (array) |
| `orders.order-paid`        | Payment for an order has been successfully captured. | `orderId`, `paymentId`, `amountPaid`                                 |
| `orders.order-shipped`     | An order has been fully shipped.                     | `orderId`, `shipmentId`, `shippingDate`, `carrier`, `trackingNumber` |
| `orders.order-cancelled`   | An order has been cancelled.                         | `orderId`, `cancellationReason`                                      |
| `orders.return-completed`  | A customer return has been fully processed.          | `rmaId`, `orderId`, `status`                                         |

### 4. Purchasing Module

| Event Type                           | Description                                           | Payload Fields                                                           |
| ------------------------------------ | ----------------------------------------------------- | ------------------------------------------------------------------------ |
| `purchasing.purchase-order-created`  | A new purchase order has been created.                | `poId`, `vendorId`, `orderDate`, `expectedDeliveryDate`, `items` (array) |
| `purchasing.purchase-order-approved` | A purchase order has been approved.                   | `poId`, `approvedBy`, `approvalDate`                                     |
| `purchasing.purchase-order-received` | Goods for a PO have been fully or partially received. | `poId`, `receiptId`, `dateReceived`, `itemsReceived` (array)             |

### 5. Shipping Module

| Event Type                    | Description                                       | Payload Fields                                                   |
| ----------------------------- | ------------------------------------------------- | ---------------------------------------------------------------- |
| `shipping.shipment-created`   | A shipment has been created for an order.         | `shipmentId`, `orderId`, `carrier`, `trackingNumber`, `labelUrl` |
| `shipping.shipment-delivered` | A carrier has confirmed a shipment was delivered. | `shipmentId`, `trackingNumber`, `deliveryDate`                   |

Conclusion
----------

This document establishes a standardized, DDD-aligned approach for defining and using domain events within the PineCone Pro ERP/IMS. By adhering to these conventions—clear naming, a standard envelope, and a strict separation from commands—the system achieves reliable, traceable, and scalable event-driven communication between its internal modules. This architecture ensures that the application remains loosely coupled and maintainable as it evolves.
