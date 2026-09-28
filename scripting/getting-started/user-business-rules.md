# Your first user business rule script

A user business rule owns an event-driven script. The rule, not an API request, determines when it runs. Its `subject` is the entity that triggered the event.

1. Create or edit a [User Business Rule](https://docs.erp.net/model/entities/Systems.Bpm.UserBusinessRules.html) for the `Crm.Sales.SalesOrderLines` repository.
2. Configure the rule's event, for example `COMMIT`, and complete the other required rule settings.
3. Select `JavaScript` as the script language and enter this script text:

```js
if (subject.Quantity != null && subject.Quantity.Value % 1 !== 0) {
    Action.cancel("Enter a whole-number quantity.");
}
```

4. Save and activate the rule, then try to save a line with a fractional quantity.

The script uses the triggering line directly; there is no managed-script `args` object or parameter schema. `Action.cancel` stops the operation with the given message. The [configuration guide](../configuration/user-business-rules.md) explains the rule fields, and [business-rule examples](../examples/user-business-rules.md) show other event-driven patterns.
