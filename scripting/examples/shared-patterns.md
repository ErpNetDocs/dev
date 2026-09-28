# Shared patterns

The examples in this section use the JavaScript `Domain` and `Action` APIs shared by the scripting features. Start with [basic Domain operations](basic-domain.md) for retrieval and CRUD, then use [advanced Domain queries](advanced-domain.md) for filter syntax, references, null handling, and object initializers. The [Domain API reference](../domain-model/index.md) explains the underlying operations.

These are reusable fragments, not complete scripts. A user business rule or calculated attribute can receive `subject`; a managed script receives declared values through `args`; a freeform `ExecuteScript` request receives neither automatically. Obtain IDs and Domain objects from the relevant [execution context](../concepts/execution-context-and-results.md). Modifications participate in the current transaction and take effect only if it commits.

## Special Domain types

When `customer` has a configured `CustomerTier` custom property:

```js
const tier = customer.CustomProperties["CustomerTier"].Value;
customer.CustomProperties["CustomerTier"] =
    new Domain.Types.CustomPropertyValue("Platinum", "Highest tier");
```

For a multilingual description, create a new value rather than changing an existing one:

```js
const description = new Domain.Types.MultilanguageString({
    en: "Preferred customer",
    bg: "Предпочитан клиент"
});
customer.CustomProperties["CustomerTier"] =
    new Domain.Types.CustomPropertyValue("Platinum", description);
```

An amount needs a currency object, and a quantity needs a unit object. Neither accepts a currency or unit name in place of the Domain object:

```js
const amount = new Domain.Types.Amount(10, currency);
const total = Domain.Types.Amount.Multiply(amount, 5);

const quantity = new Domain.Types.Quantity(10, unit);
const doubled = Domain.Types.Quantity.Multiply(quantity, 2);
```

See [Domain special types](../domain-model/special-types.md) for their value semantics.

## Log, cancel, and inspect context

`Action` is available without an import:

```js
Action.log("Processing started");
Action.error("A validation failed");
```

To reject the current operation from a rule:

```js
Action.cancel("This operation is not allowed.");
```

User and session information can be absent in some invocation contexts:

```js
const userId = Action.user.id;
const sessionId = Action.session.id;
```

`Action.log` and `Action.error` write their messages in a separate committed transaction. See the [Action reference](../action/api-reference.md) for other fields and operations.

## Notify a user

`Action.notify.user` requires an entity execution context, such as an entity-triggered business rule. Retrieving an entity inside a managed script does not by itself provide that context:

```js
Action.notify.user(
    "6dc7e681-8b65-4095-beb1-b3bc0c948b7c",
    "Please review this record.");
```

## Call an HTTP service

`Action.http` uses the host's network policy. Check the response and do not embed secrets in script source:

```js
const response = Action.http.get("https://api.example.com/status");
if (!response.isSuccess)
    throw new Error("HTTP " + response.statusCode);

Action.log(response.body);
```

For a JSON POST, serialize the request and set its content type:

```js
const response = Action.http.post(
    "https://api.example.com/items",
    JSON.stringify({ value: 42 }),
    "Content-Type: application/json");
```

## Create and update an attached file

An attached file needs a folder or entity owner. This example creates a root folder, then a text file in it:

```js
const folder = Action.files.createFolder({ name: "Exports" });
const file = Action.files.createFile({
    folder,
    name: "result.txt",
    mediaType: "text/plain",
    content: "First result"
});
file.writeText("Updated result");
```

The file belongs to the current Domain transaction. For pagination, entity-owned files, XLSX, and PDF, see the [Files SDK](../files/index.md).
