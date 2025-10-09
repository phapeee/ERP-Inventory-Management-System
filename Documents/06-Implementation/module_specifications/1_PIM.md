
## 1. Product Information Management (PIM)

### 1.1 Overview

The PIM module is the **central hub** for storing and governing all
product‑related data across the organisation. It provides a unified
repository for over 12 000 SKUs, including finished goods, kits/bundles,
variants and hazardous products. PIM supports multi‑channel publishing
(web store, Amazon, POS, B2B portal) and ensures that product data is
consistent, complete and up to date.

### 1.2 Functional capabilities

1. **Product catalogue management**
    - Maintain hierarchical categories (department → category →
        subcategory) and attributes for each product (e.g. dimensions,
        weight, unit of measure, hazardous classification).
    - Support variants (size, colour, packaging) via parent–child
        relationships.
    - Allow creation of **kits/bundles** that reference component
        products and calculate kit inventory availability from component
        stock levels.
    - Provide **rich descriptions**, images and digital assets for
        marketing channels.
2. **Pricing Management**
    - Support multiple pricing tiers, such as **retail and wholesale**.
    - Define price lists with effective dates and associate them with specific customer groups or channels.
    - Manage currency and unit of measure conversions for pricing.
3. **Data governance and validation**
    - Enforce **required attributes** for each product type (SKU,
        name, description, unit conversions, hazardous handling
        instructions). Required fields ensure completeness and accuracy
        when products are created or updated.
    - Validate units of measure and conversions (e.g. case → each) at
        the time of data entry to prevent inconsistent packaging
        definitions.
    - Maintain approval workflows: new products must be reviewed and
        approved by the E-Commerce Manager before publishing to sales
        channels.
    - Provide audit trails capturing who created, updated or approved
        each record.
3. **Channel syndication**
    - Support scheduled and ad‑hoc **export of product data** to
        external channels (web store, Amazon, B2B portal). Use the
        events and API layer to publish `products.product-created`,
        `products.product-price-changed`, and `products.product-discontinued` events to downstream
        systems.
    - Allow mapping of internal attribute names to channel‑specific
        fields. Channel connectors transform and transmit data using
        each platform’s API.
    - Provide status tracking of each export job (success, error) and
        error remediation capabilities.
4. **Hazardous product management**
    - Identify products with hazardous classifications (DOT/IATA
        classes) and flag them accordingly.
    - Store required regulatory data such as **UN/NA numbers**, hazard
        classes, packaging instructions and hazard statements.
    - Prevent publication to channels that do not support hazmat items
        (e.g. certain carriers or Amazon categories).
5. **Product Search**
    - Provide a search capability to find products by keyword, SKU, or category.
    - Support filtering and sorting of search results.

### 1.3 Data entities

- **Product** – primary entity containing master attributes (ID, SKU,
    name, description, category, manufacturer, brand, status,
    dimensions, weight, hazardous flag, productType). Contains
    `ValidFrom` and `ValidTo` fields for versioning.
- **ProductAttribute** – key–value table for extensible attributes
    (e.g. colour, size, material).
- **KitComponent** – mapping table linking kit parents to component
    products and their quantities. This table includes a `Version` number and `EffectiveDate` to track changes to the BOM over time, as specified in the acceptance criteria.
- **ChannelMapping** – stores mapping definitions between internal
    product fields and external channel fields.
- **ProductRevision** – captures historical versions of product
    records for auditing and rollback.

### 1.4 Process flows

1. **Create Product**
    1. User (E-Commerce Manager) enters product
        details via UI or API (`POST /api/v1/products`).
    2. System validates required fields and unit conversions. If
        validations fail, returns 400 with error details.
    3. Upon submission, record status is set to *Draft*. Audit fields
        `CreatedBy` and `CreatedAt` are captured.
    4. E-Commerce Manager receives a
        **ProductApprovalTask**. They review and approve; once approved, status changes to *Active*.
    5. System publishes a `products.product-created` event to
        the in-process event bus.
    6. Channel connectors detect the event and push product data to
        external systems.
2. **Update Product**
    1. User (E-Commerce Manager) modifies an existing product's
        details via UI or API (`PUT /api/v1/products/{productId}`).
    2. System validates the changes. If validations fail, returns 400 with error details.
    3. Upon submission, a new `ProductRevision` is created. Audit fields `UpdatedBy` and `UpdatedAt` are captured.
    4. If the price is changed, the system publishes a `products.product-price-changed` event.
    5. Channel connectors detect the event and push updated product data to external systems.
3. **Publish to channel**
    1. Scheduled job or user triggers an export.
    2. System gathers active products, transforms fields via
        ChannelMapping and calls external channel APIs.
    3. On success, sets `LastExportedAt` timestamp; on failure, logs
        error and notifies administrators.

### 1.5 API and integration

- **Inter-module Communication:** This module communicates with other internal modules exclusively by publishing and subscribing to domain events on the **in-process event bus**. Direct method calls between modules are not permitted. Key events published include `products.product-created`, `products.product-price-changed`, and `products.product-discontinued`.
- **Public API:** Functionality is exposed to external clients via the application's single, unified REST API, as defined in the `Api_specifications_and_endpoint_inventory.md` document.
- **External Integrations:** This module integrates with external systems (like the Web Store, Amazon, and B2B portals) via dedicated **adapters** (connectors). These adapters are responsible for calling external APIs or for publishing/subscribing to integration events on an external message broker.

### 1.6 Roles and permissions

- **E‑Commerce Manager (Full control):** Full control over PIM, including creating, updating, and approving products for channel publication and managing digital assets.
- **IT Administrator (Full control):** Full control for administrative purposes.
- **Read-Only Access:** The following roles have read-only access to product information for reference: Owner/GM, Operations Manager, Purchasing Lead, CSR, B2B Account Manager, Accountant, and Auditor.
- **No Access:** The Warehouse Associate has no access to the PIM module.

### 1.7 Error handling

- **Validation errors** (missing mandatory fields, invalid unit
    conversions) return 400 with detailed error messages.
- **Duplicate SKU** returns 409 Conflict.
- **Channel export failure** logs error with correlation ID and sends
    a notification. Retries are attempted according to exponential
    backoff.

### 1.8 Performance and scalability

- The system must support **thousands of concurrent product updates**
    with minimal latency. Publishing to channels should handle batch
    sizes of up to 10 000 products.
- Product updates must be propagated to all sales channels within **15 minutes**.
- Use asynchronous processing for exports via background workers and
    message queue to avoid blocking UI transactions.
- Indexes on SKU, status and channel mapping fields ensure fast
    queries.

### 1.9 Non‑functional considerations

- **Security:** Enforce role-based access control. Sensitive fields
    (e.g. cost price) should be masked except for authorised roles.
- **Regulatory compliance:** Follow hazardous materials regulations;
    restrict editing of hazmat data to authorised personnel. Keep
    historical records for regulatory audits.
- **Audit:** Every change to product data must be logged with user,
    timestamp and before/after values.
