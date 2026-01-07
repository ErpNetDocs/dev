# OAuth2 and OpenID Connect Client Libraries

@@name uses standard **OAuth 2.0** and **OpenID Connect (OIDC)** protocols for authentication and authorization.  

You can connect using any compliant client library - no proprietary SDK is required.

## Supported Standards

@@name Identity implements:

- **OAuth 2.0 Authorization Framework** ([RFC 6749](https://datatracker.ietf.org/doc/html/rfc6749))
- **OpenID Connect Core 1.0** ([openid.net/specs/openid-connect-core-1_0.html](https://openid.net/specs/openid-connect-core-1_0.html))
- **PKCE (Proof Key for Code Exchange)** ([RFC 7636](https://datatracker.ietf.org/doc/html/rfc7636))
- **OpenID Discovery Document**  

Available at:  

```http
https://<your-instance>.my.erp.net/id/.well-known/openid-configuration
```

Any library or framework that supports these standards will work with @@name.

## Choosing a Library

You can use either a **generic OAuth2/OIDC library** or a **framework-integrated** solution.  

Choose one that matches your environment and flow type.

| Platform | Recommended Libraries | Notes |
|-----------|----------------------|-------|
| **.NET / C#** | [IdentityModel](https://docs.duendesoftware.com/identitymodel/), [Microsoft.Identity.Client (MSAL)](https://github.com/AzureAD/microsoft-authentication-library-for-dotnet) | Full OAuth2 + OIDC support |
| **JavaScript / SPA** | [oidc-client-ts](https://github.com/authts/oidc-client-ts), [Auth.js / NextAuth](https://authjs.dev/) | Built-in PKCE and redirect handling |
| **Python** | [Authlib](https://docs.authlib.org/), [requests-oauthlib](https://requests-oauthlib.readthedocs.io/) | Simple flow helpers |
| **Java / Spring** | [Spring Security OAuth2 Client](https://docs.spring.io/spring-security/reference/servlet/oauth2/index.html) | Integrated token management |
| **PHP** | [League OAuth2 Client](https://oauth2-client.thephpleague.com/) | Clean API for Auth Code and Client Credentials |
| **Node.js (backend)** | [simple-oauth2](https://github.com/lelylan/simple-oauth2), [openid-client](https://github.com/panva/node-openid-client) | Handles tokens and discovery automatically |
| **Go** | [golang.org/x/oauth2](https://pkg.go.dev/golang.org/x/oauth2) | Minimal and reliable |
| **Swift / iOS** | [AppAuth-iOS](https://github.com/openid/AppAuth-iOS) | Native OIDC flow with PKCE |
| **Android / Kotlin** | [AppAuth-Android](https://github.com/openid/AppAuth-Android) | Official OIDC SDK for Android |

## Example: Using IdentityModel (C#)

```csharp
using IdentityModel.Client;
using System.Net.Http;

// Discover endpoints from metadata
var client = new HttpClient();
var disco = await client.GetDiscoveryDocumentAsync("https://your-instance.my.erp.net/id");
if (disco.IsError)
  throw new Exception(disco.Error);

// Request token using Client Credentials.
var tokenResponse = await client.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
{
  Address = disco.TokenEndpoint,
  ClientId = "myapp",
  ClientSecret = "supersecret",
  Scope = "read update"
});

Console.WriteLine(tokenResponse.AccessToken);
```

## Example: Using oidc-client-ts (JavaScript SPA)

```js
import { UserManager } from "oidc-client-ts";

const settings = {
  authority: "https://your-instance.my.erp.net/id",
  client_id: "myapp",
  redirect_uri: "https://app.example.com/auth/callback",
  response_type: "code",
  scope: "openid profile read update",
};

const userManager = new UserManager(settings);
await userManager.signinRedirect(); // Redirects to ERP.net login page
```

## Example: Using requests-oauthlib (Python)

```python
from requests_oauthlib import OAuth2Session

client_id = "myapp"
client_secret = "supersecret"
token_url = "https://your-instance.my.erp.net/id/connect/token"

oauth = OAuth2Session(client_id)
token = oauth.fetch_token(
    token_url=token_url,
    client_id=client_id,
    client_secret=client_secret,
    scope=["read", "update"]
)

print(token["access_token"])
```

## Notes for @@name Developers

- All token requests and authorizations must go through your instance's Identity (`/id` path).
- Always request scopes explicitly (`read`, `update`, etc.).
- Use **PKCE** for public apps that cannot store secrets.
- Never hardcode client secrets in browser code or mobile apps.
- Refresh tokens are supported for **interactive** apps.
- For service integrations, prefer Client Credentials flow - simpler and more stable.

## Testing and Debugging

You can safely test authentication flows in:

```http
https://<your-instance>.my.erp.net/id/.well-known/openid-configuration
```

Use tools like **Postman**, **curl**, or **jwt.io** to:

- Inspect tokens
- Validate scopes and claims
- Check expiration and issuer
- View key rotation metadata (JWKS)

---

## Learn More

- [**OAuth 2.0 Overview**](../auth/concepts/oauth2-overview.md)  
  Understand the core principles behind @@name authentication.

- [**ERP.net Identity**](../auth/concepts/identity-server.md)  
  Learn how @@name issues and validates tokens.

- [**Auth Flows Overview**](./flows/overview.md)  
  Explore the different OAuth flows for interactive, service, and hybrid apps.

- [**Access Tokens**](./tokens/access-tokens.md)  
  See how tokens are structured and used to access APIs.

- [**Reference Access Tokens (PAT, SAT)**](./tokens/reference-access-tokens.md)  
  Learn how to use long-lived tokens for integrations and automation.
