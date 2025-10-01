Data Model & ERD
================

Purpose & Scope
---------------

This document defines the relational data model for the PineCone Pro
Supplies ERP/IMS. It supports product information management,
multi‑warehouse inventory control, order processing, purchasing,
lot/serial tracking, shipping, returns and basic accounting integration.
The aim is to create a normalized and scalable database that ensures
data integrity, reduces redundancy and serves as the single source of
truth for all departments.

Design Principles
-----------------

- **Identify entities and relationships:** The first step is to
    enumerate the main business objects (e.g., products, customers,
    orders, inventory) and how they interact.
- **Normalize the schema:** Entities are normalized to reduce
    redundancy and maintain data integrity. Reference tables (such as
    categories and tax rates) are separated from transactional tables.
- **Choose appropriate data types:** Fields use appropriate datatypes
    (e.g., integers for IDs, decimals for prices) to optimise storage
    and accuracy.
- **Define primary and foreign keys:** Each table has a surrogate
    primary key; relationships are enforced via foreign keys.
- **Represent cardinality:** One‑to‑many relationships (e.g., orders
    to order lines) and many‑to‑many relationships (e.g., products and
    warehouses) are modelled explicitly.
- **Support scalability:** The design accommodates multiple
    warehouses, thousands of SKUs and additional modules (e.g.,
    forecasting) without major restructuring.

Entities & Relationships Overview
---------------------------------

Below is a high‑level summary of the core entities and their
relationships. Details of each table follow in the Schema Catalog
section.

-   **Product** – Master record for each SKU. A product has one category.
-   **ProductCategory** – A category can have many products.
-   **Vendor** – Represents suppliers. A vendor can have many purchase orders.
-   **Customer** – Represents B2B/B2C customers. A customer can have many orders and many RMAs.
-   **PurchaseOrder** – Header for purchase transactions with vendors. A purchase order has one vendor and at least one line item.
-   **PurchaseOrderLine** – Line items of a purchase order. A line item has one product.
-   **InventoryLocation** – Warehouse or cross‑dock facility. A location can have many inventory items and many shipments.
-   **InventoryItem** – Junction entity connecting Product and InventoryLocation.
-   **Order** – Customer order. An order has one customer and at least one line item. An order can have many shipments and many RMAs.
-   **OrderLine** – Line items for each order. A line item has one product and one tax rate.
-   **Shipment** – Physical shipment associated with an order. A shipment has one order and one location.
-   **RMA** – Return merchandise authorization. An RMA has one order, one customer, and optionally one shipment.
-   **Lot** – Represents lots/batches for regulated products. An inventory item can have one lot.
-   **Serial** – Unique serial numbers for high‑value items. An inventory item can have one serial number.
-   **TaxRate** – Stores jurisdictional tax rates. A tax rate can be applied to many order lines.
-   **AccountEntry** – Represents financial journal entries.
-   **User / Role** – A user has one role, and a role can have many users. A user can create many purchase orders and orders.

ERD Diagram
-----------

Below is the conceptual ERD showing the core entities and their
relationships.

<img src="/home/ubuntu/ERP-Inventory-Management-System/Documents/Markdown/attachments/data_model_erd/media/image1.png" style="width:7.5in;height:9.56042in" alt="A screenshot of a computer screen AI-generated content may be incorrect." />

ERP ERD

*Figure 1: Conceptual ERD for PineCone Pro ERP/IMS. Rectangles represent
entities; connecting lines indicate one‑to‑many relationships.
Many‑to‑many relationships (e.g., Product–Warehouse) are resolved by
junction tables such as InventoryItem.*

Field & Schema Catalog
----------------------

The following tables describe the fields, primary/foreign keys and notes
for each entity. Data types are indicative and can be adjusted to match
the chosen database platform (Azure SQL, PostgreSQL, etc.). Dates and
times should use timezone‑aware types where possible.

### Product

