# Service Applications (Service Access for Confidential Applications)

This topic describes how a **confidential external application** with no user interaction integrates with an @@name Instance ID to obtain an access token for calling instance APIs using the **Client Credentials** flow.

The following scenario is covered:

- The external application is a **confidential application**
- The application has a **backend component** capable of securely storing a secret
- The application does **not** present a user interface
- The application authenticates as a **service identity**
- The application uses the **Client Credentials** authorization flow
- The application obtains an access token to call instance APIs directly

The **Instance ID** is the identity provider of a specific @@name instance. Service applications use it to authenticate and obtain access tokens **without user involvement**, scoped to the permissions configured for the trusted application.

For example, if your @@name instance is located at:

```text
https://demodb.my.erp.net
```

then the corresponding Instance ID endpoints are located at:

```text
https://demodb.my.erp.net/id
```

## Prerequisites

Your @@name instance must have a trusted application defined with the configuration below.

> [!NOTE]
> The values shown below are **examples only** and must be replaced with values that match your application.

Typical values:

- ApplicationUri: `my-service-app-id`
- SystemUser: `<an-internal-erp-user>`

| Attribute | Value | Comment |
| --------- | ----- | ------- |
| Name | My Service Application | Used only for user-friendly identification. |
| ApplicationUri | my-service-app-id | The unique identifier of the application. |
| IsEnabled | true | Enables the trusted application. |
| SystemUserAllowed | true | Allows the application to authenticate as a service. |
| SystemUser | `<an-internal-erp-user>` | The internal user used for service authentication. |
| ImpersonateAsInternalUserAllowed | true | Allows authentication using an internal user. |
| ClientType | Confidential | Indicates that the application can securely store a secret. |
| ApplicationSecretHash | `<base64(sha256(your-secret))>` | The hashed client secret used during authentication. |

All other attributes can keep their default values and are not relevant for this scenario.

## Implementation

This section demonstrates acquiring an access token using the **Client Credentials** flow.

### 1. Request an access token

The application sends a direct request to the Instance ID token endpoint.

```http
POST /id/connect/token HTTP/1.1
Host: demodb.my.erp.net
Content-Type: application/x-www-form-urlencoded

client_id=my-service-app-id&
client_secret=<PLAIN_CLIENT_SECRET>&
grant_type=client_credentials&
scope=read%20update
```

> [!NOTE]
> The `client_secret` is sent as a **plain (unhashed) value** and must exactly match the secret configured for the trusted application.

### 2. Receive the token response

If the request is successful, the Instance ID returns an access token.

```json
{
  "access_token": "eyJhbGciOiJSUzI1NiIsImtpZCI6IjVCMjc5MjBFNjUzREQ3QUM2N0QyRjY0QjMyQTE3OTkyIiwidHlwIjoiYXQrand0In0.eyJuYmYiOjE3NjY1MDUxNDcsImV4cCI6MTc2NjUwODc0NywiaXNzIjoiaHR0cHM6Ly9lMS1kZXYubG9jYWwvaWQiLCJhdWQiOlsiRG9tYWluQVBJIiwiVGFibGVBUEkiLCJPTEFQIiwiQXBwU2VydmVyIiwiaHR0cHM6Ly9lMS1kZXYubG9jYWwvaWQvcmVzb3VyY2VzIl0sImNsaWVudF9pZCI6IlBLIiwiY2xpZW50X3N5c3RlbV91c2VyIjoiYWRtaW4iLCJjbGllbnRfc3lzdGVtX3VzZXJfdHlwZSI6IkludGVybmFsVXNlciIsImNsaWVudF9kYiI6IkUxX0RFVi0xIFRlc3QiLCJqdGkiOiI4RDBCNzA1NTczQTFBOThGREJBREZGMENEN0Y3RUVCNCIsImlhdCI6MTc2NjUwNTE0Nywic2NvcGUiOlsicmVhZCIsInVwZGF0ZSJdfQ.MRvH-EtWu-PWDhjDDn73OVQwH29DZ_RRu6XheFsoRxImyjLQRIU7-S1GuyTnnqPyXEEkGkKTu_s3IEwxGORgY48jLH3l1juJDt8_JvcyJlIdhVZSZNC1Bpft_K1NJswJ6QmJ6bWgev7cqHaxM3p7AEEPjkSmnAjdBCz7ItMV93Yio5kCRBmP9DQUoxtL0webG7zV_f5uOkt8xhbUVHpdU9FQY-XLf_heLJv_81vvpf39kPxD4WTZRly8X_mNdlqi0DxiFXK3TFOnbdoLKxeljke8jV0t-agaHWcZ4B-SMQ77falwtFaxrEzXDY4g-iUg2kl_tABOUxzoqyFkGZm3DA",
  "expires_in": 3600,
  "token_type": "Bearer",
  "scope": "read update"
}
```

The application can now use the access token to call instance APIs.

Example (Domain) API request:

```http
GET /api/domain/odata/Crm_Customers?$top=10 HTTP/1.1
Host: demodb.my.erp.net
Authorization: Bearer eyJhbGciOiJSUzI1NiIsImtpZCI6IjVCMjc5MjBFNjUzREQ3QUM2N0QyRjY0QjMyQTE3OTkyIiwidHlwIjoiYXQrand0In0.eyJuYmYiOjE3NjY1MDUxNDcsImV4cCI6MTc2NjUwODc0NywiaXNzIjoiaHR0cHM6Ly9lMS1kZXYubG9jYWwvaWQiLCJhdWQiOlsiRG9tYWluQVBJIiwiVGFibGVBUEkiLCJPTEFQIiwiQXBwU2VydmVyIiwiaHR0cHM6Ly9lMS1kZXYubG9jYWwvaWQvcmVzb3VyY2VzIl0sImNsaWVudF9pZCI6IlBLIiwiY2xpZW50X3N5c3RlbV91c2VyIjoiYWRtaW4iLCJjbGllbnRfc3lzdGVtX3VzZXJfdHlwZSI6IkludGVybmFsVXNlciIsImNsaWVudF9kYiI6IkUxX0RFVi0xIFRlc3QiLCJqdGkiOiI4RDBCNzA1NTczQTFBOThGREJBREZGMENEN0Y3RUVCNCIsImlhdCI6MTc2NjUwNTE0Nywic2NvcGUiOlsicmVhZCIsInVwZGF0ZSJdfQ.MRvH-EtWu-PWDhjDDn73OVQwH29DZ_RRu6XheFsoRxImyjLQRIU7-S1GuyTnnqPyXEEkGkKTu_s3IEwxGORgY48jLH3l1juJDt8_JvcyJlIdhVZSZNC1Bpft_K1NJswJ6QmJ6bWgev7cqHaxM3p7AEEPjkSmnAjdBCz7ItMV93Yio5kCRBmP9DQUoxtL0webG7zV_f5uOkt8xhbUVHpdU9FQY-XLf_heLJv_81vvpf39kPxD4WTZRly8X_mNdlqi0DxiFXK3TFOnbdoLKxeljke8jV0t-agaHWcZ4B-SMQ77falwtFaxrEzXDY4g-iUg2kl_tABOUxzoqyFkGZm3DA
```

## Security and implementation considerations

- This flow is intended for **machine-to-machine communication**.
- No user authentication or interaction occurs.
- The client secret must **never** be exposed outside the backend.
- Tokens are scoped and limited to the permissions configured for the application.
