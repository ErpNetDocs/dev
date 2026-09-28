# Concepts

ERP.net has three persisted script definitions and one freeform execution request:

- A **user business rule** defines code for a configured event.
- A **calculated attribute** defines code that produces an attribute value.
- A **managed script** is a `Systems.Core.Script` record invoked explicitly with declared parameters.
- **ExecuteScript** is an ad hoc Domain API request, not a persisted definition.

The distinction affects what data a script receives and what its caller gets back. See [Execution context](../execution-context.md), [Transactions and results](transactions-and-results.md), and [Security and permissions](../security.md) before adapting a script from one feature to another.