| Field | Type | Description | Key / Notes | Constraints |
| :--- | :--- | :--- | :--- | :--- |
| ProductID | INTEGER | Surrogate primary key | PK | NOT NULL, AUTO_INCREMENT |
| SKU | VARCHAR(100) | Internal product code (SKU); unique | Unique | NOT NULL |
| Barcode | VARCHAR(100) | Universal barcode/UPC | | |
| Name | VARCHAR(100) | Product name | | NOT NULL |
| Description | VARCHAR(2000) | Long description | | |
| CategoryID | INTEGER | Foreign key to ProductCategory table | FK | NOT NULL |
| PackedWeight | DECIMAL(10,2) | Weight incl. packaging | | |
| PackedHeight | DECIMAL(10,2) | Height incl. packaging | | |
| PackedWidth | DECIMAL(10,2) | Width incl. packaging | | |
| PackedDepth | DECIMAL(10,2) | Depth incl. packaging | | |
| Refrigerated | BOOLEAN | Requires refrigeration | | NOT NULL, DEFAULT FALSE |
| HazardClass | VARCHAR(50) | Hazardous classification (e.g., ORM‑D, flammable) | | |
| BaseUnit | VARCHAR(20) | Base unit of measure (e.g., piece, gallon) | | NOT NULL |
| ConversionFactor | DECIMAL(10,4) | Factor for converting base unit to alternative units (for unit conversions) | | |
| ReorderQuantity | INTEGER | Preferred reorder quantity | | |

### ProductCategory

| Field | Type | Description | Key / Notes | Constraints |
| :--- | :--- | :--- | :--- | :--- |
| CategoryID | INTEGER | Primary key | PK | NOT NULL, AUTO_INCREMENT |
| Name | VARCHAR(100) | Category name | | NOT NULL |
| ParentID | INTEGER | Self‑reference for hierarchical categories | FK | |

### Vendor

| Field | Type | Description | Key / Notes | Constraints |
| :--- | :--- | :--- | :--- | :--- |
| VendorID | INTEGER | Primary key | PK | NOT NULL, AUTO_INCREMENT |
| Name | VARCHAR(100) | Legal name | | NOT NULL |
| Address | VARCHAR(200) | Mailing address | | |
| ContactName | VARCHAR(100) | Primary contact person | | |
| Email | VARCHAR(100) | Email address | | |
| Phone | VARCHAR(40) | Phone number | | |
| MinOrderQty | INTEGER | Minimum order quantity for this vendor | | |
| LeadTimeDays | INTEGER | Standard lead time (days) | | |
| Rating | DECIMAL(4,2) | Vendor performance score (0–5) | | |

### Customer

| Field | Type | Description | Key / Notes | Constraints |
| :--- | :--- | :--- | :--- | :--- |
| CustomerID | INTEGER | Primary key | PK | NOT NULL, AUTO_INCREMENT |
| Name | VARCHAR(100) | Customer name | | NOT NULL |
| Address | VARCHAR(200) | Billing/shipping address | | |
| PhoneNumber | VARCHAR(40) | Contact phone | | |
| Email | VARCHAR(100) | Email address | | |
| Type | ENUM (B2C,B2B) | Customer type | | NOT NULL |
| AccountTerms | VARCHAR(50) | Payment terms (e.g., Net‑30) | | |

### PurchaseOrder

| Field | Type | Description | Key / Notes | Constraints |
| :--- | :--- | :--- | :--- | :--- |
| PurchaseOrderID | INTEGER | Primary key | PK | NOT NULL, AUTO_INCREMENT |
| VendorID | INTEGER | Supplier placing the order | FK | NOT NULL |
| OrderDate | DATE | Date the PO was issued | | NOT NULL |
| Status | ENUM | Draft, Submitted, Approved, Received, Closed | | NOT NULL |
| ExpectedDate | DATE | Expected delivery date | | |
| TotalAmount | DECIMAL(12,2) | Total value | | |
| CreatedBy | INTEGER | User who created the PO | FK | NOT NULL |

### PurchaseOrderLine

| Field | Type | Description | Key / Notes | Constraints |
| :--- | :--- | :--- | :--- | :--- |
| PurchaseOrderLineID | INTEGER | Primary key | PK | NOT NULL, AUTO_INCREMENT |
| PurchaseOrderID | INTEGER | Associated purchase order | FK | NOT NULL |
| ProductID | INTEGER | Product ordered | FK | NOT NULL |
| QuantityOrdered | INTEGER | Number of units ordered | | NOT NULL |
| UnitPrice | DECIMAL(10,2) | Price per unit at order time | | NOT NULL |
| QuantityReceived | INTEGER | Units received so far | | |
| Status | ENUM | Open, Partial, Completed | | NOT NULL |

