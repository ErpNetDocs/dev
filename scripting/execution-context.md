# Execution context

A script runs inside a Domain transaction. Its available values depend on the [feature](index.md) that hosts it; a script does not acquire another entity or elevated access merely because it can use the `Domain` object.

| Feature | Context and result |
| --- | --- |
| User business rule | `subject` is the triggering entity. The script can change data or cancel the operation. |
| JavaScript calculated attribute | `subject` is the entity being calculated; a top-level `return` produces the attribute value. |
| [ExecuteScript](../domain-api/operations/execute-script.md) | The request supplies source code. There is no automatic `subject`, declared `args`, or persisted parameter schema; the action returns metadata and console output. |
| [Managed script](managed-scripts.md) | Read declared values from `args`; there is no current-entity `subject`. Declared output parameters and a return value are returned to the caller. |

The global `Domain` object exposes repository access, and the global [`Action` object](action/index.md) exposes host helpers. The repository and helper operations run with the permissions and transaction context of the invocation.

For example, a user business rule attached to Customers can read its triggering customer:

```js
Action.log("Processing customer " + subject.Number);
```

The equivalent managed script must receive a customer ID through `args`:

```js
const customer = Domain.Crm.Sales.CustomersRepository.getById(args.customerId);
if (customer === null)
    throw new Error("Customer not found.");
return customer.Number;
```

That managed script needs a declared `customerId` input and a return value; see [Managed script examples](examples.md#read-a-customer-by-id). [Common patterns](examples/common-patterns.md) demonstrates reusable operations without assuming either global. Changes to Domain entities participate in the caller's transaction and persist only if it commits. This does not imply that every host action has the same lifetime; consult the [Action reference](action/api-reference.md) for each helper.
