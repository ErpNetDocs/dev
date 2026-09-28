# Execute a managed script

`Systems.Core.Script.Execute` is a **bound OData action** that runs one stored, active [managed script](../../scripting/managed-scripts.md). Unlike the unbound [`ExecuteScript`](execute-script.md) action, it does not accept JavaScript source in the request body. Its source and optional [parameter schema](../../scripting/parameters.md) come from the selected `Systems.Core.Script` record.

## Request

```http
POST /api/domain/odata/Systems_Core_Scripts(<script-id>)/Execute
Content-Type: application/json
Authorization: Bearer <access-token-with-exec-scope>

{ "arguments": { "quantity": 3, "unitPrice": 12.5 } }
```

`arguments` is a JSON **object**, not an escaped string. The names and values must match the script's schema. Do not send an `argumentsJson` property. For the [line-total example](../../scripting/managed-scripts.md#example-calculate-a-line-total), this request returns `total` as an output and `37.5` as the return value.

The `arguments` property is optional. These two bodies both supply an empty argument object:

```json
{}
```

```json
{ "arguments": null }
```

They work when the script has no required input, or every omitted input has a default. For example, a script whose input schema declares `"default": 3` for `multiplier` can run with `{}`. A required input without a default still causes validation to fail.

## Response

The response is a JSON object. The relevant fields for the line-total example are:

```json
{
  "parameters": { "total": 37.5 },
  "returnValue": 37.5
}
```

OData also includes its usual response metadata. `parameters` contains only declared `Output` and `InputOutput` values. `returnValue` contains the JavaScript return value, or JSON `null` when the body does not return one. See [Parameters and results](../../scripting/parameters.md).

## Authorization and failures

The calling application needs the OAuth [`exec` scope](../../scripting/security.md) and the instance needs the X21 Advanced BPM license. `exec` also authorizes the freeform `ExecuteScript` action, so grant it only to applications that are trusted with that broader capability.

Execution fails for an inactive script, empty source, unsupported managed-script language, invalid arguments, invalid outputs, or exceeded [execution limits](../../scripting/execution-settings.md). The action does not change a script's definition. Ordinary Domain permissions and transaction behavior still apply to data accessed by the script.
