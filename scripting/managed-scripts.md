# Managed scripts

A managed script is a reusable `Systems.Core.Script` definition stored in `Sys_Scripts`. It is separate from a user business rule, a calculated attribute, and the freeform Domain API [`ExecuteScript`](../domain-api/operations/execute-script.md) action.

## Define a script

Create a `Systems.Core.Script` record and set these fields:

| Field | Purpose |
| --- | --- |
| `Code` | Unique identifier, at most 16 characters. Database uniqueness is enforced. |
| `Name` | Display name. |
| `IsActive` | Must be enabled before execution. New scripts are inactive by default. |
| `ScriptLanguage` | `JavaScript` is the only supported managed-script language. |
| `ScriptText` | JavaScript function **body**, not a complete function declaration. |
| `ParametersSchema` | Optional JSON declaration of inputs, outputs, and return value. |
| `ExecutionSettings` | Optional JSON limits that may be stricter than platform limits. |
| `Notes` | Optional description for maintainers. |

The body receives an `args` object. Do not declare `function execute(...)` around it, and do not expect parameter names to become global variables. You may use `return` directly in the body.

### Example: calculate a line total

Set `ScriptText` to:

```js
args.total = args.quantity * args.unitPrice;
return args.total;
```

Set `ParametersSchema` to:

```json
{
  "parameters": [
    { "name": "quantity", "direction": "Input", "schema": { "type": "integer", "minimum": 1 } },
    { "name": "unitPrice", "direction": "Input", "schema": { "type": "number", "minimum": 0 } },
    { "name": "total", "direction": "Output", "schema": { "type": "number" } },
    { "name": "result", "direction": "Return", "schema": { "type": "number" } }
  ]
}
```

After saving and activating the script, call its [bound Domain API action](../domain-api/operations/execute-managed-script.md):

```http
POST /api/domain/odata/Systems_Core_Scripts(<script-id>)/Execute
Content-Type: application/json
Authorization: Bearer <access-token>

{ "arguments": { "quantity": 3, "unitPrice": 12.5 } }
```

The relevant response fields are:

```json
{
  "parameters": { "total": 37.5 },
  "returnValue": 37.5
}
```

The actual OData response also contains OData metadata. `Input` values are not repeated in `parameters`; it contains only `Output` and `InputOutput` values. See [Parameters and results](parameters.md) for defaults, missing arguments, and validation.

For more complete managed-script examples, including file editing and document creation, see [Managed script examples](examples.md).

## Execution behavior

- An inactive script, an empty body, or an unsupported language cannot run.
- Each invocation gets its own argument values. Changing `args` in one call does not change a later call.
- Script changes are picked up by preparation caching; a modified in-memory script runs its current source. Cache implementation details are not part of the API contract.
- Script settings can only tighten [platform execution limits](execution-settings.md).
- Circular calls and excessive managed-script nesting are rejected.

Execution requires the [X21 Advanced BPM license and `exec` scope](security.md) for a caller-scoped transaction.
