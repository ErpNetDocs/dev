# Getting Started

Start with the feature you are configuring or calling. Each walkthrough connects the definition or request, the code, its available values, and the result. They are separate starting points, not steps in one sequence.

- [User business rules](user-business-rules.md) run a script when a configured business-rule event occurs.
- [Calculated attributes](calculated-attributes.md) evaluate a script to produce an attribute value.
- [ExecuteScript](execute-script.md) accepts one-off JavaScript through the Domain API.
- [Managed scripts](managed-scripts.md) store a callable definition in `Systems.Core.Script` with declared inputs and results.

After a walkthrough, use [Scripting APIs](../apis.md) to find operations you can call **inside** the script. [Shared patterns](../examples/shared-patterns.md) shows reusable code fragments; unlike the walkthroughs, those fragments are not complete definitions or requests. If you are moving code between features, check [Execution context and results](../concepts/execution-context-and-results.md) first: `subject`, `args`, and results are not interchangeable.
