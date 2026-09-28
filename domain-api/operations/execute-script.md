# ExecuteScript

@@name Domain API defines an `ExecuteScript` endpoint which can be used to execute JavaScript code directly in the domain context.

`ExecuteScript` is an **unbound OData action**, invoked via **HTTP POST**, whose request body contains **raw JavaScript source code**. The JavaScript runs in the current Domain transaction, with access to the Domain Model and the scripting APIs exposed by the application. It can read and modify domain data, subject to the caller's permissions. A standalone call does not implicitly commit changes.

This is one-off execution, not a stored script definition. To run an active `Systems.Core.Script` with declared JSON arguments and a result, use [Execute a managed script](execute-managed-script.md) instead.

The calling application needs the OAuth [`exec` scope](../../auth/concepts/scopes.md), and the instance needs the X21 Advanced BPM license. The same scope allows both freeform and managed-script execution; request it only for applications that need this capability.

---

## Endpoint

```http
POST /api/domain/odata/ExecuteScript
```

## Request body

**Required.**
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
Authorization: Bearer <access-token-with-exec-scope>

console.log('Starting script');

// Read at most ten active customers.
const customers = Domain.Crm.Sales.CustomersRepository.query({
    active: { equals: true }
}, { fetch: 10 });

console.log(`Fetched ${customers.Count} active customers.`);
```

## Transaction control from JavaScript

The script runtime exposes a global `Transaction` object for the current Domain transaction. In a standalone `ExecuteScript` call, pending Domain changes are **not committed automatically**. Call `Transaction.commit()` in the script if they should persist, or `Transaction.rollback()` to discard them. For example:

```js
const customer = Domain.Crm.Sales.CustomersRepository.getById("<customer-id>");
if (customer === null)
    throw new Error("Customer not found.");

customer.Active = false;
Transaction.commit();
```

Replace `<customer-id>` with an actual customer GUID. `Transaction.begin()` resets the current transaction by rolling it back; it does not open an independent nested transaction. Use these operations deliberately: a later rollback cannot undo changes already committed. Do not call them from a script running in an [externally managed transaction](#external-transaction-control).

## External transaction control

`ExecuteScript` can execute **inside an externally managed Domain API transaction**.

If a transaction has been started beforehand using the `BeginTransaction` unbound action and its `TransactionId` is provided in the request headers, the script runs **within that existing transaction**. In this case:

- All changes are applied to the in-memory transaction dataset.
- The script does not implicitly commit or end the transaction.
- Final persistence is controlled externally via `EndTransaction` with `commit: true` or `commit: false`.

This allows `ExecuteScript` to be composed with other Domain API operations as part of a larger transactional workflow, where transaction lifecycle (begin, commit, rollback) is managed outside the script.

> [!WARNING]
> **External transaction interaction**
>
> When `ExecuteScript` runs inside a transaction started via `BeginTransaction`, the global `Transaction` object operates on that same transaction.
>
> Calling `Transaction.begin()`, `Transaction.commit()`, or `Transaction.rollback()` from the script will directly affect the external transaction and may interfere with its lifecycle.  
> This usage is **not recommended**. When a transaction is managed externally, control it only via the Domain API transaction actions.

For more details about transaction lifecycle and management, see the [Domain API Transactions](../data-manipulation/transactions.md) documentation.

## Result

On success, the action returns a JSON object with execution metadata and captured console output.

**Example response**

```json
{
  "ok": true,
  "sessionId": "f3b2c8a7...",
  "transactionId": "9d1a4e2c...",
  "durationMs": 37,
  "console": "Starting script\nProcessed 128 customers"
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

the action returns an OData error response and does not automatically commit pending Domain changes. An earlier explicit `Transaction.commit()` or external side effect cannot be undone by a later script error.

---

## Learn More

- [Developer scripting guide](../../scripting/index.md)
- [Scripting product overview](https://docs.erp.net/tech/advanced/scripting/index.html)
- [Advanced scripting examples](https://github.com/erpnet/JavaScriptExamples)
