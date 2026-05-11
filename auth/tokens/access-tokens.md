# Access Tokens

An **access token** is the credential that allows your application to call @@name APIs on behalf of a user or a service.  
It defines **who** is calling, **what** can be accessed, and **for how long**.

All @@name access tokens are issued by **@@name Identity** using the **OAuth 2.0** standard and are formatted as **JWTs (JSON Web Tokens)**.

## Access Token Basics

When an application successfully authenticates (using Authorization Code or Client Credentials flow), @@name Identity returns an access token.  
This token is then passed in the `Authorization` HTTP header with each API request:

```http
GET /api/domain/odata/Crm_Sales_Customers?$top=5 HTTP/1.1
Host: testdb.my.erp.net
Authorization: Bearer eyJhbGciOiJSUzI1NiIsImtpZCI6IjVCMjc5MjBFNjUzREQ3QUM2N0QyRjY0QjMyQTE3OTkyIiwidHlwIjoiYXQrand0In0...
```

If the token is valid, the @@name API processes the request according to the scopes and permissions defined in the token.

> [!NOTE]
> Access tokens are short-lived and must be renewed periodically.
> Their lifetime is typically around one hour, but can vary depending on configuration.

## JWT Structure

@@name access tokens are JWTs - digitally signed, URL-safe strings that contain encoded claims.

A JWT consists of three parts, separated by dots:

```css
header.payload.signature
```

### Example Token

```text
eyJhbGciOiJSUzI1NiIsImtpZCI6IjVCMjc5MjBFNjUzREQ3QUM2N0QyRjY0QjMyQTE3OTkyIiwidHlwIjoiYXQrand0In0.eyJuYmYiOjE3NjIxODU3NjAsImV4cCI6MTc2MjE4OTM2MCwiaXNzIjoiaHR0cHM6Ly9wa29zdG92Lm15LmVycC5uZXQvaWQiLCJhdWQiOlsiRG9tYWluQVBJIiwiVGFibGVBUEkiLCJPTEFQIiwiQXBwU2VydmVyIiwiaHR0cHM6Ly9wa29zdG92Lm15LmVycC5uZXQvaWQvcmVzb3VyY2VzIl0sImNsaWVudF9pZCI6IlBLIiwiY2xpZW50X3N5c3RlbV91c2VyIjoiYWRtaW4iLCJjbGllbnRfc3lzdGVtX3VzZXJfdHlwZSI6IkludGVybmFsVXNlciIsImNsaWVudF9kYiI6IkUxX0RFVi0xIFRlc3QiLCJqdGkiOiIxQjc5QzI0QUIyNUUwRjY3NURGMjIzM0NERTM3MTI0NCIsImlhdCI6MTc2MjE4NTc2MCwic2NvcGUiOlsiRG9tYWluQXBpIiwicmVhZCIsInNlYyIsInVwZGF0ZSJdfQ.QjdrJ_jvbUXIGBgwajwg0dEZO8Y7VjxNW7lUi_9fdh3hliVTh-0WgOEIsClWSROsTvlzUR4Poz0kG0lKKFex9wmQ54X0M5epdeH-p3EJR018SVROp9AJNB7RiKf-dGzOFRXwjB4ueX84j8L-uIubrQL3iwmI1kI8KRGFscaVxEx7sXZFn1FVMIsaZhY8mDEEOhyicvmN9zQQOdNlSjTtP2qL6tpkS-k1kJKAE9oeKdidXbQ1yJbBHX2qUhJM8plzd-RC297m-UFqfzFgZ_Lf04cZN0xdiV8OXiuyrIC9zvfmUSex9n0ROSoIZ4BzwcZ9VM2p1FOx7RFNLdlT0Q8arQ
```

### 1. Header

Specifies the algorithm and type of the token.

```json
{
  "alg": "RS256",
  "kid": "5B27920E653DD7AC67D2F64B32A17992",
  "typ": "at+jwt"
}
```

- **alg** - Signing algorithm (always RS256 for @@name).
- **kid** - Key ID of the signing certificate.
- **typ** - Indicates the token type (access token in JWT format).

### 2. Payload

Contains the actual claims - information about the client, permissions, and validity.

```json
{
  "nbf": 1762185760,
  "exp": 1762189360,
  "iss": "https://acme.my.erp.net/id",
  "aud": ["DomainAPI", "TableAPI", "OLAP", "AppServer"],
  "client_id": "myapp.myhost.net",
  "client_system_user": "admin",
  "client_system_user_type": "InternalUser",
  "client_db": "E1_TESTDB",
  "scope": ["read", "sec", "update"]
}
```

The table below lists every claim @@name Identity may issue. Not all claims appear in every token: the **Auth Code** and **Client Credentials** columns indicate when each claim is included.

