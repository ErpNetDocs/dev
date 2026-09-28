# Global Action object

`Action` is a global helper supplied to scripts. No import is needed. It provides logging, cancellation, user and session information, HTTP requests, and the [Files SDK](../files/index.md). See the [Action API reference](api-reference.md) for method signatures and context requirements.

## Logging and cancellation

```js
Action.log("Started processing");
Action.error("Validation failed");
Action.cancel("Required data is missing");
```

`log` and `error` create information messages in a separate committed transaction. They are not rolled back with later Domain changes in the script. `cancel` stops execution by throwing an exception. A managed script can catch errors if it needs to return a controlled result, but it should not silently discard failures.

## Current user and session

```js
const userId = Action.user.id;
const userName = Action.user.name;
const sessionId = Action.session.id;
```

These values may be `null` when the execution context has no corresponding user or session. They describe the current invocation; they are not an authorization substitute. See [Security and permissions](../concepts/security.md).

## Notifications

`Action.notify.user(userId, message)` creates an immediate notification associated with the current entity. It requires an entity execution context, such as an entity-triggered business rule:

```js
Action.notify.user("6dc7e681-8b65-4095-beb1-b3bc0c948b7c", "Please review this record.");
```

**This helper is not available to a managed script invoked with a transaction context**, even if that script retrieves an entity through `Domain`. For managed scripts, use a suitable Domain operation when the product workflow requires notification.

## HTTP requests

`Action.http` can call an external service through the configured host. The host's network policy still applies:

```js
const response = Action.http.get("https://api.example.com/status");
if (!response.isSuccess)
    throw new Error("Service returned " + response.statusCode);

const responseBody = response.body;
```

`Action.http.post(url, body, headers)` sends a string body. Use explicit content-type headers when posting JSON; check the response before using it. Avoid hard-coding secrets in script source.

## Files and documents

`Action.files` is the transaction-bound interface for file and folder operations. Its `xlsx` and `pdf` members create and modify documents without script-side imports. See [Files SDK](../files/index.md), [XLSX workbooks](../files/xlsx.md), and [PDF documents](../files/pdf.md).
