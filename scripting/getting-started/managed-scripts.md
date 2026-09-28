# Your first managed script

A managed script is a stored `Systems.Core.Script` record. The record holds the code and the definition of its inputs and outputs. A caller invokes that record's `Execute` action, supplies input values, and receives the result. This walkthrough doubles a number so you can see each part of that exchange.

## 1. Define the code

Create a `Systems.Core.Script` record with a unique `Code` and a `Name`. Set `ScriptLanguage` to `JavaScript`, and set `ScriptText` to:

```js
args.doubled = args.inputValue * 2;
return args.doubled;
```

`ScriptText` is the **body** of the script, not a complete JavaScript function. The input will be `args.inputValue`. The code assigns the output to `args.doubled` and also returns that value. Neither `inputValue` nor `doubled` is a standalone global variable.

## 2. Declare the values

Set `ParametersSchema` on the same record to:

```json
{
  "parameters": [
    { "name": "inputValue", "direction": "Input", "schema": { "type": "integer" } },
    { "name": "doubled", "direction": "Output", "schema": { "type": "integer" } },
    { "name": "result", "direction": "Return", "schema": { "type": "integer" } }
  ]
}
```

This schema says that the caller must supply an integer `inputValue`; the script must produce an integer `doubled`; and the value from JavaScript `return` must also be an integer. The name `result` labels the return declaration; the code does not set `args.result`.

Save the record and set `IsActive` to `true`. An inactive script cannot run.

## 3. Call the script

Call `Execute` using the **record ID**, not its `Code`:

```http
POST /api/domain/odata/Systems_Core_Scripts(<script-id>)/Execute
Content-Type: application/json
Authorization: Bearer <access-token-with-exec-scope>

{ "arguments": { "inputValue": 21 } }
```

The JSON `arguments` object becomes the script's `args` object. Here, `args.inputValue` is `21`. Do not send the JavaScript source or an escaped JSON string in this request. The caller needs the [`exec` scope](../concepts/security.md), and execution requires the X21 Advanced BPM license.

## 4. Read the result

The relevant response fields are:

```json
{
  "parameters": { "doubled": 42 },
  "returnValue": 42
}
```

`parameters.doubled` comes from `args.doubled`; `returnValue` comes from `return args.doubled`. The input is not repeated in the response. The actual Domain API response also includes OData metadata.

This calculation does not change Domain data. If a script creates or edits entities or files, `Execute` still does **not** commit those changes; the caller must manage the [transaction](../../domain-api/operations/execute-managed-script.md#transactions). Continue with [Managed scripts](../configuration/managed-scripts.md) for the record fields, [Parameters and results](../configuration/parameters.md) for defaults and validation, and [complete examples](../examples/managed-scripts/index.md) for Domain and file operations.
