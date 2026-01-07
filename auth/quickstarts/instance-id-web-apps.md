# Instance ID for Web (Confidential) Applications

This topic describes how a **confidential external application** with a **backend** component capable of securely storing a secret integrates with an @@name Instance ID in order to authenticate users and access protected APIs.

The following scenario is covered:

- The external application is a **confidential application**
- The application has a **backend component** capable of securely storing a secret
- The application authenticates users against a **specific @@name instance**
- The application uses the **Authorization Code flow**
- The application obtains tokens to call instance APIs on behalf of the signed-in user

The **Instance ID** is the identity provider of a specific @@name instance. Confidential applications use it to authenticate users and obtain tokens **against a concrete @@name instance**.

For example, if your @@name instance is located at:

```text
https://mycompany.my.erp.net
```

then the corresponding Instance ID endpoints are located at:

```text
https://mycompany.my.erp.net/id
```

## Prerequisites

Your @@name instance must have a trusted application defined with the configuration below.

> [!NOTE]
> The values shown below are **examples only** and must be replaced with values that match your application.

Typical values:

- **ApplicationUri**: `backend.example.com`
- **ImpersonateLoginUrl**: `https://backend.example.com/signin-callback`

> [!NOTE]
> The application owner must generate a random client secret, compute its **Base64-encoded SHA-256 hash**, and submit the hashed value via an internal ticket to [erp.net](https://support.erp.net/) so the Trusted Application can be registered and the configuration activated.

| Attribute | Value | Comment |
| --------- | ----- | ------- |
| Name | My confidential app | Used only for user-friendly identification. |
| ApplicationUri | backend.example.com | The unique identifier of the application. |
| IsEnabled | true | Enables the trusted application. |
| ImpersonateAsInternalUserAllowed | true | Allows authentication by internal users. |
| ImpersonateAsCommunityUserAllowed | false | Disallow authentication by external (community) users. |
| ImpersonateLoginUrl | https://backend.example.com/signin-callback | The callback URL handled by the backend after sign-in. |
| ClientType | Confidential | Indicates that the application can securely store a secret. |
| ApplicationSecretHash | `<base64(sha256(your-client-secret))>` | The hashed client secret used during the token request. |
| Scope | `read` or `read update` | Use `read` for read-only access; include `update` only if the application must create, modify, or delete data |

All other attributes can keep their default values and are not relevant for this scenario.

## Implementation

This section demonstrates the Authorization Code flow against the Instance ID for a confidential application.

### 1. Start sign-in (authorize request)

The application redirects the user's browser to the Instance ID authorize endpoint.

For each sign-in request, generate:

- `state` – `<base64url(random(32 bytes))>`
- `nonce` – `<base64url(random(32 bytes))>`
- `code_verifier` - `<random(64 chars)>`
- `code_challenge` - `<base64url(sha256(code_verifier))>`

Authorize request example:

```http
GET /id/connect/authorize?
  client_id=backend.example.com&
  redirect_uri=https%3A%2F%2Fbackend.example.com%2Fsignin-callback&
  response_type=code%20id_token&
  response_mode=form_post&
  scope=openid%20profile%20offline_access%20read%20update&
  state=YzQ5ZDc4M2QxN2Q0NDU3YjlhM2Y5OWE1ZTY4OTc0ZGM&
  nonce=ZTRjOTM2M2Y0ODFhNGQ0NGE4NmU1N2ExYjY2NjE3ZmQ&
  code_challenge=VZ9R6l3kPpJ6m2m9kR9cR9n8s3c7J4GmQ9FZp5mX1kQ&
  code_challenge_method=S256
Host: mycompany.my.erp.net
```

The requested scopes define the level of access the application will have to the instance APIs.

Common resource scopes:

- `read` – allows **read-only** access to instance resources
- `update` – allows **update** access to instance resources

Request only the scopes required by the application.  
Granting broader scopes than necessary increases the impact of a compromised token.

> [!NOTE]
> The openid and profile scopes are requested because this is an OpenID Connect authentication flow that issues an ID token with basic user claims; these scopes are not defined in the Trusted Application because they are protocol-level scopes provided automatically based on the authorization flow.

**Result:**
The user is redirected to the @@name login page and signs in.

### 2. Receive the sign-in response (callback)

After a successful sign-in, the @@name Instance ID redirects the user back to the configured callback URL.

Example callback request received by the backend:

```http
GET /signin-callback?
  code=SplxlOBeZQQYbYS6WxSbIA&
  state=YzQ5ZDc4M2QxN2Q0NDU3YjlhM2Y5OWE1ZTY4OTc0ZGM
Host: backend.example.com
```

The backend must:

- Validate that the returned `state` equals the original value
- Extract the `authorization code`
- Preserve the original `code_verifier` for the token request

### 3. Exchange the authorization code (token request)

The backend exchanges the authorization code at the Instance ID token endpoint.

Because the application is confidential, the request includes the client secret.
The request must include the original `code_verifier`.

```http
POST /id/connect/token HTTP/1.1
Host: mycompany.my.erp.net
Content-Type: application/x-www-form-urlencoded

grant_type=authorization_code&
client_id=backend.example.com&
client_secret=<PLAIN_CLIENT_SECRET>&
code=SplxlOBeZQQYbYS6WxSbIA&
redirect_uri=https%3A%2F%2Fbackend.example.com%2Fsignin-callback&
code_verifier=dBjftJeZ4CVP-mB92K27uhbUJU1p1r_wW1gFWFOEjXk
```

Example token response:

```http
HTTP/1.1 200 OK
Content-Type: application/json

{
  "access_token": "eyJhbGciOiJSUzI1NiIsImtpZCI6IjVCMjc5MjBFNjUzREQ3QUM2N0QyRjY0QjMyQTE3OTkyIiwidHlwIjoiYXQrand0In0.eyJuYmYiOjE3NjY0OTQzNDYsImV4cCI6MTc2NjQ5Nzk0NiwiaXNzIjoiaHR0cHM6Ly9lMS1kZXYubG9jYWwvaWQiLCJhdWQiOlsiRG9tYWluQVBJIiwiVGFibGVBUEkiLCJPTEFQIiwiQXBwU2VydmVyIiwiaHR0cHM6Ly9lMS1kZXYubG9jYWwvaWQvcmVzb3VyY2VzIl0sImNsaWVudF9pZCI6IlBLIiwic3ViIjoiYWRtaW4iLCJhdXRoX3RpbWUiOjE3NjY0OTM0OTUsImlkcCI6ImxvY2FsIiwiaWQiOiI5ZGE2NDgzOS1hOGQwLTQ5MWQtYWViYi00ZDE4ZmE0MmIwMTQiLCJuYW1lIjoiSm9obiBEb2UiLCJlbWFpbCI6ImFkbWluQG1haWwuY29tIiwidXNlcl90eXBlIjoiSW50ZXJuYWxVc2VyIiwiaXNfYWRtaW4iOiJ0cnVlIiwiZW1haWxfdmVyaWZpZWQiOiJmYWxzZSIsImRiIjoiRTFfREVWLTEgVGVzdCIsImxvY2FsZSI6ImJnIiwianRpIjoiQTUwODY2NDY2NjU3QjM0QjAxNjlGNTRCMTZEODdGN0QiLCJzaWQiOiJEMUZGRURGRTRDQjMyMzAwRDI2QzI2NUI5QkQzNzRGQiIsImlhdCI6MTc2NjQ5NDM0Niwic2NvcGUiOlsib3BlbmlkIiwicHJvZmlsZSIsInJlYWQiLCJ1cGRhdGUiLCJvZmZsaW5lX2FjY2VzcyJdLCJhbXIiOlsicHdkIl19.c9hwe4Jq9V3EoNWugPAcbGbgV_YTLf5MM2HaeUCCvsHdinE5G-g9ZPlVNR8HtQwkz_pzZd192U2_WkrhqweWWsOsAnSihfLlmOQIY0ak2z50_CTpatyHzVkgXGhTieBrCa8myaiBNO57trs9f8kGSbEsx6y3GxCiOd4TDLtfAtDRHyvUmI5PKZLle4dzmVph_srZOS4a4JmOrJGFFJ3hNB5y-dytDV0UNuzrl3QzbyEbU9HJPwCAObXNJf-LqqSnK_CEY54NnUx9IY4UGsUskOg7g131FvDF9Bx_jz7JYEruNfMeq6UTUVjiNJUWaOF7G_twOR5r9w9wbawTt8uV6Q",
  "id_token": "eyJhbGciOiJSUzI1NiIsImtpZCI6IjVCMjc5MjBFNjUzREQ3QUM2N0QyRjY0QjMyQTE3OTkyIiwidHlwIjoiSldUIn0.eyJuYmYiOjE3NjY0OTQzNDYsImV4cCI6MTc2NjQ5NDY0NiwiaXNzIjoiaHR0cHM6Ly9lMS1kZXYubG9jYWwvaWQiLCJhdWQiOiJQSyIsIm5vbmNlIjoiNzdlN2JjOGQ1NjI4NDI3Njk2NjdiNDM0MjliZWFmODUiLCJpYXQiOjE3NjY0OTQzNDYsImF0X2hhc2giOiJXRGtwcms1UWZScm9YTVJhMlEwckhBIiwic19oYXNoIjoiV044dE14Y2VFVFJkNkVaUmxNeWYwQSIsInNpZCI6IkQxRkZFREZFNENCMzIzMDBEMjZDMjY1QjlCRDM3NEZCIiwic3ViIjoiYWRtaW4iLCJhdXRoX3RpbWUiOjE3NjY0OTM0OTUsImlkcCI6ImxvY2FsIiwiYW1yIjpbInB3ZCJdfQ.Hle13-x4R5INI9VAOqjn_i7frwmf3_jIrDYh_Kg9tyRpg5EExzPSu-Lr0cyulE_hTXG7aFf-a4XYq_fu37mi2XtiXtiqasuOiPzF5PETuxSgn8F8wBfqIxm6EFW-H4cIfxLeY6ODVEEzax4iWAUvKwQkYPRUWMe8GXJLoEIz8JjBaW0kQUFy-djOVNoJcdytqeEAG3oG6vtvAP51sBijWrWPe4JCz_ly7fVa34BTICfH1MigWicUJcFmXx1CwPC9CoSWOqcEleHV3W-R9YesnOUFRgEOiNeSpeUhhXE7HtRZgxWqCjQAQ_AElsSO-pahQ8mWRgpIrn7x_nUXyzRXZQ",
  "refresh_token": "CEA03BDB9ADF650B43215BD114962DEFFBF99B75957F5DFDC781E7315A1CE9E4",
  "expires_in": 3600,
  "scope": "openid profile offline_access read update"
}
```

The application can now:

- Validate the ID token to identify the user
- Use the access token to call instance APIs on behalf of the signed-in user

## Security and implementation considerations

- Confidential applications **must handle callbacks and token requests on the backend**.
- The client secret must **never** be exposed to frontend code.
- PKCE is recommended even for confidential clients to reduce authorization code interception risks.
- The `state` and `nonce` values must be generated per request and validated.
- Redirect URIs must exactly match the configured value.
- The Instance ID **does not allow embedding**. If authentication is initiated from within an iframe, the sign-in flow must be opened in a **popup window or top-level navigation**.
