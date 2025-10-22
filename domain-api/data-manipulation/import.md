# Import

ERP.net Domain API defines an Import endpoint which can be used to import multiple entities at once.

**Import** is unbound (not bound to any entity) action (actions are called with HTTP POST method) that inserts, updates or deletes multiple objects.
Specification with example:
```
{
  "transaction": "all-objects" | "per-object" (default),
  "model": "frontend" (default) | "backend",
  "objects":
  [
    {
      "@odata.type": "Crm_Sales_Customers",
      "@erpnet.action": "merge" (default) | "create" | "update" | "delete"
      ...
    },
    ...
  ]
}
```

## Parameters
- **model:** - allowed values are `common` or `frontend`. This parameter indicates the data model used for the import. Front-end data model uses front-end business rules. For example front-end logic is when Quantity of a SalesOrderLine is changed the corresponding QuantityBase is calculated by a dedicated front-end business rule. Common model defines minimal business logic applicable in all cases - front-end or back-end. The default is `frontend`.
- **transaction:** - `all-objects` or `per-object`. This parameter defines when the changes will be commited to the database. If `all-objects` is specified all changes are committed at once at the end of the import. If `per-object` is specified every object is saved when it is ready. The default is `per-object`.
- **objects** - an array of entity objects for import.

### Properties of the objects element
- **"@odata.type"** - Each object must specify valid entity type. The entity type is the singular form of the entity set and can be found in the documentation for each entity. The @odata.type always starts with the default namespace `Erp.` - Example [Erp.General_Products_Product](https://docs.erp.net/model/entities/General.Products.Products.html)
- **"@erpnet.action"** - This is an optional annotation for the desired import action. For top-level objects the default action is `create`. [For more information see this article](erpnet-annotation.md).
- **"@erpnet.findBy"** - This is an optional annotation that specifies the search criteria for the find action. [For more information see this article](erpnet-annotation.md).
- Any data property of the imported object.


## Return value
Specification with example
```
{
  "result": "success" | "fail", 
  "objects":
  [
    {
      "@erpnet.result": "success" | "fail",
      "@odata.id": "Crm_Sales_Customers(<guid>)",      
      "@erpnet.message": "<error-message>"
      "@erpnet.state": "Added" | "Modified" | "Deleted" | "Unchanged"
    },
    ...
  ]
}
```

### Properties of the result value
- **"@erpnet.result"** - `success` or `fail`. The result is `success` only if all objects are imported successfully.
- **objects** - an array of object results - one object for each imported object.

### Properies of the each returned object
- **"@erpnet.result"** - `success` or `fail`.
- **"@odata.id"** - the ODATA Id of the imported object. If result is `fail` this is not available.
- **"@erpnet.message"** - the error message.
- **"@erpnet.state"** -  the status of the imported object. One of "Added" | "Modified" | "Deleted" | "Unchanged". Indicates the operation performed for the object.
Only the "@odata.id" is included in the result - no other properties.

## Examples

### Import Products

The following example performs `merge` action for General_Products_Products. 
If existing product is found by the provided ExternalId it's PartNumber, BaseMeasurementCategory, MeasurementUnit, Name and ProductGroup are updated.
The action for the referenced objects is `find` because the included properties are only these that can be used in find criteria. BaseMeasurementCategory is searched by Name (providing @erpnet.findBy), MeasurementUnit and ProductGroup are searched by Code.
```
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
          "Name": "Pieces"
        }
      },
      "MeasurementUnit": {
        "Code": "pcs"
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
    },
    {
      "@odata.type": "Erp.General_Products_Product",
      "@erpnet.action": "merge",
      "ExternalId": "EXT001",
      "PartNumber": "DATP001",
      "BaseMeasurementCategory": {
        "@erpnet.action": "find",
        "@erpnet.findBy": {
          "Name": "Pieces"
        }
      },
      "MeasurementUnit": {
        "Code": "pcs"
      },
      "Name": {
        "EN": "Domain API Test 001"
      },
      "ProductGroup": {
        "Code": "DATG01",
        "Name": {
          "EN": "Domain API Tests"
        }
      }
    },
    {
      "@odata.type": "Erp.General_Products_Product",
      "@erpnet.action": "merge",
      "ExternalId": "EXT002",
      "PartNumber": "DATP002",
      "BaseMeasurementCategory": {
        "@erpnet.action": "find",
        "@erpnet.findBy": {
          "Name": "Pieces"
        }
      },
      "MeasurementUnit": {
        "Code": "pcs"
      },
      "Name": {
        "EN": "Domain API Test 002"
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

[Try it Yourself](https://testdb.my.erp.net/api/domain/query?POST+Import&payload=ewogICJtb2RlbCI6ICJmcm9udGVuZCIsCiAgInRyYW5zYWN0aW9uIjogInBlci1vYmplY3QiLAogICJvYmplY3RzIjogWwogICAgewogICAgICAiQG9kYXRhLnR5cGUiOiAiRXJwLkdlbmVyYWxfUHJvZHVjdHNfUHJvZHVjdCIsCiAgICAgICJAZXJwbmV0LmFjdGlvbiI6ICJtZXJnZSIsCiAgICAgICJFeHRlbmFsSWQiOiAiRVhUMDAwIiwKICAgICAgIlBhcnROdW1iZXIiOiAiREFUUDAwMCIsCiAgICAgICJCYXNlTWVhc3VyZW1lbnRDYXRlZ29yeSI6IHsKICAgICAgICAiQGVycG5ldC5hY3Rpb24iOiAiZmluZCIsCiAgICAgICAgIkBlcnBuZXQuZmluZEJ5IjogewogICAgICAgICAgIk5hbWUiOiAiUGllY2VzIgogICAgICAgIH0KICAgICAgfSwKICAgICAgIk1lYXN1cmVtZW50VW5pdCI6IHsKICAgICAgICAiQ29kZSI6ICJwY3MiCiAgICAgIH0sCiAgICAgICJOYW1lIjogewogICAgICAgICJFTiI6ICJEb21haW4gQVBJIFRlc3QgMDAwIgogICAgICB9LAogICAgICAiUHJvZHVjdEdyb3VwIjogewogICAgICAgICJDb2RlIjogIkRBVEcwMSIsCiAgICAgICAgIk5hbWUiOiB7CiAgICAgICAgICAiRU4iOiAiRG9tYWluIEFQSSBUZXN0cyIKICAgICAgICB9CiAgICAgIH0KICAgIH0sCiAgICB7CiAgICAgICJAb2RhdGEudHlwZSI6ICJFcnAuR2VuZXJhbF9Qcm9kdWN0c19Qcm9kdWN0IiwKICAgICAgIkBlcnBuZXQuYWN0aW9uIjogIm1lcmdlIiwKICAgICAgIkV4dGVuYWxJZCI6ICJFWFQwMDEiLAogICAgICAiUGFydE51bWJlciI6ICJEQVRQMDAxIiwKICAgICAgIkJhc2VNZWFzdXJlbWVudENhdGVnb3J5IjogewogICAgICAgICJAZXJwbmV0LmFjdGlvbiI6ICJmaW5kIiwKICAgICAgICAiQGVycG5ldC5maW5kQnkiOiB7CiAgICAgICAgICAiTmFtZSI6ICJQaWVjZXMiCiAgICAgICAgfQogICAgICB9LAogICAgICAiTWVhc3VyZW1lbnRVbml0IjogewogICAgICAgICJDb2RlIjogInBjcyIKICAgICAgfSwKICAgICAgIk5hbWUiOiB7CiAgICAgICAgIkVOIjogIkRvbWFpbiBBUEkgVGVzdCAwMDEiCiAgICAgIH0sCiAgICAgICJQcm9kdWN0R3JvdXAiOiB7CiAgICAgICAgIkNvZGUiOiAiREFURzAxIiwKICAgICAgICAiTmFtZSI6IHsKICAgICAgICAgICJFTiI6ICJEb21haW4gQVBJIFRlc3RzIgogICAgICAgIH0KICAgICAgfQogICAgfSwKICAgIHsKICAgICAgIkBvZGF0YS50eXBlIjogIkVycC5HZW5lcmFsX1Byb2R1Y3RzX1Byb2R1Y3QiLAogICAgICAiQGVycG5ldC5hY3Rpb24iOiAibWVyZ2UiLAogICAgICAiRXh0ZW5hbElkIjogIkVYVDAwMiIsCiAgICAgICJQYXJ0TnVtYmVyIjogIkRBVFAwMDIiLAogICAgICAiQmFzZU1lYXN1cmVtZW50Q2F0ZWdvcnkiOiB7CiAgICAgICAgIkBlcnBuZXQuYWN0aW9uIjogImZpbmQiLAogICAgICAgICJAZXJwbmV0LmZpbmRCeSI6IHsKICAgICAgICAgICJOYW1lIjogIlBpZWNlcyIKICAgICAgICB9CiAgICAgIH0sCiAgICAgICJNZWFzdXJlbWVudFVuaXQiOiB7CiAgICAgICAgIkNvZGUiOiAicGNzIgogICAgICB9LAogICAgICAiTmFtZSI6IHsKICAgICAgICAiRU4iOiAiRG9tYWluIEFQSSBUZXN0IDAwMiIKICAgICAgfSwKICAgICAgIlByb2R1Y3RHcm91cCI6IHsKICAgICAgICAiQ29kZSI6ICJEQVRHMDEiLAogICAgICAgICJOYW1lIjogewogICAgICAgICAgIkVOIjogIkRvbWFpbiBBUEkgVGVzdHMiCiAgICAgICAgfQogICAgICB9CiAgICB9CiAgXQp9)


### Import Sales Order

In this example, the **Import** action demonstrates how you can create or update complex records **without manually specifying IDs**.  
The key convenience is that **referenced objects (like Customer, Product, or ProductGroup)** can be **automatically imported or updated** within the same request.  

This makes the Import API extremely useful for **data synchronization with external systems**, where you may not have direct access to internal record IDs but still need to ensure all related entities are properly linked and up to date.

The system automatically determines the `@erpnet.action` and `@erpnet.findBy` criteria based on the provided properties of the nested objects.


```http
POST https://testdb.my.erp.net/api/domain/odata/Import
{
   "objects":[
      {
         "@odata.type":"Erp.Crm_Sales_SalesOrder",
         "DocumentType":{
            "Code":"CRM_SALES_ORDER"
         },
         "EnterpriseCompany":{
            "@erpnet.findBy":{
               "Code":"714895"
            }
         },
         "EnterpriseCompanyLocation":{
            "PartyCode":"00111"
         },
         "Customer":{
            "Number":"CST001",
            "Party":{
               "@odata.type":"Erp.General_Contacts_Company",
               "PartyCode":"CST001",
               "Name":{
                  "EN":"Customer 01"
               },
               "RegistrationType":{
                  "EN":"Ltd"
               },
               "RegistrationNumber":"001001001"
            },
            "EnterpriseCompany":{
               "@erpnet.findBy":{
                  "Code":"714895"
               }
            }
         },
         "DocumentCurrency":{
            "CurrencySign":"GBP"
         },
         "Lines":[
            {
               "Product":{
                  "PartNumber":"PRD001",
                  "BaseMeasurementCategory":{
                     "@erpnet.action":"find",
                     "@erpnet.findBy":{
                        "Name":"Pieces"
                     }
                  },
                  "MeasurementUnit":{
                     "Code":"pcs"
                  },
                  "Name":{
                     "EN":"Product 001"
                  },
                  "ProductGroup":{
                     "Code":"PGT01",
                     "Name":{
                        "EN":"Product Group 01"
                     }
                  }
               },
               "QuantityUnit":{
                  "Code":"pcs"
               },
               "Quantity":{
                  "Value":1,
                  "Unit":"pcs"
               },
               "UnitPrice":{
                  "Value":20,
                  "Currency":"GBP"
               }
            }
         ]
      }
   ]
}
```

[Try it Yourself](https://testdb.my.erp.net/api/domain/query?POST+Import&payload=ewogICAib2JqZWN0cyI6WwogICAgICB7CiAgICAgICAgICJAb2RhdGEudHlwZSI6IkVycC5Dcm1fU2FsZXNfU2FsZXNPcmRlciIsCiAgICAgICAgICJEb2N1bWVudFR5cGUiOnsKICAgICAgICAgICAgIkNvZGUiOiJDUk1fU0FMRVNfT1JERVIiCiAgICAgICAgIH0sCiAgICAgICAgICJFbnRlcnByaXNlQ29tcGFueSI6ewogICAgICAgICAgICAiQGVycG5ldC5maW5kQnkiOnsKICAgICAgICAgICAgICAgIkNvZGUiOiI3MTQ4OTUiCiAgICAgICAgICAgIH0KICAgICAgICAgfSwKICAgICAgICAgIkVudGVycHJpc2VDb21wYW55TG9jYXRpb24iOnsKICAgICAgICAgICAgIlBhcnR5Q29kZSI6IjAwMTExIgogICAgICAgICB9LAogICAgICAgICAiQ3VzdG9tZXIiOnsKICAgICAgICAgICAgIk51bWJlciI6IkNTVDAwMSIsCiAgICAgICAgICAgICJQYXJ0eSI6ewogICAgICAgICAgICAgICAiQG9kYXRhLnR5cGUiOiJFcnAuR2VuZXJhbF9Db250YWN0c19Db21wYW55IiwKICAgICAgICAgICAgICAgIlBhcnR5Q29kZSI6IkNTVDAwMSIsCiAgICAgICAgICAgICAgICJOYW1lIjp7CiAgICAgICAgICAgICAgICAgICJFTiI6IkN1c3RvbWVyIDAxIgogICAgICAgICAgICAgICB9LAogICAgICAgICAgICAgICAiUmVnaXN0cmF0aW9uVHlwZSI6ewogICAgICAgICAgICAgICAgICAiRU4iOiJMdGQiCiAgICAgICAgICAgICAgIH0sCiAgICAgICAgICAgICAgICJSZWdpc3RyYXRpb25OdW1iZXIiOiIwMDEwMDEwMDEiCiAgICAgICAgICAgIH0sCiAgICAgICAgICAgICJFbnRlcnByaXNlQ29tcGFueSI6ewogICAgICAgICAgICAgICAiQGVycG5ldC5maW5kQnkiOnsKICAgICAgICAgICAgICAgICAgIkNvZGUiOiI3MTQ4OTUiCiAgICAgICAgICAgICAgIH0KICAgICAgICAgICAgfQogICAgICAgICB9LAogICAgICAgICAiRG9jdW1lbnRDdXJyZW5jeSI6ewogICAgICAgICAgICAiQ3VycmVuY3lTaWduIjoiR0JQIgogICAgICAgICB9LAogICAgICAgICAiTGluZXMiOlsKICAgICAgICAgICAgewogICAgICAgICAgICAgICAiUHJvZHVjdCI6ewogICAgICAgICAgICAgICAgICAiUGFydE51bWJlciI6IlBSRDAwMSIsCiAgICAgICAgICAgICAgICAgICJCYXNlTWVhc3VyZW1lbnRDYXRlZ29yeSI6ewogICAgICAgICAgICAgICAgICAgICAiQGVycG5ldC5hY3Rpb24iOiJmaW5kIiwKICAgICAgICAgICAgICAgICAgICAgIkBlcnBuZXQuZmluZEJ5Ijp7CiAgICAgICAgICAgICAgICAgICAgICAgICJOYW1lIjoiUGllY2VzIgogICAgICAgICAgICAgICAgICAgICB9CiAgICAgICAgICAgICAgICAgIH0sCiAgICAgICAgICAgICAgICAgICJNZWFzdXJlbWVudFVuaXQiOnsKICAgICAgICAgICAgICAgICAgICAgIkNvZGUiOiJwY3MiCiAgICAgICAgICAgICAgICAgIH0sCiAgICAgICAgICAgICAgICAgICJOYW1lIjp7CiAgICAgICAgICAgICAgICAgICAgICJFTiI6IlByb2R1Y3QgMDAxIgogICAgICAgICAgICAgICAgICB9LAogICAgICAgICAgICAgICAgICAiUHJvZHVjdEdyb3VwIjp7CiAgICAgICAgICAgICAgICAgICAgICJDb2RlIjoiUEdUMDEiLAogICAgICAgICAgICAgICAgICAgICAiTmFtZSI6ewogICAgICAgICAgICAgICAgICAgICAgICAiRU4iOiJQcm9kdWN0IEdyb3VwIDAxIgogICAgICAgICAgICAgICAgICAgICB9CiAgICAgICAgICAgICAgICAgIH0KICAgICAgICAgICAgICAgfSwKICAgICAgICAgICAgICAgIlF1YW50aXR5VW5pdCI6ewogICAgICAgICAgICAgICAgICAiQ29kZSI6InBjcyIKICAgICAgICAgICAgICAgfSwKICAgICAgICAgICAgICAgIlF1YW50aXR5Ijp7CiAgICAgICAgICAgICAgICAgICJWYWx1ZSI6MSwKICAgICAgICAgICAgICAgICAgIlVuaXQiOiJwY3MiCiAgICAgICAgICAgICAgIH0sCiAgICAgICAgICAgICAgICJVbml0UHJpY2UiOnsKICAgICAgICAgICAgICAgICAgIlZhbHVlIjoyMCwKICAgICAgICAgICAgICAgICAgIkN1cnJlbmN5IjoiR0JQIgogICAgICAgICAgICAgICB9CiAgICAgICAgICAgIH0KICAgICAgICAgXQogICAgICB9CiAgIF0KfQ==)

>**NOTE** 
> In case `@erpnet.action` annotation is missing, a default value is used.
> For top-level objects the default value of `@erpnet.action` is `create`. 
> For nested objects the default `@erpnet.action` is `find` if only properties defining the search criteria are provided (either @erpnet.findBy or data properties usable in a find action). If other data properties are provided the default `@erpnet.action` is `merge`.
> See [@erpnet.action](./erpnet-action.md) topic.

**Explanation of the Automatic Actions**

| Property | Automatic Action | Description |
|-----------|------------------|--------------|
| **DocumentType** | `find` | The system searches for a document type with `Code = "CRM_SALES_ORDER"`. Document types are predefined, so we search by code. |
| **EnterpriseCompany** | `find` | Since only `Code` is provided, the system performs a lookup for an existing Enterprise Company with that code. |
| **EnterpriseCompanyLocation** | `find` | The field `PartyCode` uniquely identifies the company location, so the system searches by it. |
| **Customer** | `merge` | Customers are matched by `Number`. If a customer with `Number = "CST001"` exists, it’s passed to the sales order. Otherwise, a new one is created. The `Party` subobject (Company) will also be created if missing. |
| **Customer → Party** | `merge` | Because `Party` is an abstract class, the type is explicitly specified as `Erp.General_Contacts_Company`. A new Party (Company) record is created if none exists (The system first looks up existing party by PartyCode). |
| **DocumentCurrency** | `find` | The `CurrencySign` uniquely identifies the currency (e.g., "GBP"), so an existing record is used. |
| **Lines** | — | Represents an array of line items that will be created as part of the sales order. |
| **Product** | `merge` | The system searches for an existing product with `PartNumber = "PRD001"`. If not found, a new product is created. This behavior prevents duplicate product definitions. |
| **Product → BaseMeasurementCategory** | `find` | Uses `Name = "Pieces"` to locate the measurement category. Note that searching by multi-language properties is always with `contains` criteria. |
| **Product → MeasurementUnit** | `find` | Looks up the measurement unit by `Code = "pcs"`. |
| **Product → ProductGroup** | `merge` | Uses `Code = "PGT01"` to find or create a product group. If it exists, it’s reused; otherwise, it’s created with the given name. |
| **QuantityUnit** | `find` | The system searches for a measurement unit with `Code = "pcs"`. |
| **Quantity / UnitPrice** | — | These are complex properties of the order line and are directly assigned, not looked up. |


### Error Handling

The result contains the error message for each failed object.

Example:

```
{
  "@erpnet.result": "fail",
  "objects": [
    {
      "@erpnet.result": "fail",
      "@erpnet.message": "Object not found: EnterpriseCompany, action: find, findBy: {\"Code\":\"546346373\"}.",
      "@erpnet.error": {
        "message": "Object not found: EnterpriseCompany, action: find, findBy: {\"Code\":\"546346373\"}.",
        "code": 0,
        "type": "InvalidOperationException",
        "info": "System.InvalidOperationException: Object not found: EnterpriseCompany, action: find, findBy: {\"Code\":\"546346373\"}.\r\n   at ErpNet.Model.OData.ODataResourceInfo.GetObject()\r\n   at ErpNet.Model.OData.ODataResourceInfo.Execute()\r\n   at ErpNet.Model.OData.ODataResourceInfo.Execute()\r\n   at ErpNet.Model.OData.ODataResourceInfo.Execute()\r\n   at ErpNet.Model.OData.Operations.ImportAction.HandleRequest(ODataContext odataContext, IDictionary`2 parameters, IODataRequestMessage requestMessage)",
        "messageFormat": null,
        "parameters": null
      }
    }
  ]
}
```