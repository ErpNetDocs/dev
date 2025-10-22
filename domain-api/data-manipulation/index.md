# Data Manipulation


## Create Operations

### Creating an Entity

To create a new entity in ERP.net, you use the **POST** method of the OData service. The request body contains the entity data in JSON format.  

**Example:** 

Creating a new `Crm_Sales_SalesOrder`:

```
POST https://testdb.my.erp.net/api/domain/odata/Crm_Sales_SalesOrders
Content-Type: application/json

{
    "DocumentType": { "@odata.id": "Systems_Documents_DocumentTypes(469b67b1-8b4b-4fb4-9d97-20c96105a85a)" }, 
    "EnterpriseCompany": {"@odata.id": "General_EnterpriseCompanies(2115c294-33e9-41bd-bc0d-0cd0b7ee1735)" }, 
    "Customer": { "@odata.id": "Crm_Sales_Customers(c8a65f60-26d0-4c3a-81c9-f367dd811908)" },
    "DocumentNotes": "New sales order for testing"
}
```

**Required Fields:**

- **DocumentType:** Type of the document.  
- **EnterpriseCompany:** The company that owns the order.  
- **Customer:** The customer for this sales order.  

Other fields are optional or have defaults. Always validate your data according to the entity model.

For full details on the `Crm_Sales_SalesOrder` entity and all available fields, see the documentation of the used entity. For example [Sales Orders](https://docs.erp.net/model/entities/Crm.Sales.SalesOrders.html).


Create a sales order along with the lines.
```
POST https://testdb.my.erp.net/api/domain/odata/Crm_Sales_SalesOrders
{
  "DocumentType": {
    "@odata.id": "General_DocumentTypes(469b67b1-8b4b-4fb4-9d97-20c96105a85a)"
  },
  "EnterpriseCompany": {
    "@odata.id": "General_EnterpriseCompanies(b0e80577-fbbe-4c9b-811e-20b6c6dd465f)"
  },
  "Customer": {
    "@odata.id": "Crm_Customers(15f2640f-f374-4017-ae2d-d2a41535f054)"
  },
  "DocumentCurrency": {
    "@odata.id": "General_Currencies(3187833a-d3c1-4804-bfc0-e17e6aee3069)"
  },
  "Lines": [
    {
      "Product": {
        "@odata.id": "General_Products_Products(81d38b50-fd06-e611-8292-b31071e2ee7f)"
      },
      "QuantityUnit": {
        "@odata.id": "General_MeasurementUnits(7dbe6d6a-22ef-4c2f-a798-054bc2d13c8b)"
      },
      "Quantity": {
        "Value": 1,
        "Unit": "pcs"
      },
      "UnitPrice": {
        "Value": 20,
        "Currency": "BGN"
      }
    }
  ]
}

```

### Property Dependencies and Update Order

In ERP.net, setting a property may trigger internal **events** that automatically update other dependent properties. Because of this, the **order in which properties are set is important** when creating or updating entities through the API.

For example, the `Quantity` field is a complex type that depends on the `QuantityUnit` reference.  
If you set the `Quantity` before assigning the correct `QuantityUnit`, the system may interpret the value using an outdated or incorrect unit, leading to validation or conversion errors.

To ensure correct behavior, always set the unit **before** the quantity value.

**Example:**

```
{`
    "QuantityUnit": { "@odata.id": "General_MeasurementUnits(6f7cbe0e-9a2a-4e3b-9b64-4f86d8c9e7f0)" },
    "Quantity": { "Value": 1.00, "Unit": "PCE" }
}
```

This ensures that the system knows which measurement unit applies before processing the quantity, preventing inconsistencies when working with values in different units.


> **NOTE:** The same rule applies to other dependent fields such as `Amount` and `Currency` — always set the currency reference before specifying the amount value.



### Data Validation and Business Rules

ERP.net enforces **data validation** through a combination of **System Business Rules** and **User Business Rules**:

- **System Business Rules:** Predefined rules for each entity (e.g., `Crm.Sales.SalesOrders`) that automatically enforce business logic on the back-end. Examples include assigning a sales person, validating fiscal numbers, or restricting line directions. These rules ensure data consistency and compliance without manual intervention.

- **User Business Rules:** Custom rules defined by developers or administrators to implement organization-specific validations. These can be applied on the back-end for security and integrity, and optionally on the front-end for immediate feedback.

Together, system and user business rules ensure that all data entered or modified in ERP.net adheres to required business logic and organizational policies.


### Front-End vs. Back-End Models

ERP.net uses two models to handle entity data and business logic — the **front-end model** and the **back-end model**.  
While both represent the same entities, they differ in the **business rules** and **data validations** that are executed.

- **Front-End Model:**  
  Designed for immediate feedback in client applications.  
  It contains business rules that are triggered on **attribute change events** — for example, when a user edits a field in a form, dependent fields may be recalculated or validated right away.  
  This provides a dynamic and interactive user experience.

- **Back-End Model:**  
  Used when the data is **committed to the server** (e.g., on save, post, or document state change).  
  It contains rules and validations that enforce business integrity, such as required field checks, calculated defaults, and document-level logic triggered by workflow events like state transitions.  

In short, the **front-end model reacts to user edits**, while the **back-end model enforces consistency and performs final validations** before persisting the data.

For more details, see [Transactions in Domain API](https://docs.erp.net/dev/domain-api/transactions.html).



## Update Operations

ERP.net supports updating entities using the **PATCH** method, which performs **partial updates**.  
Only the fields included in the request are modified; all other properties remain unchanged (except dependent properties updated by business rules triggered on attribute change).

**Key Points:**

- Use **PATCH** to modify one or more fields of an existing entity.  
- You do **not** need to send the entire entity.  
- Always consider **property dependencies** — some fields must be set in a specific order (e.g., `QuantityUnit` before `Quantity`, `Currency` before `Amount`).  
- Business rules and validations defined in the front-end or back-end models will be applied depending on how the data is processed.
- ERP.net supports **optimistic locking** to prevent conflicts during concurrent updates. Learn more in [Optimistic Locking](https://docs.erp.net/dev/domain-api/data-sync/optimistic-locking.html).


**Example:** Updating the notes of a sales order:

```
PATCH https://testdb.my.erp.net/api/domain/odata/Crm_Sales_SalesOrders(f3fe442a-d5fe-49a9-8bda-00c895d630bb)
Content-Type: application/json