### InventoryLocation

| Field | Type | Description | Key / Notes | Constraints |
| :--- | :--- | :--- | :--- | :--- |
| LocationID | INTEGER | Primary key | PK | NOT NULL, AUTO_INCREMENT |
| Name | VARCHAR(100) | Name of warehouse or cross‑dock | | NOT NULL |
| Address | VARCHAR(200) | Physical address | | |
| IsRefrigerated | BOOLEAN | Facility has refrigeration | | NOT NULL, DEFAULT FALSE |
| Type | ENUM | DC, CrossDock, 3PL | | NOT NULL |

### InventoryItem

| Field | Type | Description | Key / Notes | Constraints |
| :--- | :--- | :--- | :--- | :--- |
| InventoryItemID | INTEGER | Primary key | PK | NOT NULL, AUTO_INCREMENT |
| LocationID | INTEGER | Warehouse where the stock is held | FK | NOT NULL |
| ProductID | INTEGER | Associated product | FK | NOT NULL |
| LotID | INTEGER | Lot identifier (nullable) | FK | |
| SerialID | INTEGER | Serial number (nullable) | FK | |
| QuantityOnHand | INTEGER | Current stock quantity | | NOT NULL |
| QuantityReserved | INTEGER | Quantity reserved for orders | | NOT NULL |
| MinimumStockLevel | INTEGER | Min stock level for this location | | |
| MaximumStockLevel | INTEGER | Max stock level | | |
| ReorderPoint | INTEGER | Level at which reorder is triggered | | |
| CostPerUnit | DECIMAL(10,2) | Average cost per unit | | |
| LastUpdated | TIMESTAMP | Timestamp of last inventory update | | NOT NULL |

### Lot

| Field | Type | Description | Key / Notes | Constraints |
| :--- | :--- | :--- | :--- | :--- |
| LotID | INTEGER | Primary key | PK | NOT NULL, AUTO_INCREMENT |
| ProductID | INTEGER | Product associated with the lot | FK | NOT NULL |
| LotNumber | VARCHAR(50) | Lot/batch number | | NOT NULL |
| ManufactureDate | DATE | Date of manufacture | | |
| ExpirationDate | DATE | Expiry date | | |
| SupplierID | INTEGER | Vendor that supplied the lot | FK | |

### Serial

| Field | Type | Description | Key / Notes | Constraints |
| :--- | :--- | :--- | :--- | :--- |
| SerialID | INTEGER | Primary key | PK | NOT NULL, AUTO_INCREMENT |
| ProductID | INTEGER | Product associated | FK | NOT NULL |
| SerialNumber | VARCHAR(100) | Unique serial | Unique | NOT NULL |
| WarrantyEnd | DATE | Warranty expiry date | | |
| CustomerID | INTEGER | Owner/customer (nullable) | FK | |

### Order

| Field | Type | Description | Key / Notes | Constraints |
| :--- | :--- | :--- | :--- | :--- |
| OrderID | INTEGER | Primary key | PK | NOT NULL, AUTO_INCREMENT |
| CustomerID | INTEGER | Ordering customer | FK | NOT NULL |
| OrderDate | DATE | When the order was placed | | NOT NULL |
| Status | ENUM | Pending, Confirmed, Packed, Shipped, Cancelled | | NOT NULL |
| Channel | ENUM | Web, B2B Portal, POS, Amazon | | NOT NULL |
| PaymentStatus | ENUM | Unpaid, Paid, Refunded | | NOT NULL |
| FraudStatus | ENUM | Pending, Approved, Flagged | | NOT NULL |
| TotalAmount | DECIMAL(12,2) | Total before tax | | |
| TaxAmount | DECIMAL(12,2) | Computed tax | | |
| ShippingAmount | DECIMAL(12,2) | Shipping cost | | |
| CreatedBy | INTEGER | User who created the order | FK | NOT NULL |

### OrderLine

| Field | Type | Description | Key / Notes | Constraints |
| :--- | :--- | :--- | :--- | :--- |
| OrderLineID | INTEGER | Primary key | PK | NOT NULL, AUTO_INCREMENT |
| OrderID | INTEGER | Associated order | FK | NOT NULL |
| ProductID | INTEGER | Product ordered | FK | NOT NULL |
| Quantity | INTEGER | Units ordered | | NOT NULL |
| UnitPrice | DECIMAL(10,2) | Price per unit at order time | | NOT NULL |
| TaxRateID | INTEGER | Tax jurisdiction | FK | NOT NULL |
| LineTotal | DECIMAL(12,2) | Quantity × UnitPrice | | |

