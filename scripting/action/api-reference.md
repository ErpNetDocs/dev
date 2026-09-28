# Action API reference

The global `Action` object is available to JavaScript scripts. Methods use lower-case JavaScript names. The available behavior depends on the script's [execution context](../execution-context.md).

| Member | Purpose | Context note |
| --- | --- | --- |
| `Action.log(message)` | Create an informational message. | Commits the message in a separate transaction. |
| `Action.error(message)` | Create an error information message. | Commits the message in a separate transaction. |
| `Action.cancel(reason?)` | Throw and stop script execution. | Optional reason; no reason uses a generic message. |
| `Action.notify.user(userId, message)` | Send an immediate notification. | Requires an entity context; not usable by transaction-context managed scripts. |
| `Action.user` | Read current-user information. | Individual fields can be `null`. |
| `Action.session` | Read current-session information. | Individual fields can be `null`. |
| `Action.http.get(url, headers?)` | Send an HTTP GET request. | Subject to host networking policy. |
| `Action.http.post(url, body, headers?)` | Send an HTTP POST request. | `body` and `headers` are strings. |
| `Action.files` | Read, query, create, or edit files and folders. | Requires a Domain transaction; see [Files SDK](../files/index.md). |

## User and session properties

`Action.user` exposes `id`, `name`, `roles`, `locale`, and `email`. `roles` is a comma-separated string. `Action.session` exposes `id` and `startedAt`; the latter is a UTC ISO 8601 string when available.

```js
const userName = Action.user.name;
const locale = Action.user.locale;
const startedAt = Action.session.startedAt;
```

These properties describe the current transaction's user and session; a missing value is not proof that access is unrestricted.

## HTTP response

`Action.http.get` and `Action.http.post` return an object with `isSuccess`, `statusCode`, `body`, and `errorMessage`:

```js
const response = Action.http.post(
    "https://api.example.com/items",
    JSON.stringify({ value: 42 }),
    "Content-Type: application/json");

if (!response.isSuccess)
    throw new Error(response.errorMessage || "HTTP " + response.statusCode);

const responseBody = response.body;
```

Multiple HTTP headers are separated by newline characters in the headers string. `isSuccess` indicates a 2xx status. Do not put credentials into scripts unless their storage and access are separately secured.

For method behavior and contextual examples, see the [Action guide](index.md). For file-specific methods, see the [Files SDK](../files/index.md).
