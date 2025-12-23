# Token Request and Response

This page documents the exact request and response for the **Client Credentials** grant in @@name.  

Service apps use this to obtain short-lived access tokens that represent the app's configured **System User**.

## Endpoint

- Authorization server base: `https://{instance}/id`
- Token endpoint: `POST /connect/token`
- Content type: `application/x-www-form-urlencoded`

## Request parameters

| Name | Required | Example | Notes |
|---|---|---|---|
| `grant_type` | Yes | `client_credentials` | Must be exactly `client_credentials`. |
| `client_id` | Yes | `my.trusted.app/service` | Your Trusted Application `ApplicationUri`. |
| `client_secret` | Yes | `<your_plain_client_secret>` | Confidential clients only. Keep secret server-side. |
| `scope` | Recommended | `DomainApi read` | Space-delimited scopes your service needs. Request only what you need. |

## HTTP example

```http
POST /id/connect/token HTTP/1.1
Host: testdb.my.erp.net
Content-Type: application/x-www-form-urlencoded

grant_type=client_credentials&
client_id=my.trusted.app/service&
client_secret=<your_plain_client_secret>&
scope=read
```

## cURL example

```bash
curl -X POST "https://testdb.my.erp.net/id/connect/token" \
  -H "Content-Type: application/x-www-form-urlencoded" \
  -d "grant_type=client_credentials" \
  -d "client_id=my.trusted.app/service" \
  -d "client_secret=<your_plain_client_secret>" \
  -d "scope=read"
```

## Successful response

```json
{
  "access_token": "<access_token>",
  "expires_in": 3600,
  "token_type": "Bearer",
  "scope": "read"
}
```

### Successful response fields

- `access_token`: The short-lived bearer token your service must send in the `Authorization: Bearer <token>` header when calling @@name APIs. It executes under the configured System User and within the granted scopes.
- `expires_in`: Lifetime of the access token in seconds. After this time elapses, request a new token.
- `token_type`: Always Bearer for @@name APIs.
- `scope`: Space-delimited list of scopes actually granted (for example, read, update). May be a subset of what you requested.

> [!NOTE]
> Client Credentials flow does not return a refresh token. When the token expires, request a new one.

## Using the token

```http
GET /api/domain/odata/Crm_Customers?$top=10 HTTP/1.1
Host: testdb.my.erp.net
Authorization: Bearer <access_token>
```

- The call runs under the System User configured in your Trusted Application.
- A session is created when the token is first used, not when it is issued.

## Scopes

Typical scopes for service apps:

- read for read-only access.
- update for write operations.

> [!NOTE]
> Follow least-privilege. Start with read; add update only when strictly required.

## Learn More

- [**Overview**](overview.md)  
  When and why to use Client Credentials.

- [**Step-by-Step Example**](service-apps-step-by-step.md)  
  Minimal sequence to obtain a token and call the APIs.

- [**Common Errors**](service-apps-errors.md)  
  Fix common issues like invalid_client and invalid_scope.

- [**Trusted Applications and Access Control**](../../how-apps-connect/trusted-apps-access.md)  
  Configure System User and scopes.
