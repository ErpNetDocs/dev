# Parameters and results

`ParametersSchema` is a JSON object containing a `parameters` array. Each parameter has a unique `name`, a `direction`, an optional `description`, and an optional `schema` object for its value. A `default` belongs inside `schema`.

| Direction | Supplied by caller? | Read or written by script? | Included in `parameters` result? |
| --- | --- | --- | --- |
| `Input` | Yes, unless it has a default | Read from `args` | No |
| `Output` | No | Set `args.name` | Yes |
| `InputOutput` | Yes, unless it has a default | Read and update `args.name` | Yes |
| `Return` | No | Use JavaScript `return` | No; appears as `returnValue` |

At most one `Return` parameter may be declared. An omitted `ParametersSchema` means no inputs are declared; supplied argument properties are rejected. A script may still return a value without a `Return` declaration, but that value is not schema-validated.

## Example: inputs, outputs, and a default

```json
{
  "parameters": [
    {
      "name": "fileId",
      "direction": "Input",
      "description": "File to process.",
      "schema": { "type": "string", "format": "uuid" }
    },
    {
      "name": "counter",
      "direction": "InputOutput",
      "schema": { "type": "integer", "default": 0, "minimum": 0 }
    },
    {
      "name": "pageCount",
      "direction": "Output",
      "schema": { "type": "integer", "minimum": 0 }
    },
    {
      "name": "result",
      "direction": "Return",
      "schema": { "type": "boolean" }
    }
  ]
}
```

For this schema, a caller can send `{ "fileId": "<uuid>" }`; `counter` starts at `0`. The script must set `args.pageCount` to an integer before it completes. It can update `args.counter` and return `true` or `false`. An `Output` value is initialized to `null`, but its declared type is checked after execution: leaving `pageCount` at `null` fails validation.

```js
const file = Action.files.getFileById(args.fileId);
if (file === null)
    throw new Error("File not found.");

args.counter += 1;
args.pageCount = 1;
return true;
```

The caller receives `parameters.counter`, `parameters.pageCount`, and `returnValue`; it does not receive `fileId` in the output envelope.

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

That request is valid for a script with no required inputs, or where every input has a default. See the [Domain API action](../domain-api/operations/execute-managed-script.md) for the complete HTTP shape.
