# Migrating from Legacy Authentication

@@name still accepts two legacy authentication schemes: **HTTP Basic** and **ErpLogin** (the `POST /domain/login` endpoint that returns an `Authorization: ErpSession <id>` header).

Both schemes continue to be accepted for backward compatibility. They are expected to be retired in a future release. All new integrations - and any integration you actively maintain - should move to one of the supported OAuth 2.0 paths described below.

This topic explains what to migrate to, and how.

## What is being phased out

| Legacy scheme | What the integration sends | What is wrong with it |
|---|---|---|
| **HTTP Basic** | `Authorization: Basic base64(user:password)` on every request. | Sends a user password on every call. No application identity. No scopes. No revocation other than rotating the password. Cannot use 2FA. |
| **ErpLogin** | `POST /domain/login` with `{ "user", "pass", "app", "lang" }`, then `Authorization: ErpSession <id>` on subsequent calls. | Looks like OAuth client_credentials but is not: it takes a user password, returns an opaque session id with no `expires_in`, no `scope`, no refresh, and is not discoverable via the standard OAuth metadata. |

Both schemes share the same root problem: they couple authentication to a **user's password** rather than to an **application identity** registered as a Trusted Application.

## What to migrate to

Pick a target based on what the integration represents:

| Your integration is... | Migrate to | Why |
|---|---|---|
| A server-side daemon, scheduled job, or backend-to-backend integration that should act **as itself** (no human user). | **Client Credentials Flow** (short-lived bearer tokens) or **SAT** (long-lived Service Access Token). | The token represents the application via its configured **System User**. No user password is involved. |
| A backend that should act **on behalf of a specific named user** (for example, a per-user automation the user explicitly authorised). | **PAT** (Personal Access Token). | The token carries the user's identity and claims. The user can revoke individual tokens from the Profile site without changing their password. |
| An interactive UI that logs users in. | **Authorization Code Flow with PKCE**. | Standard interactive OAuth. Not the focus of this topic, but the right answer if your "Basic" usage today is actually just "I prompted the user for credentials and forwarded them". |

Client Credentials access tokens, PATs, and SATs are all sent as standard `Authorization: Bearer <token>` and accepted across @@name endpoints and external integrations (for example custom services, reporting tools, or BI connectors). Once the token is in hand, the consumer does not need to know which kind it is.

For background, see:

- [Service and Background Apps (Client Credentials Flow)](../flows/client-credentials/overview.md)
- [Reference Access Tokens (PAT, SAT)](../tokens/reference-access-tokens.md)
- [Choosing the Right Flow](../flows/choosing-flow.md)

## Prerequisite: register a Trusted Application

Every non-interactive integration must be registered as a **Trusted Application** in the target instance. The registration controls which flows are allowed and what identity the resulting token carries.

Set the following on the Trusted Application before migrating. The required values depend on which target you are moving to.

### For Client Credentials

Client Credentials issues short-lived OAuth access tokens through `/id/connect/token`. Reference token (PAT/SAT) issuance is not used, so `AccessTokens` can be set to `None`.

| Field | Value |
|---|---|
| `ApplicationUri` (used as OAuth `client_id`) | A stable URI for the app, for example `my.trusted.app/integration` |
| `ClientType` | `Confidential` |
| `ApplicationSecretHash` | Hash of a strong secret. Rotate periodically. |
| `Scope` | The scopes the integration needs (for example `read`, `update`) |
| `SystemUserAllowed` | `true` |
| `SystemUser` | The least-privilege ERP user the integration acts as (for example `svc.integration`) |
| `AccessTokens` | `None` |
| `IsEnabled` | `true` |

### For SAT

SAT shares the Client Credentials configuration, with one difference: reference token issuance must be enabled. Administrators issue SATs from the Instance Manager site.

| Field | Value |
|---|---|
| `ApplicationUri` (used as OAuth `client_id`) | A stable URI for the app, for example `my.trusted.app/integration` |
| `ClientType` | `Confidential` |
| `ApplicationSecretHash` | Hash of a strong secret. Rotate periodically. |
| `Scope` | Scopes the issued SATs may carry, for example `read`, `update` |
| `SystemUserAllowed` | `true` |
| `SystemUser` | The least-privilege ERP user the SAT acts as |
| `AccessTokens` | `AdministratorsOnly` |
| `IsEnabled` | `true` |

