# Domain API Transactions

Тhe ErpNet Domain API introduces a server-side transaction that holds any change of an entity object made between calls of `BeginTransaction` and `EndTransaction`.

An api transaction is a data set that contains the data for the objects used in the transaction. When we create a domain object in a transaction, a data set containing the data for this object is created in the memory of the api process. If we create another object in the same transaction, the second object is also saved in the same data set. The data is not yet present in the database until we commit the transaction.

If we update the same field with different API requests in the same transaction, the value of the field remains the one submitted last.

If we use a front-end transaction, each submission of a value for a given field will trigger front-end events that may update other fields.

When the transaction is committed, the entire dataset is submitted to the database. At this point, we don't guarantee the order in which records from the same table are inserted into database. That's why several objects that have reference to the same entity type must be created in different transactions (for example parent-child related documents).

## BeginTransaction

**BeginTransaction** is unbound (not bound to any entity) action (actions are called with HTTP POST method) that initializes an object transaction on the server and  returns a *TransactionId* token as a plain text (not json - for example XXXXX). This object transaction is something like memory data-set that holds copies of database records. 

If any subsequent request includes the *TransactionId* token in the HTTP header (like this: TransactionId:XXXXX) the requested operation will not be executed against the database but against the memory transaction. This means that any POST, PATCH and DELETE requests will be executed only in memory.

### Parameters

- **model:** allowed values are `common` or `frontend`. This parameter indicates the data model used for the transaction. Front-end data model uses front-end business rules. For example front-end logic is when Quantity of a SalesOrderLine is changed the corresponding QuantityBase is calculated by a dedicated front-end business rule. Common model defines minimal business logic applicable in all cases - front-end or back-end. The default is `common`.
- **trackChanges:** `true` or `false`. This parameter enables the usage of GetChanges and WaitForChanges functions. This means that if parameter `trackChanges` is not `true` any call to `GetChanges` and `WaitForChanges` will return error.

## GetChanges

**GetChanges** is unbound function - functions are invoked through GET HTTP method. This function requires the HTTP header *TransactionId* to be passed. It also requires the front-end transaction to be initialized with  *trackChanges*: `true`. Front-end transactions support front-end business rules. These are rules that are usually triggered on attribute change. For example if we change the DocumentDate attribute of a SalesOrder (using regular PATCH request including the *TransactionId* in the request header), this change will trigger updates of several other attributes. If the front-end transaction is initialized with *trackChanges*: `true`, the changes will be collected in the server side (inside the front-end transaction). GetChanges returns all changes made after the last call of GetChanges or WaitForChanges. The changes are grouped by operation type (insert, update, delete), entity name, entity id. The call of GetChanges (or WaitForChanges) clears the collected changes in the transaction. If no change is made after the last call of GetChanges it will return empty result (empty JSON object).

## WaitForChanges

**WaitForChanges** is unbound function that returns the same result as GetChanges but if there is not any change the function blocks until a change occurs or until it times out. The wait timeout is 2 minutes. This function requires the HTTP header *TransactionId* to be passed. It also requires the front-end transaction to be initialized with *trackChanges*: `true`.

The response format of GetChanges and WaitForChanges follows this JSON schema:

```
"insert" | "update" | "delete" : {
    "<entity-name>" : {   // example: "General_Products_Products"
        "<id>": { // example: 59098bcf-f331-478f-91c2-f5520590f534 (Guid)
            "<attribute>": <value> // example: "ABCClass": "A"
        }
    }
}
```
## EndTransaction

**EndTransaction** is unbound action that disposes the memory transaction created with `BeginTransaction`. After `EndTransaction` the transaction id becomes invalid.

### Parameters

- **commit:** `true` or `false`. Specifies whether to commit the transaction (save the changes) or not. Default is `true`. 

To commit the changes made in the memory transaction to the database you should provide the parameter commit = `true`.

> [!Note]
> The object transaction is called Front-End because any change of an entity object will trigger front-end business rules. For example if Quantiy of a SalesOrderLine is changed the corresponding QuantityBase will be automatically recalculated. This is front-end behavior - if front-end transaction is not used the QuantityBase will not be automatically recalculated and it's value must be explicitly set.
>
> Through Front-End Transaction, GetChanges or WaitForChanges we can synchronize our UI passing only the user actions to the server - such as update of an attribute, creating new object or deleting existing object.

## Transaction Lifespan and Management

Each transaction has a defined lifespan.

In a typical scenario:
- The lifespan of a transaction starts with the `BeginTransaction` call
- and ends with the `EndTransaction` call

Additionally, each transaction has an absolute maximum lifespan, which is **25 minutes**.

> [!Note]
> Every transaction will be automatically cleared 25 minutes after its creation unless it has already been explicitly cleared with EndTransaction.

If you reference the `TransactionId` of a cleared (or not existing) transaction, an error will be returned:
```
"Invalid TransactionId {id}".
```

> [!Note]
> It is best practice to keep transactions short and always close them when your work with them is complete.

## Examples

### Simple usage of a front end transaction

#### BeginTransaction

```http
POST /api/domain/odata/BeginTransaction HTTP/1.1
Host: https://example.com
Content-Type: application/json

{
  "model": "frontend"
}
```
Response (Transaction id): `843f05ff3f62400c990d2a3b119e256e`

#### Update

Make subsequent updates of products

