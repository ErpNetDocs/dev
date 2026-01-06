# Instance ID for Web Portals

This topic describes how a **web portal** integrates with an @@name Instance ID using **both** the **Authorization Code** flow and the **Client Credentials** flow within a **single confidential application**.

A typical example of such a web portal is an **online store**, where users sign in to manage their profile or place orders, while all backend operations and API calls are performed by the application itself.

## Scenario Overview

The following scenario is covered:

- The external application is a **confidential web application (portal)**
- The application has a **backend component** capable of securely storing a secret
- Users sign in interactively (internal and/or external)
- The application uses:
  - **Authorization Code** flow to authenticate users
  - **Client Credentials** flow to access instance APIs
- API calls are executed using a service identity
- A **single Trusted Application** is used for both flows

The **Instance ID** is the identity provider of a specific @@name instance. Confidential applications use it to authenticate users and obtain tokens **against a concrete @@name instance**.

For example, if your @@name instance is located at:

```text
https://mycompany.my.erp.net
```

then the corresponding Instance ID endpoints are located at:

```text
https://mycompany.my.erp.net/id
```

## Design Goal

The goal of this approach is to clearly separate user authentication from API access.

- Users are authenticated to establish identity and session context
- API access is performed using a **service identity**, not the user
- External (community) users can sign in without receiving API permissions
- Backend integrations remain stable and centrally controlled

## Flow Separation

| Purpose | Flow Used | Token Used | Identity |
|--------|-----------|------------|----------|
| User sign-in | Authorization Code | ID token | Signed-in user |
| API access | Client Credentials | Access token | Application system user |

In this model, the Authorization Code flow is used **only** to identify the user, while the Client Credentials flow is used to authorize all API calls.

## Prerequisites

Your @@name instance must have a trusted application defined with the configuration below.

> [!NOTE]
> The values shown below are **examples only** and must be replaced with values that match your application.

Typical values:

- **ApplicationUri**: `portal.example.com`
- **ImpersonateLoginUrl**: `https://portal.example.com/signin-callback`
- **SystemUser**: `<an-internal-erp-user>`

