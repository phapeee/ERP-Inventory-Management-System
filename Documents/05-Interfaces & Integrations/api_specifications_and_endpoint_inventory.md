API Specifications & Endpoint Inventory
=======================================

Purpose and Scope
-----------------

This document defines the **publicly exposed HTTP/REST interface** for
the ERP/IMS platform, which is built as a **modular monolith** in
accordance with the architecture decision records (ADR-012, ADR-016). It
inventories each API endpoint, grouped by its corresponding domain
module, and describes its request/response semantics.

The key goals of this document are to:

- Provide a clear and unified API reference for client developers
    (e.g., the Angular front-end team).
- Guide external partners and third-party integrators on how to
    interact with the platform.
- Ensure the API design is consistent with the principles of Clean
    Architecture and Domain-Driven Design.

This document covers the public API for all modules, including Product,
Inventory, Orders, and others. It also outlines conventions for
versioning, authentication, authorization, pagination, filtering, and
error handling.

It does **not** cover internal, in-process communication between
modules, which is handled exclusively by an event bus (see ADR-012). The
API defined here is the sole entry point for external clients.

Design Principles and Standards
-------------------------------

### RESTful Guidelines

The API follows REST principles and adopts the design guidelines
recommended by the Azure Architecture Center. It is platform-independent,
loosely coupled, and uses standard HTTP verbs (`GET`, `POST`, `PUT`,
`PATCH`, `DELETE`) to operate on resources. The API is **stateless**,
meaning each request from a client contains all the information needed
to complete the operation.

### Resource URIs and Naming Conventions

Resources are organized around business entities as defined in our
Domain-Driven Design. URIs use **plural nouns** for collections (e.g.,
`/products`, `/orders`). Nested URIs are used sparingly, primarily for
resources that have a clear parent-child relationship (e.g.,
`/orders/{orderId}/items`).

### Versioning

The platform uses **URI versioning** to indicate breaking changes. The
major version is prefixed to the base path (e.g., `/api/v1/products`).
Non-breaking changes (like adding an optional field) do not require a
version increment.

### Pagination, Filtering, and Sorting

Collection endpoints implement **pagination** using `page` and
`pageSize` query parameters. Sorting is handled by a `sort` parameter,
and filtering uses query parameters specific to the resource (e.g.,
`status=shipped`).

### Data Format

All endpoints accept and return **JSON** encoded as UTF-8
(`Content-Type: application/json; charset=utf-8`).

### Authentication and Authorization

The API is secured with **OAuth2 bearer tokens** issued by
**Microsoft Entra ID**. Every request must include an
`Authorization: Bearer <token>` header. The application validates the
token and enforces **role-based access control (RBAC)** as defined in
the RBAC matrix.

### Error Handling & Status Codes

Endpoints adhere to standard HTTP status semantics. Errors include a
JSON body with an error code, message, and correlation ID.

- **200 OK:** Success for `GET`, `PUT`, `PATCH`.
- **201 Created:** Success for `POST` (resource creation).
- **204 No Content:** Success for operations that don't return a body
    (e.g., `DELETE`).
- **400 Bad Request:** Invalid request payload.
- **401 Unauthorized:** Missing or invalid authentication token.
- **403 Forbidden:** Authenticated user lacks permission.
- **404 Not Found:** Resource does not exist.
- **409 Conflict:** The request could not be completed due to a conflict
    with the current state of the resource.

Endpoint Inventory
------------------

The following tables summarize the endpoints for each functional module.
These endpoints form a single, unified API surface for the entire
application.

### Product Module

| Method & Path                       | Description                                            | Parameters / Body                                         | Response (simplified)            | Auth          |
| ----------------------------------- | ------------------------------------------------------ | --------------------------------------------------------- | -------------------------------- | ------------- |
| GET /api/v1/products                | List products with pagination, sorting, and filtering. | Query: `page`, `pageSize`, `sort`, `search`, `categoryId` | Array of product summary DTOs.   | Authenticated |
| POST /api/v1/products               | Create a new product or kit/bundle.                    | `CreateProductRequest` DTO.                               | 201 Created with `Product` DTO.  | ProductAdmin  |
| GET /api/v1/products/{productId}    | Retrieve a product’s details.                          | Path: `productId`                                         | `Product` DTO.                   | Authenticated |
| PUT /api/v1/products/{productId}    | Update an existing product.                            | `UpdateProductRequest` DTO.                               | `Product` DTO or 204 No Content. | ProductAdmin  |
| DELETE /api/v1/products/{productId} | Soft delete or archive a product.                      | Path: `productId`                                         | 204 No Content.                  | ProductAdmin  |
| GET /api/v1/products/search         | Search products by keyword.                            | Query: `q` (string)                                       | Array of `Product` DTOs.         | Authenticated |

