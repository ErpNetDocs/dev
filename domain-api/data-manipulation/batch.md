# OData $batch

## Overview

OData `$batch` allows multiple Domain API requests to be sent in a single HTTP request.

Use `$batch` when you want to:

- execute several read requests together
- group related write operations
- create a resource and continue working with it in the same request

For the OData protocol definition, see [OData Version 4.01 Part 1: Protocol - Batch Requests](https://docs.oasis-open.org/odata/odata/v4.01/csprd02/part1-protocol/odata-v4.01-csprd02-part1-protocol.html#sec_BatchRequests).

## Getting Started

Use the Domain API batch endpoint:

`POST /api/domain/odata/$batch`

The following example is executable in `testdb.my.erp.net`.

[Run in Query Tool](https://testdb.my.erp.net/api/domain/query?POST+$batch&payload=LS1iYXRjaF8xMjMKQ29udGVudC1UeXBlOiBhcHBsaWNhdGlvbi9odHRwIApDb250ZW50LVRyYW5zZmVyLUVuY29kaW5nOmJpbmFyeQoKR0VUIEdlbmVyYWxfUHJvZHVjdHNfUHJvZHVjdHM/JHRvcD0yJiRzZWxlY3Q9UGFydE51bWJlcixOYW1lIEhUVFAvMS4xCgotLWJhdGNoXzEyMwpDb250ZW50LVR5cGU6IGFwcGxpY2F0aW9uL2h0dHAgCkNvbnRlbnQtVHJhbnNmZXItRW5jb2Rpbmc6YmluYXJ5CgpHRVQgR2VuZXJhbF9Qcm9kdWN0c19Qcm9kdWN0R3JvdXBzPyR0b3A9MiYkc2VsZWN0PU5hbWUsQ29kZSBIVFRQLzEuMQoKLS1iYXRjaF8xMjMKQ29udGVudC1UeXBlOiBtdWx0aXBhcnQvbWl4ZWQ7IGJvdW5kYXJ5PWNoYW5nZXNldF8xCgotLWNoYW5nZXNldF8xCkNvbnRlbnQtVHlwZTogYXBwbGljYXRpb24vaHR0cCAKQ29udGVudC1UcmFuc2Zlci1FbmNvZGluZzogYmluYXJ5CkNvbnRlbnQtSUQ6IDEKClBPU1QgR2VuZXJhbF9Qcm9kdWN0c19Qcm9kdWN0cyBIVFRQLzEuMQoKewogICJQYXJ0TnVtYmVyIjogIlBSRC0wMDUiLAogICJOYW1lIjogewogICAgIkVOIjogIlBSRC0wMDUiCiAgfSwKICJCYXNlTWVhc3VyZW1lbnRDYXRlZ29yeSI6IHsKICAgICJAZXJwbmV0LmFjdGlvbiI6ICJmaW5kIiwKICAgICJAZXJwbmV0LmZpbmRCeSI6ICB7TmFtZTogIlBpZWNlcyJ9CiAgfSwKICAiTWVhc3VyZW1lbnRVbml0IjogewogICAgIkNvZGUiOiAicGNzIgogIH0sCiAgIlByb2R1Y3RHcm91cCI6IHsKICAgICJDb2RlIjogIjQ1MCIKICB9LAogICJDb3N0aW5nQ3VycmVuY3kiOiB7ICJDdXJyZW5jeVNpZ24iOiAiRVVSIiB9Cn0KCi0tY2hhbmdlc2V0XzEKQ29udGVudC1UeXBlOiBhcHBsaWNhdGlvbi9odHRwIApDb250ZW50LVRyYW5zZmVyLUVuY29kaW5nOiBiaW5hcnkKQ29udGVudC1JRDogMgoKUEFUQ0ggJDEgSFRUUC8xLjEKCnsKICJTdGFuZGFyZENvc3RQZXJMb3QiOiB7CiAgICAiVmFsdWUiOiAxMCwKICAgICJDdXJyZW5jeSI6ICJFVVIiCiAgfQp9CgotLWNoYW5nZXNldF8xCkNvbnRlbnQtVHlwZTogYXBwbGljYXRpb24vaHR0cCAKQ29udGVudC1UcmFuc2Zlci1FbmNvZGluZzogYmluYXJ5CkNvbnRlbnQtSUQ6IDMKClBPU1QgJDEvUHJpY2VzIEhUVFAvMS4xCgp7CiAiUHJpY2UiOiB7CiAgICAiVmFsdWUiOiAxMiwKICAgICJDdXJyZW5jeSI6ICJFVVIiCiAgfSwKICAiUHJpY2VRdWFudGl0eSI6ewogICAgIlZhbHVlIjogMSwKICAgICJVbml0IjogInBjcyIKICB9Cn0KLS1jaGFuZ2VzZXRfMS0tCi0tYmF0Y2hfMTIzLS0=)

> [!NOTE]
> The example creates product `PRD-005`. If the product already exists, change the `PartNumber` before running the batch again.

The following batch request contains two read operations and one changeset with three write operations.

```http
POST /api/domain/odata/$batch HTTP/1.1
Content-Type: multipart/mixed; boundary=batch_123

--batch_123
Content-Type: application/http
Content-Transfer-Encoding: binary

GET General_Products_Products?$top=2&$select=PartNumber,Name HTTP/1.1

--batch_123
Content-Type: application/http
Content-Transfer-Encoding: binary

GET General_Products_ProductGroups?$top=2&$select=Name,Code HTTP/1.1

--batch_123
Content-Type: multipart/mixed; boundary=changeset_1

--changeset_1
Content-Type: application/http
Content-Transfer-Encoding: binary
Content-ID: 1

POST General_Products_Products HTTP/1.1
Content-Type: application/json

{
  "PartNumber": "PRD-005",
  "Name": {
    "EN": "PRD-005"
  },
  "BaseMeasurementCategory": {
    "@erpnet.action": "find",
    "@erpnet.findBy": {
      "Name": "Pieces"
    }
  },
  "MeasurementUnit": {
    "Code": "pcs"
  },
  "ProductGroup": {
    "Code": "450"
  },
  "CostingCurrency": {
    "CurrencySign": "EUR"
  }
}

--changeset_1
Content-Type: application/http
Content-Transfer-Encoding: binary
Content-ID: 2

PATCH $1 HTTP/1.1
Content-Type: application/json

{
  "StandardCostPerLot": {
    "Value": 10,
    "Currency": "EUR"
  }
}

--changeset_1
Content-Type: application/http
Content-Transfer-Encoding: binary
Content-ID: 3

POST $1/Prices HTTP/1.1
Content-Type: application/json

{
  "Price": {
    "Value": 12,
    "Currency": "EUR"
  },
  "PriceQuantity": {
    "Value": 1,
    "Unit": "pcs"
  }
}

--changeset_1--
--batch_123--
```

This batch performs the following operations:

1. `GET General_Products_Products...`  
   Returns up to two products with `PartNumber` and `Name`.

2. `GET General_Products_ProductGroups...`  
   Returns up to two product groups with `Name` and `Code`.

3. `POST General_Products_Products`  
   Creates a new product.  
   `BaseMeasurementCategory` uses `@erpnet.action: "find"` to resolve an existing record by `Name`.

   The nested resources `MeasurementUnit`, `ProductGroup`, and `CostingCurrency` do not specify `@erpnet.action`. In `POST` processing, ERP.net uses the implicit **`merge`** action for such nested resources. This means the system tries to match an existing record by the supplied key fields and bind it to the new product.

4. `PATCH $1`  
   Updates the product created by request `1`.  
   `$1` is a Content-ID reference to the resource created earlier in the same changeset.

5. `POST $1/Prices`  
   Creates a new record in the `Prices` child collection of the product created by request `1`.

## Concepts

### Batch structure

A batch request uses `multipart/mixed`.

The outer batch can contain:

- individual read requests
- one or more changesets

A changeset contains write operations such as `POST`, `PATCH`, and `DELETE`.

### Changeset transaction

A changeset is an **atomic transaction**.

All operations in the changeset either succeed together or fail together. If one write operation in the changeset fails, all changes made by that changeset are rolled back and the whole changeset fails.

### Content-ID references

A request inside a changeset can define a `Content-ID`:

```http
Content-ID: 1
```

Later requests in the same changeset can reference the created resource through `$1`:

```http
PATCH $1 HTTP/1.1
POST $1/Prices HTTP/1.1
```

This allows dependent operations to be executed in one batch.

### `@erpnet.action` and nested resource processing

ERP.net extends standard OData updates with `@erpnet.action`.

In this example:

- `@erpnet.action: "find"` tells ERP.net to locate an existing related record by the fields in `@erpnet.findBy`
- nested resources in a `POST` request without explicit `@erpnet.action` are processed with implicit **`merge`**

This is useful when creating a resource that references existing related records without providing their full resource URLs.

### Batch response

The batch response is also `multipart/mixed`. Each top-level request in the batch returns a corresponding response part in the same order.

In the example response from `testdb.my.erp.net`:

- the first `GET` returns `200 OK`
- the second `GET` returns `200 OK`
- the changeset returns an error for `Content-ID: 1`

The response of our $batch request is
```http
--batchresponse_cd29f967-a9ef-43db-ad37-7d815c56fdf3
Content-Type: application/http
Content-Transfer-Encoding: binary

HTTP/1.1 200 OK
Content-Type: application/json;odata.metadata=minimal;odata.streaming=true;IEEE754Compatible=false;charset=utf-8
OData-Version: 4.0

{"@odata.context":"https://testdb.my.erp.net/api/domain/odata/$metadata#General_Products_Products","@odata.nextLink":"https://testdb.my.erp.net/api/domain/odata/General_Products_Products?%24top=2&%24select=PartNumber%2cName&%24skiptoken=d8372f13-a7f5-484f-bc20-00b929a00283","value":[{"@odata.id":"General_Products_Products(edf2bd2a-7e4d-e111-a06c-00155d00050a)","PartNumber":"90002","Name":{"EN":"Customs Duties","BG":"\u041c\u0438\u0442\u043e"}},{"@odata.id":"General_Products_Products(7d05ca48-e51a-4672-b187-002a1beef969)","PartNumber":"5100000003","Name":{"EN":"Suspension Arm - Rear Left","BG":"\u041d\u043e\u0441\u0430\u0447 - \u0437\u0430\u0434\u0435\u043d \u043b\u044f\u0432"}}]}
--batchresponse_cd29f967-a9ef-43db-ad37-7d815c56fdf3
Content-Type: application/http
Content-Transfer-Encoding: binary

HTTP/1.1 200 OK
Content-Type: application/json;odata.metadata=minimal;odata.streaming=true;IEEE754Compatible=false;charset=utf-8
OData-Version: 4.0

{"@odata.context":"https://testdb.my.erp.net/api/domain/odata/$metadata#General_Products_ProductGroups","@odata.nextLink":"https://testdb.my.erp.net/api/domain/odata/General_Products_ProductGroups?%24top=2&%24select=Name%2cCode&%24skiptoken=f00170b0-528c-4481-8f14-09eab18d3c37","value":[{"@odata.id":"General_Products_ProductGroups(d189cd37-1301-e011-9c1f-0030488d5648)","Code":"2","Name":{"EN":"Goods","BG":"\u0421\u0442\u043e\u043a\u0438"}},{"@odata.id":"General_Products_ProductGroups(7174e93a-a85e-43b8-9f77-042e2ac89b5a)","Code":"20102","Name":{"EN":"Coffee Maker","BG":"\u041a\u0430\u0444\u0435 \u043c\u0430\u0448\u0438\u043d\u0438"}}]}
--batchresponse_cd29f967-a9ef-43db-ad37-7d815c56fdf3
Content-Type: multipart/mixed; boundary=changesetresponse_98300da1-9dd2-4da8-868b-2393d65b2715

--changesetresponse_98300da1-9dd2-4da8-868b-2393d65b2715
Content-Type: application/http
Content-Transfer-Encoding: binary
Content-ID: 1

HTTP/1.1 500 Internal Server Error
Content-Type: application/json

{"error":{"message":"An error occurred while saving data to the database\r\nThe validation R17342: ProductAndProductGroupEnterpriseCompanyAreDifferent failed for event Commit: \r\n\r\nCannot save product with code \u0027PRD-005\u0027, because it has an invalid entry for Enterprise Company.\r\n\r\nWhen an enterprise company is specified for a product group, the products within this group should have the same enterprise company. (Constraint R17342)","code":2029,"type":"Aloe.EnterpriseOne.Server.ServerAPI.Exceptions.EnterpriseOneServerException","info":"...skipped","messageFormat":null,"parameters":null}}
--changesetresponse_98300da1-9dd2-4da8-868b-2393d65b2715--
--batchresponse_cd29f967-a9ef-43db-ad37-7d815c56fdf3--
```

The failed write operation returns `500 Internal Server Error` with validation error `R17342`. Because the changeset is atomic, the remaining write operations are not applied and the whole changeset fails.

## Related topics

- [Create](create.md)
- [Update](update.md)
- [Delete](delete.md)
- [Transactions](transactions.md)
- [ERP.net Actions](erpnet-action.md)
