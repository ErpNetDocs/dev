# Operations (actions and functions)

In OData, an **operation** is either an **action** or a **function**:

- **Action**: invoked with **HTTP POST**. Actions may have side effects (create/update/delete data).
- **Function**: invoked with **HTTP GET**. Functions should not have side effects and return data.

Operations can be:

- **Unbound**: not tied to a specific entity instance (called on the service root).
- **Bound**: tied to a specific entity instance (called on `/EntitySet(key)/OperationName`).

> [!tip]
> For a complete list of operations supported by a given ERP.net instance, inspect the instance EDM model at `.../api/domain/odata/$metadata`.

---

## Unbound operations (service root)

Unbound operations are invoked directly under:

`/api/domain/odata/<OperationName>`

| Operation | Kind | HTTP | Description | See |
|---|---|---:|---|---|
| `BeginTransaction` | Action | POST | Starts a server-side API transaction and returns `TransactionId`. | [Transactions](transactions.md) |
| `EndTransaction` | Action | POST | Ends an API transaction (commit or rollback). | [Transactions](transactions.md) |
| `GetChanges` | Function | GET | Returns collected changes for a front-end transaction (requires `trackChanges=true`). | [Transactions](transactions.md) |
| `WaitForChanges` | Function | GET | Like `GetChanges`, but blocks until changes occur or timeout. | [Transactions](transactions.md) |
| `Import` | Action | POST | Bulk insert/update/delete multiple objects in a single request. | [Import Data](import.md) |
| `ExecuteScript` | Action | POST | Executes raw JavaScript in the domain context. | [ExecuteScript](../execute-script.md) |

---

## Bound operations (available on all entities)

ERP.net exposes a set of operations declared on the base **EntityObject** type.
They can be invoked for any entity instance:

`/api/domain/odata/<EntitySet>(<id>)/<OperationName>`

| Operation | Kind | HTTP | Purpose | Notes / reference |
|---|---|---:|---|---|
| `CreateNotification` | Action | POST | Creates a notification for a user (and sends a real-time event). | [Create notification](../common-tasks/create-notification.md) |
| `GetAllowedCustomPropertyValues` | Function | GET | Returns the allowed values for a stored attribute (custom property) for the current entity instance. | See the *API Methods* section in the model docs (e.g. [General.Products.Products](https://docs.erp.net/model/entities/General.Products.Products.html)). |
| `CreateCopy` | Action | POST | Duplicates the object and its child objects within the same aggregate. | See the *API Methods* section in the model docs (e.g. [General.Products.Products](https://docs.erp.net/model/entities/General.Products.Products.html)). |

> [!note]
> The exact invocation syntax (parameter passing for functions) follows OData v4 rules and is described in the service `$metadata`.

## See also

- [Transactions](transactions.md)
- [Import Data](import.md)
- [ExecuteScript](../execute-script.md)
- [Stored attributes (custom properties)](../common-tasks/stored-attributes.md)
