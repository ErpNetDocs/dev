# Calculated attribute examples

Each snippet is a JavaScript function body for a calculated attribute. The current entity is `subject`, and the top-level `return` value becomes the attribute value. Configure the attribute's result type to match the value produced. There is no managed-script `args` object or result envelope.

## Show a customer's active status

For a string attribute on `Crm.Sales.Customers`:

```js
return subject.Active ? "Active" : "Inactive";
```

## Detect a whole-number quantity

For a Boolean attribute on `Crm.Sales.SalesOrderLines`:

```js
if (subject.Quantity == null)
    return null;

return subject.Quantity.Value % 1 === 0;
```

If the quantity is absent, the calculated value is null rather than false.

## Check whether a customer has sales orders

For a Boolean attribute on `Crm.Sales.Customers`:

```js
const orders = Domain.Crm.Sales.SalesOrdersRepository.query(
    { customer: subject }, { fetch: 1 });
return orders.Count > 0;
```

Only one matching row is needed for this question. Calculated attributes can be evaluated repeatedly in lists and reports; keep queries narrow and bounded.

## Read a configured custom property

For a string attribute on `Crm.Sales.Customers` when `CustomerTier` is configured:

```js
const tier = subject.CustomProperties["CustomerTier"];
return tier == null ? null : tier.Value;
```

The property must exist for the entity. See [Domain special types](../domain-model/special-types.md) for reading and updating custom properties, and the [Technical Documentation](https://docs.erp.net/tech/advanced/calculated-attributes/examples/index.html) for other calculated-attribute scenarios.