| Claim | Description | Example | Auth Code | Client Credentials |
|---|---|---|---|---|
| `iss` | Identifies the issuer. Clients reject tokens whose `iss` does not match the configured authority. | `"https://acme.my.erp.net/id"` | yes | yes |
| `aud` | Which @@name APIs will accept the token. Each API verifies it appears in this list. | `["DomainAPI", "TableAPI","OLAP","AppServer"]` | yes | yes |
| `iat` | Unix timestamp of when the token was issued. | `1762185760` | yes | yes |
| `nbf` | "Not before". The token is invalid before this instant. | `1762185760` | yes | yes |
| `exp` | Expiration. The token is invalid after this instant (typically `iat` + ~1 hour). | `1762189360` | yes | yes |
| `jti` | Unique token id. Enables per-token revocation and lets clients deduplicate. | `"1B79C24AB25E0F675DF2233CDE371244"` | yes | yes |
| `sub` | The acting user's login (`Sec_Users.Login`). Absent in service-to-service tokens. | `"john.doe"` | yes | no |
| `sub_id` | Internal `Sec_Users.Id`. Stable across renames, unlike `sub`. | `"4587"` | yes | no |
| `auth_time` | When the user actually authenticated (ID token only). Lets relying parties enforce `max_age` and re-authentication policies. | `1762185700` | yes | n/a |
| `sid` | Session id. Used by the application server as the license-slot key and for single sign-out. | `"E4D2A57B3F1C..."` | yes | no |
| `scope` | The granted scopes. What the token is authorized to do. APIs gate operations against these. | `["read","sec","update"]` | yes | yes |
| `client_id` | The trusted application that obtained the token. Used for audit, per-client throttling, and audience checks. | `"myapp.myhost.net"` | yes | yes |
| `name` | Display name shown in UIs. | `"John Doe"` | yes | no |
| `email` | The user's email. Used by downstream apps for notifications and lookups. | `"john.doe@example.com"` | yes | no |
| `email_verified` | Whether the email was confirmed. Apps gate flows such as password reset on this. | `true` | yes | no |
| `locale` | The user's preferred culture. Servers use it to localize messages, dates, and numbers. | `"en-US"` | yes | no |
| `user_type` | User classification (`InternalUser`, `ExternalCommunityUser`, `SystemUserNoLogin`). Trusted apps validate that the type is allowed before issuing tokens. | `"InternalUser"` | yes | no |
| `is_admin` | Admin flag. Short-circuits authorization for administrative operations. | `true` | yes | no |
| `db` / `client_db` | Which @@name database (tenant) the token is bound to. APIs use this to route the request. The user-flow name is `db`; the service-flow name is `client_db`. | `"E1_TESTDB"` | yes (`db`) | yes (`client_db`) |
| `system_user` / `client_system_user` | Service-account login used for non-impersonated actions (background jobs, system writes). | `"admin"` | no | yes (`client_system_user`) |
| `system_user_type` / `client_system_user_type` | User type of the service account. Same allow-list checks as `user_type`. | `"InternalUser"` | no | yes (`client_system_user_type`) |
| `system_user_id` / `client_system_user_id` | Internal id of the service account, when needed for foreign-key references. | `"1"` | no | yes (when set) |
| `idp` | Which identity provider authenticated this session. Apps can branch behavior for federated users. | `"google"` or a provider GUID | yes (external IdP only) | no |
| `tid` | External tenant id (typically Azure AD `tid`). Used for tenant-scoped authorization on the @@name side. | `"72f988bf-86f1-41af-91ab-2d7cd011db47"` | yes (external IdP only) | no |

### 3. Signature

The final part of the token is the digital signature.  

It verifies that the token was issued by @@name and has not been tampered with.

The signature is created using the @@name Identity private key, and validated using the public key, available at:

```http
https://<instance>/id/.well-known/openid-configuration/jwks
```

## Validating Access Tokens

You can decode and inspect tokens using standard JWT libraries or tools such as [jwt.io](https://jwt.io).  
To validate a token:

1. Verify the **signature** matches @@name Identity public key.  
2. Check the **expiration time (`exp`)** and **issuer (`iss`)**.  
3. Ensure the **audience (`aud`)** includes the API you are calling.  
4. Confirm the **scopes** grant the needed access.

> [!IMPORTANT]  
> @@name APIs automatically perform all these checks on every request.  
> Manual validation is only needed when implementing middleware or custom API gateways.

---

## Security Notes

- Always transmit tokens using **HTTPS**.  
- Never log or store access tokens in plain text.  
- Treat them as **confidential credentials**.  
- Access tokens are **short-lived** - design your apps to handle renewal automatically.  
- Use **refresh tokens** only in trusted, confidential applications.

---

## Learn More

- [**Reference Access Tokens (PAT, SAT)**](reference-access-tokens.md)  
  Learn how long-lived reference tokens provide persistent automation access.

- [**Scopes**](../concepts/scopes.md)  
  Understand how scopes define the APIs and actions available to your app.

- [**Token Lifetime and Renewal**](token-lifetime.md)  
  See how token expiration works and how refresh tokens extend access.