### For PAT

PATs are minted by the end user from the Profile site against an existing Trusted Application. The user's own interactive sign-in authorises the issuance call, so the app does not need a client secret and `ClientType` is not validated at issuance time.

| Field | Value |
|---|---|
| `ApplicationUri` (used as OAuth `client_id`) | A stable URI for the app, for example `my.trusted.app/integration` |
| `Scope` | Scopes a PAT may carry, for example `read`, `update` |
| `AccessTokens` | `AuthenticatedUsers` (any signed-in user can mint a PAT) or `AdministratorsOnly` (admins mint PATs on behalf of users) |
| `IsEnabled` | `true` |

See [Trusted Applications and Access Control](./trusted-apps-access.md) for the full reference.

## Migrating from Basic to Client Credentials

This is the most common path: a backend that today authenticates with `Authorization: Basic ...`.

### Before

```http
GET /api/domain/odata/Crm_Sales_Customers?$top=10 HTTP/1.1
Host: testdb.my.erp.net
Authorization: Basic base64(svc.integration:hunter2)
```

### After

1. Register a Trusted Application as in the prerequisite section, with `SystemUserAllowed = true` and `SystemUser = svc.integration` (or whichever user the integration should act as).

2. Exchange the client secret for an access token at the standard token endpoint:

   ```http
   POST /id/connect/token HTTP/1.1
   Host: testdb.my.erp.net
   Content-Type: application/x-www-form-urlencoded

   grant_type=client_credentials&
   client_id=my.trusted.app/integration&
   client_secret=<your_plain_client_secret>&
   scope=read
   ```

   Response:

   ```json
   {
     "access_token": "<access_token>",
     "expires_in": 3600,
     "token_type": "Bearer",
     "scope": "read"
   }
   ```

3. Send the access token on subsequent calls:

   ```http
   GET /api/domain/odata/Crm_Sales_Customers?$top=10 HTTP/1.1
   Host: testdb.my.erp.net
   Authorization: Bearer <access_token>
   ```

4. Cache the token in process until it is close to expiry, then request a fresh one. Do **not** call `/id/connect/token` per request.

5. Decommission the Basic credentials:
   - Remove the user password from the integration's configuration.
   - Rotate the password on the original ERP user so any lingering Basic clients fail loudly.
   - Disable interactive login for the dedicated System User configured on the Trusted Application.

For a fuller walkthrough, see [Step-by-Step: Client Credentials Flow](../flows/client-credentials/service-apps-step-by-step.md).

## Migrating from Basic to PAT

Use this when the integration must act as a specific person and you cannot move to Client Credentials.

### Before

```http
GET /api/domain/odata/Crm_Sales_Customers HTTP/1.1
Host: testdb.my.erp.net
Authorization: Basic base64(alice:hunter2)
```

### After

1. Make sure the target Trusted Application has `AccessTokens = AuthenticatedUsers` (or `AdministratorsOnly` if an admin will mint the token for the user).

2. The user mints a PAT from the Profile site. The token returned starts with `enrt_`, for example:

   ```text
   enrt_1D41D4694B4F02D3D6A31FFA07E20B73F48248B26C75A0CCCB5F9DBEE41F7960
   ```

   See [Issuing Reference Tokens](../tokens/issuing-reference-tokens.md) for the issuance UI and request shape.

3. Use the PAT in subsequent calls:

   ```http
   GET /api/domain/odata/Crm_Sales_Customers HTTP/1.1
   Host: testdb.my.erp.net
   Authorization: Bearer enrt_1D41D4694B4F02D3D6A31FFA07E20B73F48248B26C75A0CCCB5F9DBEE41F7960
   ```

4. Decommission the Basic credentials as in the previous section. The user's password is no longer used by the integration.

> [!NOTE]
> A PAT inherits the issuing user's permissions. Restrict scopes and expiration to the minimum needed.

## Migrating from ErpLogin to Client Credentials