### Inventory & Warehouse Module

| Method & Path                                | Description                                       | Parameters / Body                                             | Response (simplified)              | Auth             |
| -------------------------------------------- | ------------------------------------------------- | ------------------------------------------------------------- | ---------------------------------- | ---------------- |
| GET /api/v1/inventory/locations              | List warehouses, cross-docks, and 3PL facilities. | Query: `type`                                                 | Array of `Location` DTOs.          | InventoryViewer  |
| GET /api/v1/inventory/items                  | List inventory items across all locations.        | Query: `productId`, `locationId`, `lotNumber`, `serialNumber` | Array of `InventoryItem` DTOs.     | InventoryViewer  |
| POST /api/v1/inventory/receipts              | Record a receipt of goods (ASN or manual).        | `ReceiveGoodsRequest` DTO.                                    | 201 Created with `Receipt` DTO.    | InventoryManager |
| POST /api/v1/inventory/transfers             | Transfer stock between locations.                 | `TransferStockRequest` DTO.                                   | 201 Created with `Transfer` DTO.   | InventoryManager |
| POST /api/v1/inventory/cycle-counts          | Initiate a cycle count for a location.            | `StartCycleCountRequest` DTO.                                 | 201 Created with `CycleCount` DTO. | InventoryManager |
| GET /api/v1/inventory/cycle-counts/{countId} | Retrieve cycle count results and variances.       | Path: `countId`                                               | `CycleCount` DTO.                  | InventoryManager |
| GET /api/v1/inventory/lots                   | List lot numbers and expiration dates.            | Query: `productId`, `locationId`                              | Array of `Lot` DTOs.               | InventoryViewer  |
| GET /api/v1/inventory/serials                | List serialized items.                            | Query: `productId`, `locationId`                              | Array of `Serial` DTOs.            | InventoryViewer  |

### Order Management Module

| Method & Path                              | Description                                          | Parameters / Body                                 | Response (simplified)          | Auth                            |
| ------------------------------------------ | ---------------------------------------------------- | ------------------------------------------------- | ------------------------------ | ------------------------------- |
| GET /api/v1/orders                         | List orders with pagination and filtering.           | Query: `page`, `pageSize`, `status`, `customerId` | Array of `OrderSummary` DTOs.  | OrderViewer                     |
| POST /api/v1/orders                        | Create a new order.                                  | `CreateOrderRequest` DTO.                         | 201 Created with `Order` DTO.  | Authenticated                   |
| GET /api/v1/orders/{orderId}               | Retrieve order details.                              | Path: `orderId`                                   | `Order` DTO.                   | OrderViewer or Customer (owner) |
| PUT /api/v1/orders/{orderId}               | Update order details prior to fulfillment.           | `UpdateOrderRequest` DTO.                         | `Order` DTO.                   | CSR                             |
| POST /api/v1/orders/{orderId}/cancel       | Cancel an order prior to shipment.                   | Path: `orderId`; Body: `CancelOrderRequest` DTO.  | `Order` DTO.                   | CSR or Customer                 |
| POST /api/v1/orders/{orderId}/payments     | Capture a payment for an order.                      | `CapturePaymentRequest` DTO.                      | `Payment` DTO.                 | OrderManager                    |
| GET /api/v1/orders/{orderId}/shipments     | Retrieve shipments for an order.                     | Path: `orderId`                                   | Array of `Shipment` DTOs.      | OrderViewer                     |
| POST /api/v1/returns                       | Initiate a return (RMA) for one or more order lines. | `CreateReturnRequest` DTO.                        | 201 Created with `Return` DTO. | CSR or Customer                 |
| GET /api/v1/returns/{returnId}             | Retrieve RMA details.                                | Path: `returnId`                                  | `Return` DTO.                  | CSR or ReturnsViewer            |
| PUT /api/v1/returns/{returnId}/disposition | Update the disposition of returned items.            | `UpdateDispositionRequest` DTO.                   | `Return` DTO.                  | ReturnsManager                  |

### Purchasing & Vendor Module