{
    "DocumentNotes": "Updated sales order description"
}
```

**Required Considerations:**

- If updating fields involved in calculations or triggers, ensure dependent properties are included in the correct order.  
- Only include fields you intend to modify; others will remain unchanged.  
- System and user business rules will execute as appropriate (front-end rules on attribute changes, back-end rules on commit or document events).

For more details on entity properties and rules, refer to the documentation of each entity. Foe example [Sales Orders Documentation](https://docs.erp.net/model/entities/Crm.Sales.SalesOrders.html).


Concurrency Control: Discuss handling concurrent updates.


### Delete Operations

ERP.net supports deleting entities using the **DELETE** HTTP method.  
The request removes the specified entity from the system.

**Key Points:**

- Use **DELETE** with the entity’s OData ID.  
- Business rules on the back-end may enforce restrictions or cascade actions during deletion.  

**Example:** Deleting a sales order:

```
DELETE https://testdb.my.erp.net/api/domain/odata/Crm_Sales_SalesOrders(f3fe442a-d5fe-49a9-8bda-00c895d630bb)
```

Only the entity specified in the request is removed; related entities may be affected according to system rules and cascades.



### Transactions

ERP.net supports **transactions** to ensure that multiple operations on entities are processed consistently.  
Transactions allow you to group create, update, or delete operations into a single unit of work that is either **committed** or **rolled back** as a whole.

**Key Points:**

- Use transactions to maintain **data integrity** when performing multiple operations.  
- Transactions support **partial updates, deletes, and inserts** in a single request.  
- Business rules and validations are applied according to the choosen model: front-end or back-end.  
- ERP.net ensures that either all operations in a transaction succeed, or none are applied.

For full details, see the [Transactions documentation](../transactions.md).



### ERP.net Actions (`@erpnet.action`)

ERP.net uses the `@erpnet.action` annotation to control how JSON data is processed when creating, updating, or importing entities.  
It provides a flexible way to specify operations such as `create`, `update`, `delete`, `find`, and `merge` for top-level or nested objects.

**Key Points:**

- Determines the operation performed on the object.  
- Can be applied to nested objects with optional `@erpnet.findBy` criteria for locating existing entities.  
- Supports combined operations, e.g., find or create, merge, and partial updates.  
- Defaults are automatically applied if the annotation is omitted.

**Example:** Creating or updating a product with a merge action:

```
POST General_Products_Products
{
    "@erpnet.action": "merge",
    "PartNumber": "DAT003",
    "MeasurementUnit": { "Code": "PCE" },
    "Name": { "EN": "Domain API Test 002" }
}
```

For more details, see the [ERP.net Actions documentation](./erpnet-action.md).



### Data Import

ERP.net provides an **Import** endpoint that allows you to insert, update, or delete multiple entities in a single request. This unbound action enables efficient bulk operations, supporting both front-end and back-end models.

**Key Points:**

- **Model Options:** Choose between `frontend` (default) or `backend` models, affecting the business logic applied during the import.
- **Transaction Modes:** Select `all-objects` to commit all changes at once, or `per-object` to commit each object individually.
- **Actions:** Specify operations like `create`, `update`, `delete`, or `merge` for each object.
- **Find Criteria:** Use `@erpnet.findBy` to define search criteria for locating existing entities during the import.

**Example:**

```json
POST ~/Import
{
  "model": "frontend",
  "transaction": "per-object",
  "objects": [
    {
      "@odata.type": "Erp.General_Products_Product",
      "@erpnet.action": "merge",
      "ExternalId": "EXT000",
      "PartNumber": "DATP000",
      "BaseMeasurementCategory": {
        "@erpnet.action": "find",
        "@erpnet.findBy": {
          "Name": "Unit"
        }
      },
      "MeasurementUnit": {
        "Code": "PCE"
      },
      "Name": {
        "EN": "Domain API Test 000"
      },
      "ProductGroup": {
        "Code": "DATG01",
        "Name": {
          "EN": "Domain API Tests"
        }
      }
    }
  ]
}
```

For more details, refer to the [Data Import documentation](./import.md).



### Data Sync

ERP.net provides **Data Synchronization** to efficiently keep client applications up-to-date with system changes.  
Instead of fetching all data repeatedly, Data Sync retrieves only entities that have changed since the last synchronization, ensuring minimal network usage and fast updates.

**Key Points:**

- **Incremental Updates:** Fetch only new or modified entities since a given timestamp or object version.  
- **Object Versioning:** Each entity aggregate has a version number that increments when any related property or reference changes, allowing precise change tracking.  
- **Front-End and Back-End Models:** Supports both models, applying appropriate business rules during synchronization.  
- **Optimistic Locking:** Prevents conflicts when multiple clients attempt to modify the same entity concurrently.  
- **Use Cases:** Ideal for mobile apps, offline clients, integrations, and scenarios requiring near real-time data consistency.

For more details, see the [Data Sync documentation](https://docs.erp.net/dev/domain-api/data-sync/index.html) and [Object Versioning](https://docs.erp.net/dev/domain-api/data-sync/object-version.html).


## Error Handling

When an error occurs during a Domain API operation, the service returns an HTTP status code **500 (Internal Server Error)** and a JSON body containing detailed error information.

For more details, see [Error Handling Topic](./error-handling.md)

