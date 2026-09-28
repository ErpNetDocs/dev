# User business rule examples

Each example is script text for a user business rule. Configure the indicated repository and event on the rule; the entity involved in that event is `subject`. These are not managed scripts and do not use `args` or a parameter schema. See [configuration](../configuration/user-business-rules.md) for the rule setup.

## Reject a fractional sales-order quantity

Configure a rule for `Crm.Sales.SalesOrderLines` on a suitable commit event:

```js
if (subject.Quantity != null && subject.Quantity.Value % 1 !== 0) {
    Action.cancel("Enter a whole-number quantity.");
}
```

`Action.cancel` rejects the operation with the supplied message. This is the scripting form of the [whole-quantity validation scenario](https://docs.erp.net/tech/advanced/user-business-rules/examples/whole-quantity-validation.html).

## Log the current document number

For a rule on `Crm.Sales.SalesOrders`:

```js
Action.log("Processing sales order " + subject.DocumentNo);
```

The information message is written separately from the Domain transaction. Do not log confidential document data.

## Query orders related to the triggering customer

For a rule on `Crm.Sales.Customers`:

```js
const orders = Domain.Crm.Sales.SalesOrdersRepository.query(
    { customer: subject }, { fetch: 2 });

if (orders.Count > 0)
    Action.log("Customer has at least one sales order.");
```

The query uses `subject` as a Domain reference. `fetch: 2` keeps the result bounded; use a larger limit only if the rule really needs more rows.

## Set a custom property on the current entity

When a `Crm.Sales.Customers` rule has a configured `CustomerTier` custom property:

```js
if (subject.Active) {
    subject.CustomProperties["CustomerTier"] =
        new Domain.Types.CustomPropertyValue("Active", "Active customer");
}
```

The property change participates in the operation that triggered the rule. See [special types](../domain-model/special-types.md) for multilingual descriptions.

## Notify a user

An entity-triggered business rule can use `Action.notify.user`:

```js
Action.notify.user(
    "6dc7e681-8b65-4095-beb1-b3bc0c948b7c",
    "Please review the current record.");
```

Replace the sample GUID with a real user ID. This helper needs an entity context; it is not generally available to a transaction-context managed script. See the [Action guide](../action/index.md).