The shape change is straightforward: instead of trading a username and password for an opaque `ErpSession` id, you trade a `client_id` and `client_secret` for a standard OAuth access token.

### Before

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

```http
GET /api/domain/odata/Crm_Sales_Customers HTTP/1.1
Host: testdb.my.erp.net
Authorization: ErpSession 4f9c0c7c-...-9c1f
```

### After

```http
POST /id/connect/token HTTP/1.1
Host: testdb.my.erp.net
Content-Type: application/x-www-form-urlencoded

grant_type=client_credentials&
client_id=my.trusted.app/integration&
client_secret=<your_plain_client_secret>&
scope=read
```

```http
GET /api/domain/odata/Crm_Sales_Customers HTTP/1.1
Host: testdb.my.erp.net
Authorization: Bearer <access_token>
```

Field mapping:

| ErpLogin field | OAuth equivalent | Notes |
|---|---|---|
| `user` + `pass` | `client_id` + `client_secret` of a Trusted Application | The user password is gone. The integration authenticates as an app, and runs as the Trusted Application's System User. |
| `app` | `client_id` | Use the Trusted Application's `ApplicationUri` as `client_id`. |
| `lang` | No OAuth equivalent | Pass language out of band, for example via the `?lang=` query string where the consumer supports it. |

Other differences to plan for:

- **Token expiry.** OAuth access tokens are short-lived (see `expires_in`). Cache them and re-request when needed. ErpLogin returned a session id with no built-in expiry.
- **No refresh tokens.** Client Credentials does not issue refresh tokens. Just request a fresh token. See [Token Lifetime and Renewal](../tokens/token-lifetime.md).
- **Logout.** ErpLogin's `POST /domain/logout` has no direct analogue here. Either let the access token expire, or revoke through the identity site if immediate invalidation is required. See [Session Revocation](../sessions/session-revocation.md).

## Migrating from ErpLogin to PAT

When you specifically need a human identity in the token (per-user audit, per-user permissions), follow [Migrating from Basic to PAT](#migrating-from-basic-to-pat) instead. The mint endpoint is different (Profile site, `enrt_` token returned), and the lifetime is much longer than a Client Credentials access token.

## Coexistence period

Until the legacy schemes are removed:

- Basic, ErpLogin, and Bearer (Client Credentials access tokens, PATs, SATs) all keep working in parallel. The server prefers OAuth schemes and falls back to the legacy ones.
- Send only **one** authorization scheme per request. Mixing `Basic` and `Bearer` in the same call is undefined.
- Basic and ErpLogin maintain their own per-host caches. Token-based clients are validated through @@name Identity and do not depend on that cache, which is one of the reasons OAuth scales better across server farms.

## Migration checklist

- [ ] Trusted Application registered with `ApplicationUri`, secret, and `Scope` set correctly.
- [ ] For Client Credentials and SAT: `SystemUserAllowed = true` and a least-privilege `SystemUser` configured.
- [ ] Integration code calls `/id/connect/token` (Client Credentials) or uses an issued `enrt_` token (PAT or SAT).
- [ ] Access tokens cached in process and reused until close to expiry.
- [ ] All authenticated requests send `Authorization: Bearer <token>`.
- [ ] Legacy credentials removed from configuration; service account password rotated.
- [ ] Token expiry path tested. Renewal verified end to end.
- [ ] Revocation path tested. PAT from the Profile site; Client Credentials by rotating the secret.

---

## Learn More

- [**Choosing the Right Flow**](../flows/choosing-flow.md)  
  Decision guide for picking between Authorization Code, Client Credentials, and reference tokens.

- [**Service and Background Apps (Client Credentials Flow)**](../flows/client-credentials/overview.md)  
  Full walkthrough for the most common migration target.

- [**Reference Access Tokens (PAT, SAT)**](../tokens/reference-access-tokens.md)  
  When to use long-lived reference tokens and how they are issued.

- [**Trusted Applications and Access Control**](./trusted-apps-access.md)  
  System User, scopes, and access policies.

- [**Security Best Practices**](../security-best-practices.md)  
  Secret handling, rotation, and audit guidance for production integrations.
