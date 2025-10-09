
## 2. Inventory & Warehouse Management

### 2.1 Overview

This module governs the storage, tracking and control of physical
inventory across all warehouses, cross‑docks and 3PL facilities. It must
maintain **real‑time stock levels** and ensure that physical counts
match system records. The system supports bin/zone management,
RF‑enabled picking and cycle counts to achieve ≥ 98 % inventory
accuracy. Inventory management also
monitors reorder points, manages lot/expiry and serial numbers, and
integrates with purchasing and order fulfilment workflows.

### 2.2 Functional capabilities

1. **Multi‑warehouse & location management**
    - Define physical warehouses, distribution centers and
        cross‑docks. Each facility has zones, aisles and bins. Include
        location attributes (temperature control, hazardous storage).
    - Implement **slotting strategies** to optimize storage and picking efficiency. This includes slotting items based on their velocity (ABC analysis), size, weight, and hazardous material classification. The system should suggest optimal bin locations for new products and re-slotting for existing products based on changing demand patterns.
    - Support **3PL integration** for remote warehouses; maintain
        separate location codes but unify inventory view across internal
        and 3PL sites.
    - Transfer stock between locations with appropriate paperwork and
        inventory adjustments.
2. **Inventory transactions**
    - Track all stock movements: receipts, put‑aways, picks,
        transfers, adjustments, cycle counts, returns and disposals.
    - Each transaction records `TransactionType`, `Quantity`,
        `LotNumber` (if applicable), `SerialNumber` (if applicable),
        `FromLocation`, `ToLocation`, `CreatedBy` and timestamps.
3. **Cycle counting and accuracy**
    - Configure **ABC cycle counting**: high‑value or fast‑moving
        items counted more frequently. Generate cycle count tasks
        automatically.
    - Provide RF scanning support for counting. After counts, variance
        is calculated and an adjustment transaction is created if
        needed.
    - Report **inventory accuracy** as the ratio of correct counts to
        total counts.
4. **Lot, expiry and serial management**
    - Capture lot numbers and expiration dates at receiving. Require
        this information for regulated products as per primary
        requirements.
    - Assign serial numbers to high‑value tools; link serial numbers
        to warranty or service records.
    - Implement **`FirstExpiryFirstOut`** (FEFO) picking for items
        with expiry dates.
5. **Reorder point and safety stock**
    - Calculate reorder points based on historical demand, lead times
        and safety stock policies. When on‑hand falls below the reorder
        point, generate purchase recommendations.
    - Provide dashboards showing days‑of‑supply, safety stock levels
        and pending POs.
6. **Inventory valuation**
    - Maintain cost layers (FIFO, LIFO or weighted average). Costing
        method is configurable per product.
    - Update cost of goods sold (COGS) when inventory is issued to
        orders or scrapped.
7. **ATP Recalculation**
    - The system must recalculate Available-to-Promise (ATP) quantities in real-time whenever an inventory-related event occurs. This includes order entry, stock receipts, and inventory adjustments. The ATP calculation should consider on-hand stock, reserved quantities, and incoming stock from purchase orders.

### 2.3 Data entities

- **Warehouse** – defines physical facility; includes address,
    timezone, capacity and flags for 3PL.
- **Location** – child of warehouse; includes zone, aisle, bin code,
    type (picking, bulk, returns, hazmat), capacity.
- **InventoryItem** – snapshot of quantity on hand by Product,
    Location, Lot, Serial (optional) and status (available, allocated,
    quarantined). Contains reorder point and safety stock fields.
- **InventoryTransaction** – logs every inventory movement; references
    product, lot/serial, quantity, source/destination, user, date and
    reason.
- **CycleCountTask** and **CycleCountResult** – tasks for scheduled
    counts and recorded results.

### 2.4 Process flows

1. **Receiving and put‑away**
    1. Create or import **ASN** (advanced shipping notice) from vendor
        or 3PL.
    2. Warehouse associate scans incoming cartons, captures lot/serial
        information and verifies against expected quantities.
    3. System creates a `Receipt` transaction, updates `InventoryItem`
        records, and publishes a `inventory.stock-received` event to the in-process
        event bus.
    4. Generate put‑away tasks to move stock from inbound staging to
        storage bins.
