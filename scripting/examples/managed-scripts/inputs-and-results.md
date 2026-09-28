# Managed script inputs and results

Set up an active script as described in the [managed-script examples overview](index.md). The request below is the JSON body for its `Execute` action.

## Calculate a value and update an argument

This example demonstrates an input, an input/output parameter, an output parameter, and a return value. Set `ScriptText` to:

```js
args.doubled = args.inputValue * 2;
args.counter += 1;
return { success: args.inputValue === 21 };
```

Set `ParametersSchema` to:

```json
{
  "parameters": [
    { "name": "inputValue", "direction": "Input", "schema": { "type": "integer" } },
    { "name": "counter", "direction": "InputOutput", "schema": { "type": "integer" } },
    { "name": "doubled", "direction": "Output", "schema": { "type": "integer" } },
    { "name": "result", "direction": "Return", "schema": { "type": "object" } }
  ]
}
```

Request body:

```json
{ "arguments": { "inputValue": 21, "counter": 4 } }
```

Relevant response:

```json
{
  "parameters": { "counter": 5, "doubled": 42 },
  "returnValue": { "success": true }
}
```

The `Input` value is not repeated in the result. Only `Output` and `InputOutput` values appear under `parameters`.
