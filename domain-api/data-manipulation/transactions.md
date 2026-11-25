# Domain API Transactions

Use a Domain API transaction to group create, update, and delete requests that must be saved together. Begin the transaction and send its ID with each request. A successful write stages the change. Commit to save the changes together or roll back to discard them.

For related writes that you can send together in a single request, you can also use a changeset in [OData `$batch`](batch.md).

## Transaction lifecycle

This example updates the short names of two existing products as one unit. Use an access token with permission to update those products. Replace the example host and IDs with values from your instance.

### BeginTransaction

Send `POST BeginTransaction` to the Domain API service root. This is an unbound action. The response body is the transaction ID as plain text.

| Request field | Value |
|---------------|-------|
| `model` | `"common"` is the default and uses the business rules shared by the models. Choose `"frontend"` when you need rules that run as fields change, such as recalculating dependent fields during an edit. |
| `trackChanges` | Set to `true` to collect changes for `GetChanges`. Defaults to `false`. |

```http
POST /api/domain/odata/BeginTransaction HTTP/1.1
Host: demodb.my.erp.net
Authorization: Bearer <access_token>
Content-Type: application/json

{
  "model": "common"
}
```

Example response body.

```text
843f05ff3f62400c990d2a3b119e256e
```

### Send requests in the transaction

Use the returned text as the value of the `TransactionId` header. Send the header and authenticate each request in the same session. Send the requests in sequence.

The API keeps the pending changes in memory on the server. Each request with the same transaction ID works with that transaction's records, including records created in earlier requests. If you update the same field twice, the last successful update supplies its value, subject to any business rules that run on the record.

```http
PATCH /api/domain/odata/General_Products_Products(59098bcf-f331-478f-91c2-f5520590f534) HTTP/1.1
Host: demodb.my.erp.net
Authorization: Bearer <access_token>
TransactionId: 843f05ff3f62400c990d2a3b119e256e
Content-Type: application/json

{
  "ShortName": { "EN": "Oak chair" }
}
```

```http
PATCH /api/domain/odata/General_Products_Products(5263a2d3-88b0-41db-adae-31c76135719e) HTTP/1.1
Host: demodb.my.erp.net
Authorization: Bearer <access_token>
TransactionId: 843f05ff3f62400c990d2a3b119e256e
Content-Type: application/json

{
  "ShortName": { "EN": "Oak table" }
}
```

A successful `PATCH` returns `204 No Content`. To read a staged value before committing, send a `GET` with the same `TransactionId`. An ordinary write sent without the header runs separately and can save immediately.

The order of requests in a transaction does not guarantee the order in which records are inserted at commit. If a dependent record requires a previously saved record, commit that record first. For example, save a parent document before starting a separate transaction for a document that requires it as a saved parent. See [Working with documents](../common-tasks/working-with-documents.md).

### EndTransaction

Call the unbound action `POST EndTransaction` to save or discard the staged changes. Send the `TransactionId` header and an explicit `commit` value.

```http
POST /api/domain/odata/EndTransaction HTTP/1.1
Host: demodb.my.erp.net
Authorization: Bearer <access_token>
TransactionId: 843f05ff3f62400c990d2a3b119e256e
Content-Type: application/json

{
  "commit": true
}
```

Use `"commit": true` to save the changes or `"commit": false` to discard them. Omitting `commit` can discard staged changes, so always send it explicitly. A successful response returns the ID as plain text and closes the transaction.

### Handle errors and retries

Business rules can still reject the changes at commit time. In an application, make sure the error or cancellation path attempts to roll back any transaction that is still open. Check for success from `EndTransaction` before reporting that the changes were saved. If the commit response is lost, check the saved data before repeating the operation. See [Error handling](error-handling.md).

## Front-end transactions

Choose `"frontend"` to run rules that react to edits. For example, changing a sales order line's `Quantity` can recalculate `QuantityBase` as the edit is applied. With `"common"`, supply any dependent values your request needs during editing. A client can use [change tracking](#track-changes) to refresh its form with values changed by front-end rules, including values on related records. See [Data Validation and Business Rules](data-validation.md).

### Update product attributes

Start a front-end transaction with `POST BeginTransaction`.

```http
POST /api/domain/odata/BeginTransaction HTTP/1.1
Host: demodb.my.erp.net
Authorization: Bearer <access_token>
Content-Type: application/json

{
  "model": "frontend"
}
```

Use the returned ID on the product update. The example uses `82cc6c73327448e1a7be64da7b655e4c`. This request sets the ABC class, standard lot size, and measurement unit. Use a unit from the product's measurement category and express `StandardLotSizeBase` in the category's base unit.

