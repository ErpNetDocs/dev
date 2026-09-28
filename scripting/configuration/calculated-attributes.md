# Configure a calculated attribute script

Create or edit a calculated attribute for the repository whose values it computes. Configure the attribute's result type, select `JavaScript` as its script language, and put a JavaScript function body in `ScriptText`.

The current entity is `subject`. Use a top-level `return` to produce the value; returning nothing or `null` produces a null calculated value. This is not a `Systems.Core.Script`: it has no managed-script `args`, `ParametersSchema`, or output envelope.

Calculated values may be requested repeatedly. Keep queries narrow and limit fetched rows. Start with [Your first calculated attribute script](../getting-started/calculated-attributes.md), then see the [calculated-attribute examples](../examples/calculated-attributes.md). The [Technical Documentation](https://docs.erp.net/tech/advanced/calculated-attributes/index.html) covers the attribute's product configuration and other calculation methods.
