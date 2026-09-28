# Scripting

ERP.net runs JavaScript in several product features. The feature determines **when the code runs, what values it receives, and what happens to its result**. There is no single script definition used by all four features.

| Feature | Where the script is defined | When it runs | Main input or result |
| --- | --- | --- | --- |
| [User business rule](getting-started/user-business-rules.md) | On a user business rule | When its configured event occurs | The triggering entity is `subject`; the script can validate or change data. |
| [Calculated attribute](getting-started/calculated-attributes.md) | On a calculated attribute | When its value is evaluated | The current entity is `subject`; the script returns the attribute value. |
| [ExecuteScript](getting-started/execute-script.md) | JavaScript source in a Domain API request | When the action is called | There is no stored definition or declared `args` schema. |
| [Managed script](getting-started/managed-scripts.md) | In a `Systems.Core.Script` record | When its `Execute` action is called | Declared inputs are in `args`; outputs and a return value are returned to the caller. |

The similarly named API actions accept different requests: `ExecuteScript` receives JavaScript source, while `Systems.Core.Script/Execute` receives a script record ID and JSON arguments. Neither action is how a user business rule or calculated attribute is triggered.

In every case, the script can use the [Domain Model](domain-model/index.md) to work with ERP.net entities and the [`Action` object](action/index.md) for supported host operations. What differs is the context provided by the feature: `subject` is available to business rules and calculated attributes, while a managed script reads declared inputs from `args`. A freeform `ExecuteScript` call provides neither automatically.

Managed scripts currently support JavaScript only. Other features may retain legacy C# behavior; consult their own documentation.

## Follow a complete example

If you are working with the new `Systems.Core.Script` entity, start with [Your first managed script](getting-started/managed-scripts.md). It follows one value from the script record and parameter schema, through the HTTP request and `args`, to the response. Then use [Managed scripts](configuration/managed-scripts.md) and [Parameters and results](configuration/parameters.md) as references. For another feature, open its own Getting Started page from the table above; its inputs and results are different.

## Read by topic

- [Getting Started](getting-started/index.md) shows how to use each feature from definition to result.
- [Concepts](concepts/execution-context-and-results.md) explains what the features share and where they differ.
- [Configuration](configuration/index.md) covers the fields and settings for each stored definition.
- [Scripting APIs](apis.md) documents the Domain Model, `Action`, and the Files SDK used inside script bodies.
- [Examples](examples/index.md) distinguishes reusable code fragments from complete, feature-specific examples.
- [Limitations and best practices](best-practices.md) covers reliability and resource use.

The older [Technical Documentation scripting overview](https://docs.erp.net/tech/advanced/scripting/index.html) remains available for existing links. This guide is the maintained authoring reference.
