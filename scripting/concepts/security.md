# Security and permissions

Running scripts can read and modify Domain data. Review stored and caller-supplied scripts before execution, and write scripts with the same care as other code that changes business data.

## Authorization and licensing

The following execution operations are available starting with ERP.net version **27.1.1.96** and require the X21 Advanced BPM license:

- the bound [`Systems.Core.Script/Execute` action](../../domain-api/operations/execute-managed-script.md), which runs a stored, active script; and
- the unbound [`ExecuteScript` action](../../domain-api/operations/execute-script.md), which runs JavaScript source supplied in the request body.

The calling application must authenticate to the Domain API. Request the normal [OAuth scopes](../../auth/concepts/scopes.md) needed for the script's data operations; for example, `update` is required to create, modify, or delete data. Script execution does not require an additional OAuth scope.

Authentication and the license do not waive ordinary Domain access rules or data permissions.

## Execution context

Scripts run in the current Domain transaction with the caller's permissions. Internal system transactions have a different trust context; they are not a substitute for granting an external caller access.

User business rules and calculated attributes have their own triggers and execution contexts. See [Execution context and results](execution-context-and-results.md) when choosing a scripting entry point.

## Practical safeguards

- Request the least set of scopes an application needs for its data operations.
- Keep managed scripts inactive until reviewed; activation is required for execution.
- Declare and validate inputs with [parameter schemas](../configuration/parameters.md), and apply [execution settings](../configuration/execution-settings.md) where stricter limits are appropriate.
- Use the transaction-bound [Files SDK](../files/index.md) for files. Scripts do not gain arbitrary host filesystem access from it.
