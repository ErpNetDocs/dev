
# Delete Operations

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

