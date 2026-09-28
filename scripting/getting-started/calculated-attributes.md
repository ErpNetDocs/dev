# Your first calculated attribute script

A calculated attribute evaluates its script when the attribute's value is needed. The current entity is available as `subject`, and the script's returned value becomes the attribute value.

1. Create or edit a calculated attribute for the `Crm.Sales.Customers` repository.
2. Configure it to return a string and select `JavaScript` as its script language.
3. Set its script text to:

```js
return subject.Active ? "Active" : "Inactive";
```

4. Save the attribute and read its value for a customer.

Calculated-attribute JavaScript is a function body, so a top-level `return` is valid. It does not use a managed-script parameter schema or return envelope. Keep calculations efficient: this value may be requested repeatedly in forms, lists, and reports. See [configuration](../configuration/calculated-attributes.md) and [calculated-attribute examples](../examples/calculated-attributes.md).