| Method & Path                               | Description                             | Parameters / Body                               | Response (simplified)                 | Auth               |
| ------------------------------------------- | --------------------------------------- | ----------------------------------------------- | ------------------------------------- | ------------------ |
| GET /api/v1/purchase-orders                 | List purchase orders.                   | Query: `page`, `pageSize`, `status`, `vendorId` | Array of `PurchaseOrder` DTOs.        | PurchasingViewer   |
| POST /api/v1/purchase-orders                | Create a new purchase order.            | `CreatePurchaseOrderRequest` DTO.               | 201 Created with `PurchaseOrder` DTO. | PurchasingManager  |
| GET /api/v1/purchase-orders/{poId}          | Retrieve purchase order details.        | Path: `poId`                                    | `PurchaseOrder` DTO.                  | PurchasingViewer   |
| PUT /api/v1/purchase-orders/{poId}          | Update a purchase order.                | `UpdatePurchaseOrderRequest` DTO.               | `PurchaseOrder` DTO.                  | PurchasingManager  |
| POST /api/v1/purchase-orders/{poId}/approve | Approve or reject a purchase order.     | `ApprovePurchaseOrderRequest` DTO.              | `PurchaseOrder` DTO.                  | PurchasingApprover |
| GET /api/v1/vendors                         | List vendors.                           | Query: `page`, `pageSize`, `search`             | Array of `Vendor` DTOs.               | PurchasingViewer   |
| POST /api/v1/vendors                        | Create a new vendor.                    | `CreateVendorRequest` DTO.                      | 201 Created with `Vendor` DTO.        | PurchasingManager  |
| GET /api/v1/vendors/{vendorId}              | Retrieve vendor details and scorecards. | Path: `vendorId`                                | `Vendor` DTO.                         | PurchasingViewer   |
| PUT /api/v1/vendors/{vendorId}              | Update vendor information.              | `UpdateVendorRequest` DTO.                      | `Vendor` DTO.                         | PurchasingManager  |

### Shipping & Logistics Module

| Method & Path                               | Description                             | Parameters / Body           | Response (simplified)            | Auth            |
| ------------------------------------------- | --------------------------------------- | --------------------------- | -------------------------------- | --------------- |
| GET /api/v1/shipping/carriers               | List supported shipping carriers.       | —                           | Array of `Carrier` DTOs.         | ShippingViewer  |
| POST /api/v1/shipping/rates                 | Compare shipping rates for a parcel.    | `GetRatesRequest` DTO.      | Array of `RateOption` DTOs.      | ShippingViewer  |
| POST /api/v1/shipping/labels                | Purchase a shipping label for an order. | `PurchaseLabelRequest` DTO. | 201 Created with `Shipment` DTO. | ShippingManager |
| GET /api/v1/shipping/track/{trackingNumber} | Track a shipment’s current status.      | Path: `trackingNumber`      | `TrackingStatus` DTO.            | ShippingViewer  |

### Tax & Accounting Module

| Method & Path                | Description                            | Parameters / Body                                 | Response (simplified)           | Auth         |
| ---------------------------- | -------------------------------------- | ------------------------------------------------- | ------------------------------- | ------------ |
| POST /api/v1/taxes/calculate | Calculate tax for an order or invoice. | `CalculateTaxRequest` DTO.                        | `TaxCalculation` DTO.           | OrderManager |
| GET /api/v1/gl-entries       | List general ledger entries.           | Query: `page`, `pageSize`, `startDate`, `endDate` | Array of `GlEntry` DTOs.        | Accountant   |
| POST /api/v1/gl-entries      | Create a manual journal entry.         | `CreateJournalEntryRequest` DTO.                  | 201 Created with `GlEntry` DTO. | Accountant   |
| GET /api/v1/accounts         | Retrieve the chart of accounts.        | —                                                 | Array of `Account` DTOs.        | Accountant   |

### Analytics & Reporting Module

| Method & Path                            | Description                           | Parameters / Body                 | Response (simplified)            | Auth             |
| ---------------------------------------- | ------------------------------------- | --------------------------------- | -------------------------------- | ---------------- |
| GET /api/v1/analytics/dashboard          | Returns KPIs and summary metrics.     | Query: `period`, `metric`         | `DashboardMetrics` DTO.          | ExecutiveViewer  |
| GET /api/v1/analytics/reports/{reportId} | Download a prebuilt report (PDF/CSV). | Path: `reportId`; Query: `format` | Binary or base64 encoded report. | Authorized roles |

### External Integration Endpoints

The application integrates with external services through dedicated
**integration adapters** located within the relevant domain modules.
These are not separate services.

- **Payment Integration (within Order Module):**
  - `POST /api/v1/payments`: Processes a payment via the configured
        payment gateway.
- **Shipping Carrier Integration (within Shipping Module):**
  - The Shipping module contains adapters that communicate with
        external carrier APIs (e.g., UPS, FedEx) to fetch rates and
        purchase labels. These are not exposed as public endpoints.
- **Tax Calculation Integration (within Tax Module):**
  - The Tax module contains an adapter that calls an external tax
        service to get accurate rates. This is triggered by the
        `/api/v1/taxes/calculate` endpoint.

Conclusion
----------

This document provides a unified view of the ERP/IMS public API,
designed to align with our modular monolith architecture. By grouping
endpoints by domain module and adhering to RESTful conventions, the API
provides a clear and consistent interface for all external clients. The
design emphasizes a strong separation between the public interface and
the internal, event-driven workings of the application, in keeping with
the principles of Clean Architecture and Domain-Driven Design.
