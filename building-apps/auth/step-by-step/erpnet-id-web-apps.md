# ERP.net ID for Web Applications

@@name Identity acts as an external identity provider.  
It enables external applications to identify users and trust the identity information they receive.

This topic covers the following scenario:

- A user signs in through @@name Identity
- The application receives a verified user identity
- The application uses the received identity to recognize the signed-in user

## Prerequisites

@@name Identity must have a defined trusted application with the configuration below.

> [!NOTE]
> The values shown below (such as `myapp.myhost.net` and the sign-in callback URL) are **examples only**.  
> They must be replaced with values that match the actual domain and callback endpoint of your external application.

Typical values are:

- **ApplicationUri**: a DNS name or URI that uniquely identifies your application  
  (for example: `myapp.myhost.net`)
- **ImpersonateLoginUrl**: a publicly reachable callback endpoint handled by your backend  
  (for example: `https://myapp.myhost.net/signin-callback`)

| Attribute | Value | Comment |
| --------- | ----- | ------- |
| Name | My trusted app | Used only for user-friendly identification. |
| ApplicationUri | myapp.myhost.net | The unique identifier of the trusted application. This value must be unique and is defined by the external application. |
| IsEnabled | true | Enables the trusted application. |
| ImpersonateAsInternalUserAllowed | true | Allows authentication of internal users. |
| ImpersonateAsCommunityUserAllowed | true | Allows authentication of external (community) users. |
| ImpersonateLoginUrl | https://myapp.myhost.net/signin-callback | The callback URL of the external application. This must exactly match the URL where the application listens for the sign-in response. |
| ClientType | Confidential | Indicates that the external application is a confidential client and can securely store a secret. |
| ApplicationSecretHash | `<base64(sha256(your-secret))>` | The hashed secret of the external application. |

All other attributes can keep their default values and are not relevant for this scenario.

## Implementation

This section demonstrates a sign-in flow that uses a hybrid response type (`code id_token`) for compatibility with existing integrations.  
The flow also uses PKCE (Proof Key for Code Exchange).

### 1. Start sign-in (authorize request)

The external application redirects the user's browser to the @@name Identity authorize endpoint at **`id.erp.net`**.

Generate the following values per sign-in request:

- `state` – `<base64url(random(32 bytes))>`
- `nonce` – `<base64url(random(32 bytes))>`
- `code_verifier` - `<random(64 chars)>`
- `code_challenge` - `<base64url(sha256(code_verifier))>`

Authorize request example:

```http
GET /id/connect/authorize?
  client_id=myapp.myhost.net&
  redirect_uri=https%3A%2F%2Fmyapp.myhost.net%2Fsignin-callback&
  response_type=code%20id_token&
  response_mode=form_post&
  scope=openid%20profile&
  state=YzQ5ZDc4M2QxN2Q0NDU3YjlhM2Y5OWE1ZTY4OTc0ZGM&
  nonce=ZTRjOTM2M2Y0ODFhNGQ0NGE4NmU1N2ExYjY2NjE3ZmQ&
  code_challenge=VZ9R6l3kPpJ6m2m9kR9cR9n8s3c7J4GmQ9FZp5mX1kQ&
  code_challenge_method=S256
Host: id.erp.net
```

### 2. Receive the sign-in response (callback)

After a successful sign-in, @@name Identity redirects the user to the configured callback URL using `form_post`.

Example callback request received by the external application:

```http
POST /signin-callback/ HTTP/1.1
Host: myapp.myhost.net
Content-Type: application/x-www-form-urlencoded

code=SplxlOBeZQQYbYS6WxSbIA&state=YzQ5ZDc4M2QxN2Q0NDU3YjlhM2Y5OWE1ZTY4OTc0ZGM&id_token=eyJhbGciOiJSUzI1NiIsImtpZCI6IjVCMjc5MjBFNjUzREQ3QUM2N0QyRjY0QjMyQTE3OTkyIiwidHlwIjoiSldUIn0.eyJuYmYiOjE3NjY0OTM3ODEsImV4cCI6MTc2NjQ5NDA4MSwiaXNzIjoiaHR0cHM6Ly9lMS1kZXYubG9jYWwvaWQiLCJhdWQiOiJQSyIsIm5vbmNlIjoiNjUwMTAwMWU0NGUzNGFlY2FiMTQzMTRiMDI4YzM0YzQiLCJpYXQiOjE3NjY0OTM3ODEsImF0X2hhc2giOiIwYWpYNkhYODNtbmJVNVhxVDUtUUZ3Iiwic19oYXNoIjoib1RRalZZUXdBRDZEMWV2TzlBTVNrUSIsInNpZCI6IkQxRkZFREZFNENCMzIzMDBEMjZDMjY1QjlCRDM3NEZCIiwic3ViIjoiYWRtaW4iLCJhdXRoX3RpbWUiOjE3NjY0OTM0OTUsImlkcCI6ImxvY2FsIiwiYW1yIjpbInB3ZCJdfQ.hL4SpH9yoMx5j0e2KMmd6Xttw3M5ktRNbs39m2cJ0-8MvCIoeTkkNtasXS7burHX9mh1vxAow1kYYdcedOQNRWERyGsNQM3jy1yal5jCixBRTPkC86bkAeS_h9Q7gWWLFSrSeaHvi_xJGoRFBLymS44dM20kZxFI9o1qZfOX79hnNnIoF_j7BEV0u77lj7Bb7OE3Xh2cyBcn0-yQB9SmGhesvt5B2IqZ0odn6il_qMabnAK_sm80b-4HtA4w8uLCCpD4JBg6g4O2zmhm_jVUQtYsqS9VjPMA4F1-pTB3ayrP40Dvq_cGP5dPspenR2GCxJqAg0bQu8AcBqLJkVcD-w
```

The external application must:

- Validate that the returned `state` equals the value sent in the authorize request
- Extract the `authorization code`

### 3. Exchange the authorization code (token request)

The external application exchanges the authorization code at the token endpoint.

The token request must include the original `code_verifier`.

```http
POST /id/connect/token HTTP/1.1
Host: id.erp.net
Content-Type: application/x-www-form-urlencoded

client_id=myapp.myhost.net&
client_secret=<PLAIN_CLIENT_SECRET>&
grant_type=authorization_code&
code=SplxlOBeZQQYbYS6WxSbIA&
redirect_uri=https%3A%2F%2Fmyapp.myhost.net%2Fsignin-callback&
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

The application can use the returned ID token to identify and recognize the signed-in user.

## Implementation considerations

- Because the application is a **confidential client**, the sign-in callback **must be handled by a backend component**. Handling the callback in frontend code would expose sensitive data.
- For the same reason, the **token request must be performed by the backend**. The client secret is stored and used there and must never be sent to or stored in the frontend.
- The **client secret must never be exposed** in browser code, mobile apps, or any other client-side environment.
- The `state` value must be generated per sign-in request and **validated on callback**. Skipping this opens the door to CSRF attacks.
- The `code_verifier` must be generated per request and **stored securely** until the token exchange is performed.
- The `nonce` value must be unique per sign-in request and **validated against the ID token**. Reusing a static nonce defeats its purpose.
- @@name Identity at `id.erp.net` **does not allow embedding** in iframes. If authentication is initiated from within an iframe, the sign-in flow must be opened in a **popup window or a top-level browser navigation**.
