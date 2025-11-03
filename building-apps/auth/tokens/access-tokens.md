# Access Tokens

An **access token** is the credential that allows your application to call @@name APIs on behalf of a user or a service.  
It defines **who** is calling, **what** can be accessed, and **for how long**.

All @@name access tokens are issued by the **Identity Server** using the **OAuth 2.0** standard and are formatted as **JWTs (JSON Web Tokens)**.

## Access Token Basics

When an application successfully authenticates (using Authorization Code or Client Credentials flow), the Identity Server returns an access token.  
This token is then passed in the `Authorization` HTTP header with each API request:

```http
GET /api/domain/odata/Crm_Customers?$top=5 HTTP/1.1
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
  "iss": "https://pkostov.my.erp.net/id",
  "aud": ["DomainAPI", "TableAPI", "OLAP", "AppServer"],
  "client_id": "PK",
  "client_system_user": "admin",
  "client_system_user_type": "InternalUser",
  "client_db": "E1_DEV-1 Test",
  "scope": ["DomainApi", "read", "sec", "update"]
}
```

| **Claim** | **Description** |
|------------|-----------------|
| **nbf** | "Not before" - token is invalid before this time (in Unix timestamp). |
| **exp** | Expiration time - when the token becomes invalid. |
| **iss** | Issuer - @@name Identity Server that issued the token. |
| **aud** | Audience - the @@name APIs or resources this token is valid for. |
| **client_id** | The application that obtained the token (Trusted Application). |
| **client_system_user** | The system user identity associated with the application. |
| **client_system_user_type** | Indicates whether it's an internal or service user. |
| **client_db** | The database (tenant) to which the token applies. |
| **scope** | The granted permissions and API access levels. |

### 3. Signature

The final part of the token is the digital signature.  

It verifies that the token was issued by @@name and has not been tampered with.

The signature is created using the @@name Identity Server's private key, and validated using the public key, available at:

```http
https://<instance>/id/.well-known/openid-configuration/jwks
```

## Validating Access Tokens

You can decode and inspect tokens using standard JWT libraries or tools such as [jwt.io](https://jwt.io).  
To validate a token:

1. Verify the **signature** matches the Identity Server’s public key.  
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

- [**Scopes and Permissions**](scopes.md)  
  Understand how scopes define the APIs and actions available to your app.

- [**Token Lifetime and Renewal**](token-lifetime.md)  
  See how token expiration works and how refresh tokens extend access.

- [**Trusted Applications and Access Control**](../../how-apps-connect/trusted-apps-access.md)  
  Learn how app registrations define system users and permissions.
