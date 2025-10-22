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