# Managed scripts with Domain data

Set up an active script as described in the [managed-script examples overview](index.md). Each request below is the JSON body for its `Execute` action.

Managed scripts can access the Domain Model through the global `Domain` object. Unlike a user business rule, a managed script has no current-entity `subject`; pass the entity ID through `args` and retrieve it from its repository. Return plain values rather than Domain objects. The examples below use customers, but the repository and attributes can be changed for other entities. Normal Domain permissions and transaction rules still apply.

## Read a customer by ID

Assume the customer has number `C-001` and is active. Set `ScriptText` to:

```js
const customer = Domain.Crm.Sales.CustomersRepository.getById(args.customerId);
if (customer === null)
    throw new Error("Customer not found.");

return { number: customer.Number, active: customer.Active };
```

Set `ParametersSchema` to:

```json
{
  "parameters": [
    { "name": "customerId", "direction": "Input", "schema": { "type": "string", "format": "uuid" } },
    { "name": "result", "direction": "Return", "schema": { "type": "object" } }
  ]
}
```

Request body (replace the ID with the customer's ID):

```json
{ "arguments": { "customerId": "11111111-1111-4111-8111-111111111111" } }
```

Relevant response for the assumed customer:

```json
{
  "parameters": {},
  "returnValue": { "number": "C-001", "active": true }
}
```

## Find customers by number prefix

This query fetches at most ten customers. Assume the matching numbers are `C-001` and `C-002`. Set `ScriptText` to:

```js
const customers = Domain.Crm.Sales.CustomersRepository.query(
    { number: { startsWith: args.prefix } },
    { fetch: 10 });

const numbers = [];
for (let i = 0; i < customers.Count; i++)
    numbers.push(customers[i].Number);

return numbers;
```

Set `ParametersSchema` to:

```json
{
  "parameters": [
    { "name": "prefix", "direction": "Input", "schema": { "type": "string", "minLength": 1 } },
    { "name": "result", "direction": "Return", "schema": { "type": "array", "maxItems": 10 } }
  ]
}
```

Request body:

```json
{ "arguments": { "prefix": "C-" } }
```

Relevant response for the assumed matches:

```json
{
  "parameters": {},
  "returnValue": ["C-001", "C-002"]
}
```

The `fetch` option bounds the query; the return array contains only customer numbers, not live Domain objects. Do not assume the sample order unless your query defines an ordering.

## Change a customer's active state

Assume the customer is currently active. Set `ScriptText` to:

```js
const customer = Domain.Crm.Sales.CustomersRepository.getById(args.customerId);
if (customer === null)
    throw new Error("Customer not found.");

const wasActive = customer.Active;
customer.Active = args.active;
return { wasActive, isActive: customer.Active };
```

Set `ParametersSchema` to:

```json
{
  "parameters": [
    { "name": "customerId", "direction": "Input", "schema": { "type": "string", "format": "uuid" } },
    { "name": "active", "direction": "Input", "schema": { "type": "boolean" } },
    { "name": "result", "direction": "Return", "schema": { "type": "object" } }
  ]
}
```

Request body (replace the ID with the customer's ID):

```json
{ "arguments": { "customerId": "11111111-1111-4111-8111-111111111111", "active": false } }
```

Relevant response for an initially active customer:

```json
{
  "parameters": {},
  "returnValue": { "wasActive": true, "isActive": false }
}
```

This changes the customer in the current Domain transaction. The change persists only when the caller [commits the transaction](../../concepts/transactions-and-persistence.md#commit-a-managed-script-call); the script does not commit it itself.
