# Parameters and results

`ParametersSchema` describes the values crossing a managed script's boundary. The caller sends input values in the HTTP `arguments` object. The runtime places them in JavaScript `args`; the script writes output values back to `args` and can use `return` for a separate result. The response contains the outputs under `parameters` and the returned value under `returnValue`. See [Your first managed script](../getting-started/managed-scripts.md) for this complete flow.

The field is a JSON object containing a `parameters` array. Each parameter has a unique `name`, a `direction`, an optional `description`, and an optional `schema` object for its value. A `default` belongs inside `schema`.

| Direction | Supplied by caller? | Read or written by script? | Included in `parameters` result? |
| --- | --- | --- | --- |
| `Input` | Yes, unless it has a default | Read from `args` | No |
| `Output` | No | Set `args.name` | Yes |
| `InputOutput` | Yes, unless it has a default | Read and update `args.name` | Yes |
| `Return` | No | Use JavaScript `return` | No; appears as `returnValue` |

At most one `Return` parameter may be declared. Its name identifies the declaration; it is not an `args` property. An omitted `ParametersSchema` means no inputs are declared; supplied argument properties are rejected. A script may still return a value without a `Return` declaration, but that value is not schema-validated.

## Example: inputs, outputs, and a default

This extends the [first managed script](../getting-started/managed-scripts.md) with a `counter` that starts at `0` when the caller omits it. Set `ParametersSchema` to:

```json
{
  "parameters": [
    {
      "name": "inputValue",
      "direction": "Input",
      "schema": { "type": "integer" }
    },
    {
      "name": "counter",
      "direction": "InputOutput",
      "schema": { "type": "integer", "default": 0, "minimum": 0 }
    },
    {
      "name": "doubled",
      "direction": "Output",
      "schema": { "type": "integer" }
    },
    {
      "name": "result",
      "direction": "Return",
      "schema": { "type": "integer" }
    }
  ]
}
```

Set `ScriptText` to:

```js
args.counter += 1;
args.doubled = args.inputValue * 2;
return args.doubled;
```

The caller supplies `inputValue` inside `arguments` and leaves `counter` out:

```json
{ "arguments": { "inputValue": 21 } }
```

The runtime supplies the default `counter: 0` in `args`. The script changes it to `1` and sets `doubled` to `42`. The relevant response fields are:

```json
{
  "parameters": { "counter": 1, "doubled": 42 },
  "returnValue": 42
}
```

The input `inputValue` is not repeated in the response. `counter` appears because it is `InputOutput`; `doubled` appears because it is `Output`. A declared output starts as `null`. Here, the integer schema rejects `null`, so the script must assign an integer before it finishes.

## Supported validation rules

The current implementation validates these rules on each parameter's **top-level value**:

| Keyword | Supported values or meaning |
| --- | --- |
| `type` | `string`, `integer`, `number`, `boolean`, `object`, `array`, `null`; one type or a nonempty array of types. |
| `format` | `uuid` for a string value. |
| `enum` | Nonempty array of allowed JSON values. |
| `minimum`, `maximum` | Numeric bounds. |
| `minLength`, `maxLength` | String length bounds. |
| `minItems`, `maxItems` | Array length bounds. |
| `default` | Value to use when an `Input` or `InputOutput` argument is omitted. |

For example, `"type": ["string", "null"]` allows a nullable string. The validator does **not** currently recurse into object properties or array items, or enforce `pattern`, nested `required`, and other full JSON Schema keywords. Do not rely on such rules for protection; validate nested data in the script if needed.

## Arguments and errors

Omitting the action's `arguments` parameter or setting it to JSON `null` is equivalent to `{}`. If the schema declares a required input without a default, execution then fails. Supplying an undeclared name, an `Output` or `Return` name, a duplicate name, a non-object argument value, or a value that fails a supported rule also fails. Output and return values are validated after the script runs.

```json
{ "arguments": null }
```

That request is valid for a script with no required inputs, or where every input has a default. See the [Domain API action](../../domain-api/operations/execute-managed-script.md) for the complete HTTP shape.
