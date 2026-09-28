# Execution settings

Set a managed script's `ExecutionSettings` field to a JSON object to request stricter resource limits. Omit the field or individual properties to use the platform settings. A script cannot raise a platform limit: the lower limit wins.

```json
{
  "timeoutMilliseconds": 4000,
  "memoryLimitMb": 64,
  "maximumScriptSizeKb": 64,
  "maximumInputSizeKb": 32,
  "maximumOutputSizeKb": 128,
  "maximumFileSizeMb": 10
}
```

| Property | What it limits |
| --- | --- |
| `timeoutMilliseconds` | Time spent executing one script. |
| `memoryLimitMb` | Script runtime memory. |
| `maximumScriptSizeKb` | UTF-8 size of the script body. |
| `maximumInputSizeKb` | UTF-8 size of supplied and normalized JSON arguments. |
| `maximumOutputSizeKb` | UTF-8 size of the serialized result envelope. |
| `maximumFileSizeMb` | File content processed through `Action.files`, including XLSX and PDF operations. |

`Kb` and `Mb` use decimal units: 1 KB is 1,000 bytes and 1 MB is 1,000,000 bytes. All values must be positive whole numbers. Unknown or duplicate property names, non-object JSON, and invalid values are rejected.

## Current managed-script defaults

When no stricter setting is supplied, the current production runtime options are:

| Limit | Default |
| --- | ---: |
| Execution timeout | 5 seconds |
| Runtime memory | 200 MB |
| Executable statements | 500,000 |
| Script source | 1,024 KB |
| Serialized input | 256 KB |
| Serialized output | 1,024 KB |
| Processed file | 20 MB |

These are code defaults, not a promise that every host uses the same values. Development/debug builds use a longer default timeout. A deployment can impose a stricter effective limit. The statement count is a platform limit, not an `ExecutionSettings` JSON property.

## Examples

To bound a small calculation without overriding the platform's other limits:

```json
{ "timeoutMilliseconds": 2000, "maximumInputSizeKb": 8 }
```

If the platform timeout is already one second, this does **not** grant two seconds. The effective timeout remains one second. Conversely, if the platform permits more time, this script is restricted to two seconds.

An output-size limit applies to the complete JSON result, not only to `returnValue`. A file-size limit applies when a script reads or writes embedded file content; listing file metadata does not load the file contents. See the [Files SDK](../files/index.md) for file operations.

Limits are enforcement boundaries, not performance targets. Keep queries and result sets small even when their serialized JSON fits within a configured limit. Scripts cannot override the platform's statement limit or enable additional runtime features through this JSON object.
