---
erp.type: sample
erp.topic: security
---

# Get an access token for an Agent

## Objective

CatalogScout is a product catalog assistant. Its backend uses the standard OAuth 2.0 Token Exchange grant to impersonate the @@name Agent `CatalogScout`. The subject JWT carries the Agent's login in its `sub` claim. The backend uses the returned access token to read products through the Domain API under the Agent's permissions.

## Prerequisites

Configure a Trusted Application in the target instance using the following settings.

| Attribute | Value | Comment |
|-----------|-------|---------|
| `ApplicationUri` | `com.example/product-reader` | This is the `client_id` in the request. |
| `IsEnabled` | `true` | |
| `ClientType` | `Confidential` | Keep the client secret on the server. |
| `ApplicationSecretHash` | `base64(sha256(your-client-secret))` | Use the original secret as `client_secret` in the request. |
| `Scope` | `read update` | The API scopes the application can request. |

Use an active user of type **Agent** with login `CatalogScout`. Set the Agent's **Scope** to `read` and grant the user permission to read products. The authenticated application is responsible for choosing which Agent login it asserts.

## Steps

### Create the subject token

Use a compact unsigned JWT. The header sets `alg` to `none`.

```json
{"alg":"none","typ":"JWT"}
```

The payload sets `sub` to the Agent's login.

```json
{"sub":"CatalogScout"}
```

Base64url-encode the header and payload without padding and join them with periods. The signature is empty, so the resulting JWT ends with a period.

```text
eyJhbGciOiJub25lIiwidHlwIjoiSldUIn0.eyJzdWIiOiJDYXRhbG9nU2NvdXQifQ.
```

See the [Agent impersonation JWT reference](../flows/token-exchange-use-cases.md#subject-jwt) for the supported claims.

### Token endpoint

Send the POST to the ID site of the instance where you configured the Agent and Trusted Application. The example uses `https://demodb.my.erp.net/id/connect/token`. Substitute your client ID and secret in the form body. If you use a different Agent login, encode it in the JWT and replace `subject_token`.

```http
POST /id/connect/token HTTP/1.1
Host: demodb.my.erp.net
Content-Type: application/x-www-form-urlencoded
Content-Length: 309

client_id=com.example%2Fproduct-reader&client_secret=example-client-secret&grant_type=urn%3Aietf%3Aparams%3Aoauth%3Agrant-type%3Atoken-exchange&subject_token_type=urn%3Aietf%3Aparams%3Aoauth%3Atoken-type%3Ajwt&subject_token=eyJhbGciOiJub25lIiwidHlwIjoiSldUIn0.eyJzdWIiOiJDYXRhbG9nU2NvdXQifQ.&scope=read+update
```

Form-encode any values you substitute and let your HTTP client calculate `Content-Length`.

A successful request returns HTTP `200` with a response like this.

```json
{
  "access_token": "<access_token>",
  "issued_token_type": "urn:ietf:params:oauth:token-type:access_token",
  "token_type": "Bearer",
  "expires_in": 3600,
  "scope": "read"
}
```

The request asks for `read update`, which the Trusted Application allows. The Agent's scope narrows the grant to `read`.

### Authorized Domain API call

Replace `<access_token>` in the `Authorization` header with the `access_token` from the response.

```http
GET /api/domain/odata/General_Products_Products?$top=1 HTTP/1.1
Host: demodb.my.erp.net
Authorization: Bearer <access_token>
```

Use the same instance host as in the token request. Repeat the exchange when the access token expires.

## Resources

[OAuth 2.0 Token Exchange](../flows/token-exchange.md)
