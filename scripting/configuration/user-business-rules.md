# Configure a user business rule script

Create or edit a [User Business Rule](https://docs.erp.net/model/entities/Systems.Bpm.UserBusinessRules.html). Configure the repository and event that trigger it, select `JavaScript` as its script language, and put the code in `ScriptText`. Complete the rule's other required settings, save it, and activate it when ready.

The script executes when the configured event occurs. Its `subject` is the entity involved in that event; there is no `args` parameter schema. A rule can inspect or change Domain objects and can use `Action.cancel` to reject an operation. Its changes remain subject to the triggering operation's transaction.

For a first test, use `Action.log("Rule triggered");` and confirm the information message. Then follow [Your first user business rule script](../getting-started/user-business-rules.md) or the [business-rule examples](../examples/user-business-rules.md). The [Technical Documentation](https://docs.erp.net/tech/advanced/user-business-rules/index.html) remains the product reference for rule events and conditions.
