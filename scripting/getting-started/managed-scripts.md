# Your first managed script

A managed script is a stored, callable `Systems.Core.Script` record. Its JavaScript body receives declared parameters through `args` and can return output parameters and a return value.

1. Create a `Systems.Core.Script` with a unique code and a name. Select `JavaScript` and set the script text to:

```js
args.doubled = args.inputValue * 2;
return args.doubled;
```

2. Set its `ParametersSchema` to:

```json
{
  "parameters": [
    { "name": "inputValue", "direction": "Input", "schema": { "type": "integer" } },
    { "name": "doubled", "direction": "Output", "schema": { "type": "integer" } },
    { "name": "result", "direction": "Return", "schema": { "type": "integer" } }
  ]
}
```

3. Save and activate the record. Call its bound Domain API action:

```http
POST /api/domain/odata/Systems_Core_Scripts(<script-id>)/Execute
Content-Type: application/json
Authorization: Bearer <access-token-with-exec-scope>

{ "arguments": { "inputValue": 21 } }
```

The relevant response values are `{ "parameters": { "doubled": 42 }, "returnValue": 42 }`. Unlike `ExecuteScript`, this action does not accept source code in the request. The caller needs the `exec` scope, and execution requires the X21 Advanced BPM license. See [Managed scripts](../managed-scripts.md), [Parameters and results](../parameters.md), and [managed-script examples](../examples.md) for the complete contract.
