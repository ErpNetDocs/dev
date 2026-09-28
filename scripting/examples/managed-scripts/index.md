# Managed script examples

These examples use the stored `Systems.Core.Script` feature. For each example, create an active record with a unique `Code` and `Name`, set `ScriptLanguage` to `JavaScript`, put the JavaScript in `ScriptText`, and put its JSON declaration in `ParametersSchema`. Call the record's `Execute` action with the shown JSON request body. See [Your first managed script](../../getting-started/managed-scripts.md) for the full HTTP request and response format.

- [Inputs and results](inputs-and-results.md) shows all parameter directions and the response envelope.
- [Domain data](domain-data.md) reads, queries, and changes customer records.
- [Files and documents](files-and-documents.md) edits text files, creates XLSX and PDF files, and attaches a file to a customer.

Replace sample IDs with records from your instance. The caller needs the [`exec` scope](../../concepts/security.md) and the X21 Advanced BPM license. An `Execute` result does not commit Domain changes; for examples that edit data or files, [commit the transaction](../../concepts/transactions-and-persistence.md#commit-a-managed-script-call).