### Shipment

| Field | Type | Description | Key / Notes | Constraints |
| :--- | :--- | :--- | :--- | :--- |
| ShipmentID | INTEGER | Primary key | PK | NOT NULL, AUTO_INCREMENT |
| OrderID | INTEGER | Associated order | FK | NOT NULL |
| LocationID | INTEGER | Warehouse origin | FK | NOT NULL |
| ShipmentDate | DATE | Date shipped | | |
| Carrier | VARCHAR(100) | Shipping carrier | | |
| TrackingNumber | VARCHAR(100) | Tracking number | | |
| HazMatDocs | VARCHAR(200) | Reference to hazmat documentation when required | | |
| Status | ENUM | Pending, InTransit, Delivered, Lost | | NOT NULL |

### RMA (Return)

| Field | Type | Description | Key / Notes | Constraints |
| :--- | :--- | :--- | :--- | :--- |
| RMAID | INTEGER | Primary key | PK | NOT NULL, AUTO_INCREMENT |
| OrderID | INTEGER | Original order | FK | NOT NULL |
| ShipmentID | INTEGER | Associated shipment (if applicable) | FK | |
| CustomerID | INTEGER | Customer returning item | FK | NOT NULL |
| CreatedDate | DATE | RMA creation date | | NOT NULL |
| ReasonCode | ENUM | Reason for return (Damaged, WrongItem, Warranty, Other) | | NOT NULL |
| Disposition | ENUM | Restock, Refurbish, Scrap | | |
| Status | ENUM | Open, Processing, Closed | | NOT NULL |

### TaxRate

| Field | Type | Description | Key / Notes | Constraints |
| :--- | :--- | :--- | :--- | :--- |
| TaxRateID | INTEGER | Primary key | PK | NOT NULL, AUTO_INCREMENT |
| State | VARCHAR(50) | State/Province | | NOT NULL |
| County | VARCHAR(50) | County/locality | | |
| Category | VARCHAR(50) | Tax category (e.g., general, food) | | |
| Rate | DECIMAL(6,4) | Tax percentage | | NOT NULL |
| EffectiveDate | DATE | Effective date | | NOT NULL |

### AccountEntry

| Field | Type | Description | Key / Notes | Constraints |
| :--- | :--- | :--- | :--- | :--- |
| EntryID | INTEGER | Primary key | PK | NOT NULL, AUTO_INCREMENT |
| Date | DATE | Entry date | | NOT NULL |
| AccountNumber | VARCHAR(20) | GL account | | NOT NULL |
| Debit | DECIMAL(12,2) | Debit amount | | |
| Credit | DECIMAL(12,2) | Credit amount | | |
| ReferenceType | ENUM | Order, PurchaseOrder, Inventory, Manual | | NOT NULL |
| ReferenceID | INTEGER | ID of the source record | | |
| Description | VARCHAR(200) | Narrative description | | |

### User & Role (simplified)

| Field | Type | Description | Key / Notes | Constraints |
| :--- | :--- | :--- | :--- | :--- |
| UserID | INTEGER | Primary key | PK | NOT NULL, AUTO_INCREMENT |
| Username | VARCHAR(50) | Login name | Unique | NOT NULL |
| PasswordHash | VARCHAR(200) | Password hash | | NOT NULL |
| Email | VARCHAR(100) | Email | | NOT NULL |
| RoleID | INTEGER | Assigned role | FK | NOT NULL |

| Field | Type | Description | Key / Notes | Constraints |
| :--- | :--- | :--- | :--- | :--- |
| RoleID | INTEGER | Primary key | PK | NOT NULL, AUTO_INCREMENT |
| Name | VARCHAR(50) | Role name (e.g., Warehouse Associate, Purchasing Lead) | Unique | NOT NULL |
| Description | VARCHAR(200) | Role description | | |

This completes the Data Model and ERD specification. The design follows
good database practices—normalized schemas, clear primary and foreign
keys, and support for domain‑driven modularity—while aligning with the
functional requirements of PineCone Pro’s ERP/IMS.
