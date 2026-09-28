# Scripting

ERP.net supports JavaScript in several distinct features. A script's definition, available context, and result depend on the feature in which it runs. The [Domain Model](https://docs.erp.net/model/entities/) and selected scripting APIs are available across these contexts, but `subject`, `args`, and transaction control are not interchangeable.

| Feature | Where the script is defined | When it runs | Main input or result |
| --- | --- | --- | --- |
| [User business rule](getting-started/user-business-rules.md) | On a user business rule | When its configured event occurs | The triggering entity is `subject`; the script can validate or change data. |
| [Calculated attribute](getting-started/calculated-attributes.md) | On a calculated attribute | When its value is evaluated | The current entity is `subject`; the script returns the attribute value. |
| [ExecuteScript](getting-started/execute-script.md) | JavaScript source in a Domain API request | When the action is called | There is no stored definition or declared `args` schema. |
| [Managed script](getting-started/managed-scripts.md) | In a `Systems.Core.Script` record | When its `Execute` action is called | Declared inputs are in `args`; outputs and a return value are returned to the caller. |

These are separate product features, not interchangeable ways to invoke one script definition. Managed scripts currently support JavaScript only. Other entry points may retain legacy C# behavior; consult their own documentation.

## Read by topic

- [Getting Started](getting-started/overview.md) contains a short, independent walkthrough for each feature.
- [Concepts](concepts/overview.md) explains execution context, transactions, results, and permissions.
- [Configuration](configuration/overview.md) covers where each script is defined and the managed-script parameter and limit settings.
- [Scripting APIs](apis.md) documents Domain access, `Action`, and the Files SDK.
- [Examples](examples/overview.md) separates [common patterns](examples/common-patterns.md) from complete scripts for each feature.
- [Limitations and best practices](best-practices.md) covers reliability and resource use.

The older [Technical Documentation scripting overview](https://docs.erp.net/tech/advanced/scripting/index.html) remains available for existing links. This guide is the maintained authoring reference.
