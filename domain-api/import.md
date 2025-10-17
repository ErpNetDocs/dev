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
- **model:** - allowed values are `common` or `frontend`. This parameter indicates the data model used for the import. Front-end data model uses front-end business rules. For example front-end logic is when Quantity of a SalesOrderLine is changed the corresponding QuantityBase is calculated by a dedicated front-end business rule. Common model defines minimal business logic applicable in all cases - front-end or back-end. The default is `common`.
- **transaction:** - `all-objects` or `per-object`. This parameter defines when the changes will be commited to the database. If `all-objects` is specified all changes are committed at once at the end of the import. If `per-object` is specified every object is saved once the data is imported.
- **objects** - an array of entity objects for import.

### Properties of the objects element
- **"@odata.type"** - Each object must specify valid entity type. The entity type is the singular form of the entity set and can be found in the documentation for each entity. The @odata.type always starts with the default namespace `Erp.` - Example [Erp.General_Products_Product](https://docs.erp.net/model/entities/General.Products.Products.html)
- **"@erpnet.action"** - This is an optional annotation for the desired import action. For more information see this article. For top-level objects the default action is `create`.
- **"@erpnet.findBy"** - This is an optional annotation that specifies the search criteria for the find action. For more information see this article.
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
- **@erpnet.result** - `success` or `fail`. The result is `success` only if all objects are imported successfully.
- **objects** - an array of object results - one object for each imported object.

### Properies of the each returned object
- **@erpnet.result** - `success` or `fail`.
- **"@odata.id"** - the ODATA Id of the imported object. If result is `fail` this is not available.
- **"@erpnet.message"** - the error message.
- **"@erpnet.state"** -  the status of the imported object. One of "Added" | "Modified" | "Deleted" | "Unchanged". Indicates the operation performed for the object.
Only the @odata.id is included in the result - no other properties.

## Examples
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
    },
    {
      "@odata.type": "Erp.General_Products_Product",
      "@erpnet.action": "merge",
      "ExternalId": "EXT001",
      "PartNumber": "DATP001",
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
          "Name": "Unit"
        }
      },
      "MeasurementUnit": {
        "Code": "PCE"
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

