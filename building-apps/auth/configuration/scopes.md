# Scopes

Scopes define **what level of access** an application or token has within @@name.  

They are included in access tokens and determine which APIs, operations, and data the holder can use.

Scopes are requested by the app and granted by the @@name Identity based on the app's configuration and policies.

## Standard Scopes

| **Scope** | **Description** | **Availability** |
|------------|------------------|------------------|
| `read` | Grants **read-only** access to @@name data through APIs. | All trusted apps |
| `update` | Grants permission to **create, modify, or delete** @@name data. | All trusted apps |
| `offline_access` | Allows issuing a **refresh token**, enabling apps to renew access tokens without user interaction. | All trusted apps |
| `profile` | Grants access to **basic user information** (such as name, email, user type). | All trusted apps |
| `sec` | Grants access to **security namespace APIs** (roles, permissions, etc.). Reserved for @@name internal use only. | Internal only |

## Requesting Scopes

Applications request scopes during the **authorization** or **token** request using the `scope` parameter:

```http
POST /id/connect/token
Content-Type: application/x-www-form-urlencoded

client_id=my.trusted.app
client_secret=<secret>
grant_type=client_credentials
scope=read update
```

The resulting access token will contain the allowed scopes as part of its payload.

## Scope Enforcement

- The @@name Identity verifies which scopes a **Trusted Application** is permitted to request.  
- If a requested scope is not allowed, the server returns `invalid_scope`.  
- APIs automatically validate scopes on every request.
- If no scopes are explicitly requested, the issued access token will automatically include **all scopes defined in the Trusted Application** configuration.

> [!NOTE]  
> Always request the **minimal set of scopes** required by your application.  
> This limits exposure and improves overall security.

## Example Token Payload (Scopes)

```json
{
  "client_id": "my.trusted.app",
  "scope": ["read", "update", "offline_access"],
  "aud": ["DomainAPI"],
  "iss": "https://demo.my.erp.net/id",
  "exp": 1762189360
}
```

## Security Considerations

- Only grant `update` to apps that must modify data.  
- Avoid combining user and system scopes in the same token.  
- Regularly audit trusted apps and their allowed scopes.  

---

## Learn More

- [**Access Tokens**](../tokens/access-tokens.md)  
  Learn how scopes are embedded in tokens and used for authorization.

- [**Reference Access Tokens (PAT, SAT)**](../tokens/reference-access-tokens.md)  
  Understand how long-lived tokens also rely on scope definitions.

- [**Token Lifetime and Renewal**](../tokens/token-lifetime.md)  
  See how token expiration interacts with scoped access.

- [**Trusted Applications and Access Control**](../configuration/trusted-apps-access.md)  
  Learn how trusted app configurations define which scopes are permitted.
