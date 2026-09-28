# Domain special types

Some Domain attributes use types richer than JavaScript strings and numbers. The scripting environment exposes them through `Domain.Types`.

## Custom properties

For an entity with a configured custom property `CustomerTier`, its current value can be read and updated through `CustomProperties`:

```js
const oldValue = customer.CustomProperties["CustomerTier"].Value;
customer.CustomProperties["CustomerTier"] = "Platinum";
```

To set both a value and its description, use `CustomPropertyValue`:

```js
customer.CustomProperties["CustomerTier"] =
    new Domain.Types.CustomPropertyValue("Platinum", "Highest tier");
```

In a managed script, obtain `customer` through a repository using an ID from `args`. In a customer business rule, it might be the entry point's `subject`. The custom property must exist for that entity; see [stored attributes](https://docs.erp.net/tech/advanced/stored-attributes/index.html).

## Multilanguage strings

```js
const description = new Domain.Types.MultilanguageString({
    en: "Preferred customer",
    bg: "Предпочитан клиент"
});
const english = description["en"];
```

`MultilanguageString` is immutable. Construct another value to change a translation. A multilingual description can also be passed to `CustomPropertyValue`.

```js
customer.CustomProperties["CustomerTier"] =
    new Domain.Types.CustomPropertyValue("Platinum", description);
```

## Amounts and quantities

`Amount` combines a number with a currency object; `Quantity` combines a number with a unit object. Obtain the currency or unit from the Domain Model before constructing either type:

```js
const amount = new Domain.Types.Amount(10, currency);
const total = Domain.Types.Amount.Multiply(amount, 5);

const quantity = new Domain.Types.Quantity(10, unit);
const smallerQuantity = new Domain.Types.Quantity(5, unit);
const ratio = Domain.Types.Quantity.Divide(quantity, smallerQuantity);
```

`currency` and `unit` above stand for existing Domain objects, not strings such as `"EUR"` or `"pcs"`. See the [Domain Model reference](https://docs.erp.net/model/entities/index.html) for the relevant entity and attribute types.