> [!NOTE]
> The application owner must generate a random client secret, compute its **Base64-encoded SHA-256 hash**, and submit the hashed value via an internal ticket to [erp.net](https://support.erp.net/) so the Trusted Application can be registered and the configuration activated.

| Attribute | Value | Comment |
| --------- | ----- | ------- |
| Name | My web portal | Used only for user-friendly identification. |
| ApplicationUri | portal.example.com | The unique identifier of the application. |
| IsEnabled | true | Enables the trusted application. |
| SystemUserAllowed | true | Allows the application to authenticate as a service. |
| SystemUser | `<an-internal-erp-user>` | The internal user used for service authentication. |
| ImpersonateAsInternalUserAllowed | true | Allows authentication by internal users. |
| ImpersonateAsCommunityUserAllowed | true | Allows authentication by external (community) users. |
| ImpersonateLoginUrl | https://portal.example.com/signin-callback | The callback URL handled by the backend after sign-in. |
| ClientType | Confidential | Indicates that the application can securely store a secret. |
| ApplicationSecretHash | `<base64(sha256(your-client-secret))>` | The hashed client secret used during token requests. |
| Scope | `read` or `read update` | Use `read` for read-only access; include `update` only if the application must create, modify, or delete data |

All other attributes can keep their default values and are not relevant for this scenario.

## Implementation

This section demonstrates how a web portal uses:

- **Authorization Code** flow to authenticate users
- **Client Credential** flow to obtain access tokens for calling instance APIs

### 1. Start user sign-in (Authorization Code)

The portal redirects the user's browser to the Instance ID authorize endpoint.

For each sign-in request, generate:

- `state` – `<base64url(random(32 bytes))>`
- `nonce` – `<base64url(random(32 bytes))>`
- `code_verifier` – `<random(64 chars)>`
- `code_challenge` – `<base64url(sha256(code_verifier))>`

Authorize request example:

```http
GET /id/connect/authorize?
  client_id=portal.example.com&
  redirect_uri=https%3A%2F%2Fportal.example.com%2Fsignin-callback&
  response_type=code%20id_token&
  response_mode=form_post&
  scope=openid%20profile&
  state=YzQ5ZDc4M2QxN2Q0NDU3YjlhM2Y5OWE1ZTY4OTc0ZGM&
  nonce=ZTRjOTM2M2Y0ODFhNGQ0NGE4NmU1N2ExYjY2NjE3ZmQ&
  code_challenge=VZ9R6l3kPpJ6m2m9kR9cR9n8s3c7J4GmQ9FZp5mX1kQ&
  code_challenge_method=S256
Host: mycompany.my.erp.net
```

**Result:**  
The user is redirected to the @@name login page and signs in.

> [!NOTE]
> The openid and profile scopes are requested because this is an OpenID Connect authentication flow that issues an ID token with basic user claims; these scopes are not defined in the Trusted Application because they are protocol-level scopes provided automatically based on the authorization flow.

### 2. Receive the sign-in response (callback)

After a successful sign-in, the @@name Instance ID redirects the user back to the configured callback URL.

Example callback request received by the backend:

```http
GET /signin-callback?
  code=SplxlOBeZQQYbYS6WxSbIA&
  state=YzQ5ZDc4M2QxN2Q0NDU3YjlhM2Y5OWE1ZTY4OTc0ZGM
Host: portal.example.com
```

The backend must:

- Validate that the returned `state` equals the original value
- Extract the `authorization code`
- Preserve the original `code_verifier` for the token request

### 3. Exchange the authorization code (user tokens)

The backend exchanges the authorization code at the Instance ID token endpoint.

Because the application is confidential, the request includes the client secret.
The request must include the original `code_verifier`.

> [!NOTE]
> In this portal model, tokens from this step are used for **user identification** (ID token validation), not for calling instance APIs.

```http
POST /id/connect/token HTTP/1.1
Host: mycompany.my.erp.net
Content-Type: application/x-www-form-urlencoded

grant_type=authorization_code&
client_id=portal.example.com&
client_secret=<PLAIN_CLIENT_SECRET>&
code=SplxlOBeZQQYbYS6WxSbIA&
redirect_uri=https%3A%2F%2Fportal.example.com%2Fsignin-callback&
code_verifier=dBjftJeZ4CVP-mB92K27uhbUJU1p1r_wW1gFWFOEjXk
```

Example token response:

```http
HTTP/1.1 200 OK
Content-Type: application/json

{
  "id_token": "eyJhbGciOiJSUzI1NiIsImtpZCI6IjVCMjc5MjBFNjUzREQ3QUM2N0QyRjY0QjMyQTE3OTkyIiwidHlwIjoiSldUIn0.eyJuYmYiOjE3NjY0OTM3ODEsImV4cCI6MTc2NjQ5NDA4MSwiaXNzIjoiaHR0cHM6Ly9lMS1kZXYubG9jYWwvaWQiLCJhdWQiOiJQSyIsIm5vbmNlIjoiNjUwMTAwMWU0NGUzNGFlY2FiMTQzMTRiMDI4YzM0YzQiLCJpYXQiOjE3NjY0OTM3ODEsImF0X2hhc2giOiIwYWpYNkhYODNtbmJVNVhxVDUtUUZ3Iiwic19oYXNoIjoib1RRalZZUXdBRDZEMWV2TzlBTVNrUSIsInNpZCI6IkQxRkZFREZFNENCMzIzMDBEMjZDMjY1QjlCRDM3NEZCIiwic3ViIjoiYWRtaW4iLCJhdXRoX3RpbWUiOjE3NjY0OTM0OTUsImlkcCI6ImxvY2FsIiwiYW1yIjpbInB3ZCJdfQ.hL4SpH9yoMx5j0e2KMmd6Xttw3M5ktRNbs39m2cJ0-8MvCIoeTkkNtasXS7burHX9mh1vxAow1kYYdcedOQNRWERyGsNQM3jy1yal5jCixBRTPkC86bkAeS_h9Q7gWWLFSrSeaHvi_xJGoRFBLymS44dM20kZxFI9o1qZfOX79hnNnIoF_j7BEV0u77lj7Bb7OE3Xh2cyBcn0-yQB9SmGhesvt5B2IqZ0odn6il_qMabnAK_sm80b-4HtA4w8uLCCpD4JBg6g4O2zmhm_jVUQtYsqS9VjPMA4F1-pTB3ayrP40Dvq_cGP5dPspenR2GCxJqAg0bQu8AcBqLJkVcD-w",
  "access_token": "eyJhbGciOiJSUzI1NiIsImtpZCI6IjVCMjc5MjBFNjUzREQ3QUM2N0QyRjY0QjMyQTE3OTkyIiwidHlwIjoiYXQrand0In0.eyJuYmYiOjE3NjY0OTM3ODEsImV4cCI6MTc2NjQ5NzM4MSwiaXNzIjoiaHR0cHM6Ly9lMS1kZXYubG9jYWwvaWQiLCJhdWQiOiJodHRwczovL2UxLWRldi5sb2NhbC9pZC9yZXNvdXJjZXMiLCJjbGllbnRfaWQiOiJQSyIsInN1YiI6ImFkbWluIiwiYXV0aF90aW1lIjoxNzY2NDkzNDk1LCJpZHAiOiJsb2NhbCIsImlkIjoiOWRhNjQ4MzktYThkMC00OTFkLWFlYmItNGQxOGZhNDJiMDE0IiwibmFtZSI6IkpvaG4gRG9lIiwiZW1haWwiOiJhZG1pbkBtYWlsLmNvbSIsInVzZXJfdHlwZSI6IkludGVybmFsVXNlciIsImlzX2FkbWluIjoidHJ1ZSIsImVtYWlsX3ZlcmlmaWVkIjoiZmFsc2UiLCJkYiI6IkUxX0RFVi0xIFRlc3QiLCJsb2NhbGUiOiJiZyIsImp0aSI6IkM4MTcxRjlFQ0FEN0U2OThEMjBFN0Q2Qjc5ODQyMjYwIiwic2lkIjoiRDFGRkVERkU0Q0IzMjMwMEQyNkMyNjVCOUJEMzc0RkIiLCJpYXQiOjE3NjY0OTM3ODEsInNjb3BlIjpbIm9wZW5pZCIsInByb2ZpbGUiXSwiYW1yIjpbInB3ZCJdfQ.TWePn6yinGcAaItZzfmNM9en3ESfGFhcLulTR7U7wfVK86RW9s8LlsgqoAc2SkigH2cXrovnADCe8uwp7BFSKbQBsN18vH5GmidFpILEr-M_WnOWfFZt941SA04vXpBzDJaiunzRl4FAEMd4RKXJ2L_wCZW8DNaKmsZfS9SH8N6QAYABrNoHZbrRV83pUh7suUnsBF_Ps-jdFNTnuxoj4UooRghAp7dn6LYw3MBcTb3GNdrJosb63DXpQY-Kq58gbYrov0Of4H_RD6lEaKcFbRVmIpg5S1UoM2DFaS-PWxKN_DF9_4A8zazBHM8BgGN4hA7JPOv6W7waODLvNqhnzQ",
  "expires_in": 3600,
  "scope": "openid profile"
}
```

The application can now validate the ID token to identify the user.

### 4. Request an access token for API calls (Client Credentials)

The portal backend requests an access token using the Client Credentials flow.

```http
POST /id/connect/token HTTP/1.1
Host: mycompany.my.erp.net
Content-Type: application/x-www-form-urlencoded

client_id=portal.example.com&
client_secret=<PLAIN_CLIENT_SECRET>&
grant_type=client_credentials&
scope=read%20update
```

> [!NOTE]
> The `client_secret` is sent as a **plain (unhashed) value** and must exactly match the secret configured for the trusted application.

**Result:**  

If the request is successful, the Instance ID returns an access token.

```json
{
  "access_token": "eyJhbGciOiJSUzI1NiIsImtpZCI6IjVCMjc5MjBFNjUzREQ3QUM2N0QyRjY0QjMyQTE3OTkyIiwidHlwIjoiYXQrand0In0.eyJuYmYiOjE3NjY1MDUxNDcsImV4cCI6MTc2NjUwODc0NywiaXNzIjoiaHR0cHM6Ly9lMS1kZXYubG9jYWwvaWQiLCJhdWQiOlsiRG9tYWluQVBJIiwiVGFibGVBUEkiLCJPTEFQIiwiQXBwU2VydmVyIiwiaHR0cHM6Ly9lMS1kZXYubG9jYWwvaWQvcmVzb3VyY2VzIl0sImNsaWVudF9pZCI6IlBLIiwiY2xpZW50X3N5c3RlbV91c2VyIjoiYWRtaW4iLCJjbGllbnRfc3lzdGVtX3VzZXJfdHlwZSI6IkludGVybmFsVXNlciIsImNsaWVudF9kYiI6IkUxX0RFVi0xIFRlc3QiLCJqdGkiOiI4RDBCNzA1NTczQTFBOThGREJBREZGMENEN0Y3RUVCNCIsImlhdCI6MTc2NjUwNTE0Nywic2NvcGUiOlsicmVhZCIsInVwZGF0ZSJdfQ.MRvH-EtWu-PWDhjDDn73OVQwH29DZ_RRu6XheFsoRxImyjLQRIU7-S1GuyTnnqPyXEEkGkKTu_s3IEwxGORgY48jLH3l1juJDt8_JvcyJlIdhVZSZNC1Bpft_K1NJswJ6QmJ6bWgev7cqHaxM3p7AEEPjkSmnAjdBCz7ItMV93Yio5kCRBmP9DQUoxtL0webG7zV_f5uOkt8xhbUVHpdU9FQY-XLf_heLJv_81vvpf39kPxD4WTZRly8X_mNdlqi0DxiFXK3TFOnbdoLKxeljke8jV0t-agaHWcZ4B-SMQ77falwtFaxrEzXDY4g-iUg2kl_tABOUxzoqyFkGZm3DA",
  "expires_in": 3600,
  "token_type": "Bearer",
  "scope": "read update"
}
```

### 5. Call instance APIs using the service access token

The following example uses the **Domain API**.

```http
GET /api/domain/odata/Crm_Customers?$top=10 HTTP/1.1
Host: mycompany.my.erp.net
Authorization: Bearer eyJhbGciOiJSUzI1NiIsImtpZCI6IjVCMjc5MjBFNjUzREQ3QUM2N0QyRjY0QjMyQTE3OTkyIiwidHlwIjoiYXQrand0In0.eyJuYmYiOjE3NjY1MDUxNDcsImV4cCI6MTc2NjUwODc0NywiaXNzIjoiaHR0cHM6Ly9lMS1kZXYubG9jYWwvaWQiLCJhdWQiOlsiRG9tYWluQVBJIiwiVGFibGVBUEkiLCJPTEFQIiwiQXBwU2VydmVyIiwiaHR0cHM6Ly9lMS1kZXYubG9jYWwvaWQvcmVzb3VyY2VzIl0sImNsaWVudF9pZCI6IlBLIiwiY2xpZW50X3N5c3RlbV91c2VyIjoiYWRtaW4iLCJjbGllbnRfc3lzdGVtX3VzZXJfdHlwZSI6IkludGVybmFsVXNlciIsImNsaWVudF9kYiI6IkUxX0RFVi0xIFRlc3QiLCJqdGkiOiI4RDBCNzA1NTczQTFBOThGREJBREZGMENEN0Y3RUVCNCIsImlhdCI6MTc2NjUwNTE0Nywic2NvcGUiOlsicmVhZCIsInVwZGF0ZSJdfQ.MRvH-EtWu-PWDhjDDn73OVQwH29DZ_RRu6XheFsoRxImyjLQRIU7-S1GuyTnnqPyXEEkGkKTu_s3IEwxGORgY48jLH3l1juJDt8_JvcyJlIdhVZSZNC1Bpft_K1NJswJ6QmJ6bWgev7cqHaxM3p7AEEPjkSmnAjdBCz7ItMV93Yio5kCRBmP9DQUoxtL0webG7zV_f5uOkt8xhbUVHpdU9FQY-XLf_heLJv_81vvpf39kPxD4WTZRly8X_mNdlqi0DxiFXK3TFOnbdoLKxeljke8jV0t-agaHWcZ4B-SMQ77falwtFaxrEzXDY4g-iUg2kl_tABOUxzoqyFkGZm3DA
```

## Security and implementation considerations

- The client secret must **never** be exposed to frontend code.
- Use Authorization Code flow tokens for **identity** and session context only.
- Use Client Credentials flow tokens for all **API calls**.
- Restrict scopes to the minimum required by the portal backend.
- The `state` and `nonce` values must be generated per request and validated.
- Redirect URIs must exactly match the configured value.
- The Instance ID **does not allow embedding**. If authentication is initiated from within an iframe, the sign-in flow must be opened in a **popup window or top-level navigation**.