```http
PATCH /api/domain/odata/General_Products_Products(59098bcf-f331-478f-91c2-f5520590f534) HTTP/1.1
Host: https://example.com
Content-Type: application/json
TransactionId: 843f05ff3f62400c990d2a3b119e256e

{
  "ABCClass":"A",
  "StandardLotSizeBase":{"Value":3.45,"Unit":"PCS"},
  "MeasurementUnit@odata.bind":"https://example.com/api/domain/odata/General_MeasurementUnits(5c5e77ce-60bb-4338-abd0-3a2acb27ff93)"
}
```

#### CommitTransaction

```http
POST /api/domain/odata/EndTransaction HTTP/1.1
Host: https://example.com
Content-Type: application/json
TransactionId: 843f05ff3f62400c990d2a3b119e256e

{
  "commit": true
}
```

#### Updating SalesOrder.DocumentDate and calling `GetChanges`

### BeginTransaction

```http
POST /api/domain/odata/BeginTransaction HTTP/1.1
Host: https://example.com
Content-Type: application/json

{
  "model": "frontend",
  "trackChanges": true
}
```
Response (Transaction id): `fd5d3bbc38ae4dd9a8a5c0ff46c8e3af`

### Updating DocumentDate

This update triggers many front-end business rules that update many other attributes in the sales order and it's lines.

```http
PATCH /api/domain/odata/Crm_Sales_SalesOrders(33cd6cb9-0f43-df11-a1e1-0018f3790817)
Host: https://example.com
Content-Type: application/json
TransactionId: fd5d3bbc38ae4dd9a8a5c0ff46c8e3af

{
  "DocumentDate": "2020-05-08T00:00:00Z"
}
```
Response: `nocontent`

### Call GetChanges

The result is a JSON object with all changes made after the last call to GetChanges (or `BeginTransaction` if `GetChanges` is not called yet).

```http
GET /api/domain/odata/GetChanges HTTP/1.1
Host: https://example.com
Content-Type: application/json
TransactionId: fd5d3bbc38ae4dd9a8a5c0ff46c8e3af
```
Response:
```json
{
  "@odata.context": "https://clients.inco.bg/api/domain/odata/$metadata#Erp.OpenObject",
  "update": {
    "Crm_Sales_SalesOrders": {
      "33cd6cb9-0f43-df11-a1e1-0018f3790817": {
        "PaymentDueDate": "2020-05-08T00:00:00Z",
        "PaymentDueStartDate": "2020-05-08T00:00:00Z",
        "RequiredDeliveryDate": "2020-05-08T00:00:00Z",
        "DocumentDate": "2020-05-08T00:00:00Z"
      }
    },
    "Crm_Sales_SalesOrderLines": {
      "c253add9-0f43-df11-a1e1-0018f3790817": {
        "HistoricalUnitCost": null,
        "RequestedQuantity": null,
        "StandardUnitPrice": null,
        "RequiredDeliveryDate": "2020-05-08T00:00:00Z",
        "LineAmount": {
          "Value": 1.62,
          "Currency": "BGN"
        },
        "Quantity": {
          "Value": 2,
          "Unit": "бр"
        },
        "QuantityBase": {
          "Value": 2,
          "Unit": "бр"
        },
        "StandardQuantityBase": {
          "Value": 2,
          "Unit": "бр"
        },
        "UnitPrice": {
          "Value": 0.9,
          "Currency": "BGN"
        }
      },
      "c653add9-0f43-df11-a1e1-0018f3790817": {
        "HistoricalUnitCost": null,
        "RequestedQuantity": null,
        "StandardUnitPrice": null,
        "RequiredDeliveryDate": "2020-05-08T00:00:00Z",
        "LineAmount": {
          "Value": 5.53,
          "Currency": "BGN"
        },
        "Quantity": {
          "Value": 3,
          "Unit": "бр"
        },
        "QuantityBase": {
          "Value": 3,
          "Unit": "бр"
        },
        "StandardQuantityBase": {
          "Value": 3,
          "Unit": "бр"
        },
        "UnitPrice": {
          "Value": 2,
          "Currency": "BGN"
        }
      },
      "c753add9-0f43-df11-a1e1-0018f3790817": {
        "HistoricalUnitCost": null,
        "RequestedQuantity": null,
        "StandardUnitPrice": null,
        "RequiredDeliveryDate": "2020-05-08T00:00:00Z",
        "LineAmount": {
          "Value": 4.38,
          "Currency": "BGN"
        },
        "Quantity": {
          "Value": 6,
          "Unit": "kg"
        },
        "QuantityBase": {
          "Value": 6,
          "Unit": "kg"
        },
        "StandardQuantityBase": {
          "Value": 6,
          "Unit": "kg"
        },
        "UnitPrice": {
          "Value": 0.8,
          "Currency": "BGN"
        }
      },
      "c153add9-0f43-df11-a1e1-0018f3790817": {
        "HistoricalUnitCost": null,
        "RequestedQuantity": null,
        "StandardUnitPrice": null,
        "RequiredDeliveryDate": "2020-05-08T00:00:00Z",
        "LineAmount": {
          "Value": 10.56,
          "Currency": "BGN"
        },
        "Quantity": {
          "Value": 10,
          "Unit": "l"
        },
        "QuantityBase": {
          "Value": 20,
          "Unit": "бр"
        },
        "StandardQuantityBase": {
          "Value": 20,
          "Unit": "бр"
        },
        "UnitPrice": {
          "Value": 1.17348,
          "Currency": "BGN"
        }
      }
    }
  }
}
```
