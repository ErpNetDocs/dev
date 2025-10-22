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

