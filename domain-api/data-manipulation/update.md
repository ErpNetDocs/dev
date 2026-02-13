# Update Operations

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

**Example:** Updating a product (including dependent complex fields):

```
PATCH https://testdb.my.erp.net/api/domain/odata/General_Products_...1-478f-91c2-f5520590f534) 
Content-Type: application/json

{ 
    "ABCClass": "A", 
    "MeasurementUnit@odata.bind": "https://testdb.my.erp.net/api/domain/odata/General_Measureme...b-4338-abd0-3a2acb27ff93)",
    "StandardLotSizeBase": { "Value": 3.45, "Unit": "pcs" }
}
```

> [!note]
> When updating complex fields (e.g. `Quantity`, `Amount`) ensure dependent properties are provided consistently, and in the correct order when required by business rules. In general the measurement unit/currency must be set before the quantity/amount.




**Required Considerations:**

- If updating fields involved in calculations or triggers, ensure dependent properties are included in the correct order.  
- Only include fields you intend to modify; others will remain unchanged.  
- System and user business rules will execute as appropriate (front-end rules on attribute changes, back-end rules on commit or document events).

For more details on entity properties and rules, refer to the documentation of each entity. Foe example [Sales Orders Documentation](https://docs.erp.net/model/entities/Crm.Sales.SalesOrders.html).


>**Notes** For concurrency control and optimistic locking see [Data Sync](../data-sync/index.md) topic.