```http
PATCH /api/domain/odata/General_Products_Products(59098bcf-f331-478f-91c2-f5520590f534) HTTP/1.1
Host: demodb.my.erp.net
Authorization: Bearer <access_token>
TransactionId: 82cc6c73327448e1a7be64da7b655e4c
Content-Type: application/json

{
  "ABCClass": "A",
  "StandardLotSizeBase": { "Value": 3.45, "Unit": "PCS" },
  "MeasurementUnit@odata.bind": "General_Products_MeasurementUnits(5c5e77ce-60bb-4338-abd0-3a2acb27ff93)"
}
```

Finish with [`EndTransaction`](#endtransaction) using that same ID.

## Track changes

To read tracked changes, start the transaction with `trackChanges: true` and send its `TransactionId` header on each call.

### GetChanges

`GET GetChanges` is an unbound function. Use it after an edit to find which records and fields changed.

### WaitForChanges

`GET WaitForChanges` is defined as an unbound function that waits up to two minutes for changes. It uses the same response format as `GetChanges`, with no operation entries on timeout.

### When changes are cleared

`GetChanges` reads and clears the queue. The first read covers changes since the transaction began. Later reads cover changes since the last time the queue was read. `WaitForChanges` uses the same queue when it successfully returns changes.

### Change response format

The response groups changes by operation (`insert`, `update`, or `delete`), then by entity set and record ID. Each record entry contains the current values of its changed fields. Multiple edits to the same field appear as one value. A delete entry has an empty object for the record. A response with no operation entries means there are no new changes.

### Track changes to a sales order

#### Start tracking before updating a sales order

Start a front-end transaction with change tracking enabled.

```http
POST /api/domain/odata/BeginTransaction HTTP/1.1
Host: demodb.my.erp.net
Authorization: Bearer <access_token>
Content-Type: application/json

{
  "model": "frontend",
  "trackChanges": true
}
```

The example uses the returned ID `fd5d3bbc38ae4dd9a8a5c0ff46c8e3af`.

#### Update the order and fetch changes

Use the same ID for both requests. A change to the document date can update related dates and sales order lines.

```http
PATCH /api/domain/odata/Crm_Sales_SalesOrders(33cd6cb9-0f43-df11-a1e1-0018f3790817) HTTP/1.1
Host: demodb.my.erp.net
Authorization: Bearer <access_token>
TransactionId: fd5d3bbc38ae4dd9a8a5c0ff46c8e3af
Content-Type: application/json

{
  "DocumentDate": "2020-05-08T00:00:00Z"
}
```

```http
GET /api/domain/odata/GetChanges HTTP/1.1
Host: demodb.my.erp.net
Authorization: Bearer <access_token>
TransactionId: fd5d3bbc38ae4dd9a8a5c0ff46c8e3af
Accept: application/json
```

#### Read the response

This example response shows changes to the sales order and its lines. Returned fields depend on which rules run.

```json
{
  "@odata.context": "https://demodb.my.erp.net/api/domain/odata/$metadata#Erp.OpenObject",
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

#### Close the transaction

Commit or roll back with [`EndTransaction`](#endtransaction) and the same transaction ID.

## Limits and expiration

### Concurrent open transactions

`SessionConcurrentTransactions` limits the number of transactions a session can keep open between `BeginTransaction` and `EndTransaction` calls.

| ERP.net version | Default open transactions per session |
|-----------------|--------------------------------------:|
| Before **27.1.1.96** | 1 |
| **27.1.1.96** and later | 10 |

An explicit positive integer in the Domain API site's settings overrides the default on both version ranges and stays in effect after an upgrade. Restart the website after changing the setting. See [Configuring rate limits](https://docs.erp.net/tech/advanced/web-sites/rate-limits.html#configuring-rate-limits).

```json
{
  "RateLimits": {
    "SessionConcurrentTransactions": 10
  }
}
```

An open transaction counts toward the limit between requests. When the limit is reached, `BeginTransaction` returns HTTP 429 with the message "Maximum allowed transactions for this session have been reached." Close an existing transaction before retrying. `SessionConcurrentRequests` separately limits simultaneous HTTP requests.

### Transaction lifespan and management

Treat 25 minutes from `BeginTransaction` as the deadline to finish a transaction. The service discards expired transactions during periodic cleanup, which can run later. End transactions explicitly so the session can reuse the available slot.

If a request returns `Invalid TransactionId`, check the ID against the response from `BeginTransaction`. A closed transaction or one already removed by cleanup requires a new transaction.
