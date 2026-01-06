# ExecuteScript

@@name Domain API defines an `ExecuteScript` endpoint which can be used to execute JavaScript code directly in the domain context.

`ExecuteScript` is an **unbound OData action**, invoked via **HTTP POST**, whose request body contains **raw JavaScript source code**. The script runs server-side in a sandboxed runtime with access to the Domain Model and may query, create, update, or delete domain data.

---

## Endpoint

```http
POST /api/domain/odata/ExecuteScript
```

## Request body

**Required**.
The request body must contain **plain JavaScript source code**, encoded as UTF-8 text.

> [!WARNING]
> - The body is **not JSON**
> - There are **no parameters**
> - The entire request stream is interpreted as JavaScript
>
> If the body is empty or whitespace, the action fails.

## Example

```http
POST https://<your-instance>.my.erp.net/api/domain/odata/ExecuteScript
Content-Type: text/plain
Accept: application/json

console.log('Starting script');

// Example: deactivate all active customers
var customers = Domain.Crm.Sales.CustomersRepository.query({
    active: { equals: true }
});

const customersCount = customers.length;
for (var i = 0; i < customersCount; i++) {
    customers[i].Active = false;
}

console.log(`Processed ${customersCount} customers.`)
```

## Transaction control from JavaScript

The script runtime exposes a global `Transaction` object, which allows the script to explicitly control transaction boundaries. You can start a transaction, commit changes, or roll everything back directly from JavaScript:

```js
Transaction.begin();
Transaction.commit();
Transaction.rollback();
```

Use `Transaction.rollback()` to safely discard changes when you detect invalid data or want a "dry run" style execution. Use `Transaction.commit()` only when you are sure all changes are consistent and should be persisted.

## Result

On success, the action returns a JSON object with execution metadata and captured console output.

**Example response**

```json
{
  "ok": true,
  "sessionId": "f3b2c8a7...",
  "transactionId": "9d1a4e2c...",
  "durationMs": 37,
  "console": "Starting script\n
    Processed 128 customers"
}
```

**Fields**

- **ok** - Always `true` on success
- **sessionId** - Current session identifier
- **transactionId** - Current in-memory transaction identifier, if any
- **durationMs** - Script execution time in milliseconds
- **console** - Output collected from the global `console` object, if used

## Error handling

If script execution fails due to:

- empty request body
- JavaScript runtime error
- exceeded runtime constraints

the action returns an OData error response and **no changes are committed**.

---

## Learn More

- [ERP.net Scripting documentation](https://docs.erp.net/tech/advanced/scripting/index.html)
- [Advanced scripting examples](A collection of advanced scripts that showcase powerful automation, integrations, and more.)
