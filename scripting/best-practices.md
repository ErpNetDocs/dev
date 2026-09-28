# Limitations and best practices

## Execution limits and access

Scripts run with the current invocation's Domain transaction and permissions, not unrestricted host access. They can use only the objects and host actions exposed to the scripting runtime. Do not confuse the `subject` of a business rule or calculated attribute with a managed script's `args`; see [Execution context](execution-context.md).

Execution limits depend on the feature and its host. A managed script may tighten platform limits for time, memory, source, input/output, and processed files with [execution settings](execution-settings.md), but cannot relax them. Large loops, unbounded queries, and oversized files may fail even when the script source is valid.

## Data and transactions

- Filter Domain queries and specify a reasonable `fetch` limit. The scripting query interface is restricted; validate that a filter actually narrows the result. See [Query Domain entities](domain-model/queries.md).
- Return plain JSON-compatible values from managed scripts. Do not return live Domain entities or rely on their in-memory identity after the call.
- Domain create, update, delete, and Files SDK changes participate in the caller's transaction. They persist only when it commits. `Action.log()` and `Action.error()` commit their messages separately; external HTTP calls also cannot be rolled back with Domain data.
- Check `getById` and file lookups for `null` before using the result. Treat external IDs and names as untrusted input.

## Reliability and security

- Use `Action.log()` and `Action.error()` for useful operational diagnostics, but do not log tokens, credentials, or sensitive file contents.
- Use `Action.cancel()` when a rule must stop the operation. For a managed script, choose whether to throw an error or return a declared result; do not silently turn failures into success.
- Keep `exec` access limited to clients that genuinely need arbitrary script execution. A valid scope does not bypass ordinary Domain permissions; see [Security and permissions](security.md).
- Do not assume a script can access a host file system, import a package, or use a particular font. Use [Files SDK](files/index.md) for managed content and check document-layer requirements.

JavaScript is the only supported language for managed scripts. Other entry points can have legacy C# behavior; consult the specific entry point's documentation rather than copying a managed-script example into it unchanged.
