# ErpLogin Authentication (Legacy)

The Domain API accepts the legacy **ErpLogin** scheme: a client posts a user and password to `POST /api/domain/login` and receives an opaque session id, then sends it as an `Authorization: ErpSession <id>` header on subsequent calls.

> [!CAUTION]
> ErpLogin is a **legacy** authentication scheme, kept only for backward compatibility and expected to be retired in a future release. It couples authentication to a user password instead of to a registered application identity.
> New integrations should use **OAuth 2.0** - Client Credentials for services, or PAT/SAT for token-based access. See [Migrating from Legacy Authentication](../../auth/configuration/migrating-from-legacy-auth.md).

Each ErpLogin session is a live @@name session and occupies one **license slot** while active. For the general relationship between tokens, sessions, and licenses, see [Sessions overview](../../auth/sessions/overview.md).

## Login

```http
POST /api/domain/login HTTP/1.1
Host: testdb.my.erp.net
Content-Type: application/json

{
  "user": "svc.integration",
  "pass": "hunter2",
  "app":  "my-integration",
  "lang": "en"
}
```

The response returns the session id to use on subsequent requests:

```json
{
  "authorization": "ErpSession 4f9c0c7c-...-9c1f"
}
```

```http
GET /api/domain/odata/Crm_Sales_Customers HTTP/1.1
Host: testdb.my.erp.net
Authorization: ErpSession 4f9c0c7c-...-9c1f
```

## Logout and session retention

As of **v27.1**, `POST /api/domain/logout` no longer terminates the session or releases its license slot. The call still requires a valid session and returns `200 OK`, but the session is **retained** and follows the normal inactivity timeout: it self-releases only after **20 minutes** without any API request (sliding expiration).

```http
POST /api/domain/logout HTTP/1.1
Host: testdb.my.erp.net
Authorization: ErpSession 4f9c0c7c-...-9c1f
```

## Reusing a session on re-login

While a session is still within its inactivity window, a later `POST /api/domain/login` with the **same credentials** - database, application, user, and password - reuses the existing session and its license slot instead of opening a new one. The returned session id is the one already in use.

Changing any of those values, for example rotating the password, does not match the existing session and starts a fresh one.

> [!NOTE]
> Because logout no longer frees a slot on demand, rapidly cycling `logout` and `login` does not lower the number of concurrent sessions - each logged-out session keeps its slot until it times out. This is intentional and enforces the licensing rules against session multiplexing. See [License Compliance and Violations](../../auth/sessions/license-compliance.md).