2. **Picking and shipping**
    1. When an order is allocated, a **pick list** is generated. Items
        are selected using FEFO or FIFO rules.
    2. Warehouse associate picks items using RF scanners; system
        validates picks by scanning barcodes.
    3. Upon completion, system records `Pick` transactions and updates
        `InventoryItem` balances; picks are associated with `Shipment`
        records in the orders module.
    4. If lot or serial numbers are required, they are captured during
        picking.
3. **Cycle counting**
    1. Scheduler generates daily/weekly cycle count tasks by item
        class.
    2. Associate counts items in designated bins and enters counts via
        RF device.
    3. System compares counted quantity to system quantity. If a variance
        exists, it creates an `Adjustment` transaction, updates
        inventory accuracy metrics, and publishes a `inventory.stock-adjusted` event.
4. **Stock Transfer**
    1. User initiates a stock transfer between two locations (e.g., from a bulk storage bin to a forward picking bin) via UI or API (`POST /api/v1/inventory/transfers`).
    2. The system validates that the source location has sufficient stock.
    3. Two `InventoryTransaction` records are created: one debiting the source location and one crediting the destination location.
    4. The `InventoryItem` quantities for both locations are updated.
    5. The system publishes an `inventory.stock-transferred` event to the in-process event bus.

### 2.5 API and integration

- **Inter-module Communication:** This module communicates with other internal modules exclusively by publishing and subscribing to domain events on the **in-process event bus**. Direct method calls between modules are not permitted. Key events published include `inventory.stock-received`, `inventory.stock-adjusted`, and `inventory.stock-transferred`. It subscribes to events like `orders.order-created` to allocate inventory.
- **Public API:** Functionality is exposed to external clients via the application's single, unified REST API, as defined in the `api_specifications_and_endpoint_inventory.md` document.
- **External Integrations:** This module integrates with external systems (like a 3PL) via dedicated **adapters**. These adapters are responsible for calling external APIs or for publishing/subscribing to integration events on an external message broker (e.g., Azure Service Bus).

### 2.6 Roles and permissions

- **Operations Manager (Full control):** Full control over inventory and warehouse operations, including managing locations, creating and approving adjustments, and reviewing accuracy reports.
- **Warehouse Associate (Create/Edit):** Create inventory transactions (receipts, picks, transfers) and cycle count results.
- **IT Administrator (Full control):** Full control for administrative purposes.
- **Read-Only Access:** The following roles have read-only access: Owner/GM, Purchasing Lead, CSR, E-Comm Mgr, B2B Account Mgr, Accountant, and Auditor.

### 2.7 Error handling

- **Over‑pick/under‑pick**: If pick quantity exceeds available stock,
    system raises error; under‑picks prompt re-allocation or backorder.
- **Mismatch during receiving**: If received quantity differs from
    ASN, system flags discrepancy and requires supervisor approval.
- **Negative inventory prevention**: Transactions cannot cause
    negative on‑hand; system rejects transaction and logs event.

### 2.8 Performance and scalability

- **`RealTime` updates:** Inventory changes must propagate within
    &lt; 1 minute to ensure accurate availability across all modules.
- **3PL Integration:** Inventory updates from 3PL providers must be reflected within **15 minutes**.
- **High volume transactions:** System should handle thousands of RF
    scans per hour. Use batch processing for cycle count tasks.
- **Indexing:** Index `InventoryItem` on `ProductId`, `WarehouseId`,
    `LocationId` and `LotNumber` for fast lookups.

### 2.9 Non‑functional considerations

- **Reliability:** Use event sourcing or journalling to reconstruct
    inventory state from transactions in case of failures.
- **Security:** Limit write access to inventory transactions. Serial
    numbers and lot details must be traceable for recalls.
- **Compliance:** Track hazardous storage locations; ensure proper
    segregation of hazmat goods and maintain necessary documentation.
