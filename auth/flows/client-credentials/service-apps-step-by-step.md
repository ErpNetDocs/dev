# Step-by-Step: Client Credentials Flow

This walkthrough shows how a service app authenticates with the @@name Identity and calls the APIs using the **Client Credentials** grant.

## Prerequisites

Register a **Trusted Application** with:

| Attribute | Value | Notes |
|---|---|---|
| ApplicationUri | my.trusted.app/service | Used as `client_id` |
| ClientType | Confidential | Service app must keep a secret |
| ApplicationSecretHash | base64(sha256(your-secret)) | Store only the hash |
| SystemUserAllowed | true | Enables service access |
| SystemUser | svc.integration | Least-privilege account |
| Scope | read DomainApi | Request only what you need |
| IsEnabled | true | App is active |

## 1) Request an access token

HTTP form:

```http
POST /id/connect/token HTTP/1.1
Host: testdb.my.erp.net
Content-Type: application/x-www-form-urlencoded

grant_type=client_credentials&
client_id=my.trusted.app/service&
client_secret=<your_plain_client_secret>&
scope=read
```

### Successful response

```json
{
  "access_token": "<access_token>",
  "expires_in": 3600,
  "token_type": "Bearer",
  "scope": "read"
}
```

## 2) Call @@name APIs

```http
GET /api/domain/odata/Crm_Sales_Customers?$top=10 HTTP/1.1
Host: testdb.my.erp.net
Authorization: Bearer <access_token>
```

- The call executes under the **System User** configured in the Trusted Application.
- A session is created when the token is first used, not when issued.

## 3) Handle expiry

When API returns `401 Unauthorized`, the access token likely expired. Request a fresh token with the same client credentials and retry the API call.

> [!NOTE]
> Refresh tokens are not issued for Client Credentials. Always request a new access token.

## Security best practices

- Store `client_secret` only on secure servers. Never ship secrets to browsers or mobile apps.
- Use a dedicated **System User** with least privilege. Rotate its password and the client secret periodically.
- Do not log full tokens. Log hashes or last 6 to 8 characters for traceability.
- Send requests only to the correct instance: `https://{instance}/id`.

## Full samples

[!list limit=1000 erp.type=sample erp.topic=security default-text="None"]

## Learn More

- [**Overview**](overview.md)  
  When and why to use Client Credentials.

- [**Token Request and Response**](service-apps-token.md)  
  Parameters, examples, and response fields.

- [**Common Errors**](service-apps-errors.md)  
  Troubleshooting configuration and scope issues.

- [**Trusted Applications and Access Control**](../../configuration/trusted-apps-access.md)  
  System User, scopes, and policies.
