# Scripting APIs

JavaScript scripts can use selected ERP.net APIs without importing a package. Which context values are available depends on the feature that runs the script; see [Execution context](concepts/execution-context-and-results.md).

- [Domain Model](domain-model/index.md) provides entity repositories for lookup, queries, and changes.
- [Action](action/index.md) provides logging, cancellation, user/session information, HTTP operations, and file access. Individual helpers have context requirements; see its [API reference](action/api-reference.md).
- [Files SDK](files/index.md) works with ERP.net folders and attached files in a Domain transaction. It also provides native [XLSX](files/xlsx.md) and [PDF](files/pdf.md) processing.

The [Shared patterns](examples/shared-patterns.md) page demonstrates reusable operations without assuming that `subject` or `args` exists in every script.
