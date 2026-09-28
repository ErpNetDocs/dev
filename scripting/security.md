# Security and permissions

Running scripts can read and modify Domain data. Give script execution only to applications that need it, and write scripts with the same care as other code that changes business data.

## The `exec` scope

An application must request and be allowed the OAuth `exec` scope to make a caller-initiated script execution request. The scope applies to **both**:

- the bound [`Systems.Core.Script/Execute` action](../domain-api/operations/execute-managed-script.md), which runs a stored, active script; and
- the unbound [`ExecuteScript` action](../domain-api/operations/execute-script.md), which runs JavaScript source supplied in the request body.

`exec` is an application/token capability, **not** permission for one particular stored script. Requesting it solely to call a selected managed script also enables the freeform action for that token. The trusted application must allow the requested scope; merely putting `exec` in a token request is not enough. See [OAuth scopes](../auth/concepts/scopes.md).

The X21 Advanced BPM license is also required for these execution operations. A valid scope and license do not waive ordinary Domain access rules or data permissions.

## Execution context

Normal caller-scoped transactions carry the caller's granted scopes into model operations, including work performed for that caller. Internal system transactions have a different trust context; they are not a substitute for granting an external caller access.

Executing an existing user business rule or calculated attribute is a separate feature and does not gain a blanket `exec` requirement. If code in such a context explicitly calls the managed `Script.Execute` method, that operation still checks the current transaction's scope context.

## Practical safeguards

- Request the least set of scopes an application needs. Treat `exec` as a broad capability because it includes freeform execution.
- Keep managed scripts inactive until reviewed; activation is required for execution.
- Declare and validate inputs with [parameter schemas](parameters.md), and apply [execution settings](execution-settings.md) where stricter limits are appropriate.
- Use the transaction-bound [Files SDK](files/index.md) for files. Scripts do not gain arbitrary host filesystem access from it.
