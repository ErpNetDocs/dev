# Instance ID for SPA (Public) Applications

This topic describes how a **Single Page Application (SPA)** with no backend component, running entirely in the user's browser, integrates with an @@name Instance ID in order to authenticate users and access protected APIs.

The following scenario is covered:

- The external application is a **Single Page Application (SPA)** running **entirely** in the user's browser
- The application is a **public client** and **cannot keep a secret**
- The application uses the **Authorization Code flow with PKCE**
- The application authenticates users and obtains tokens **from a specific @@name instance**
- The application uses the obtained access token to call APIs on behalf of the signed-in user

The **Instance ID** is the identity provider of a specific @@name instance. It's used by external applications to authenticate and authorize users **against a concrete @@name instance**.

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
> The values shown below are **examples only** and must be replaced with values that match your SPA.

Typical values:

- **ApplicationUri**: `spa.example.com`
- **ImpersonateLoginUrl**: `https://spa.example.com/signin-callback`

| Attribute | Value | Comment |
| --------- | ----- | ------- |
| Name | My SPA app | Used only for user-friendly identification. |
| ApplicationUri | spa.example.com | The unique identifier of the SPA. |
| IsEnabled | true | Enables the trusted application. |
| ImpersonateAsInternalUserAllowed | true | Allows authentication by internal users. |
| ImpersonateAsCommunityUserAllowed | false | Disallows authentication by external (community) users. |
| ImpersonateLoginUrl | https://spa.example.com/signin-callback | The callback URL handled by the SPA after sign-in. |
| ClientType | Public | Indicates that the application is a public client and cannot keep a secret. |

All other attributes can keep their default values and are not relevant for this scenario.

## Implementation

This section demonstrates the Authorization Code flow with PKCE against the Instance ID.

### 1. Start sign-in (authorize request)

Before starting the sign-in flow, the SPA generates:

- `state` – `<base64url(random(32 bytes))>`
- `nonce` – `<base64url(random(32 bytes))>`
- `code_verifier` - `<random(64 chars)>`
- `code_challenge` - `<base64url(sha256(code_verifier))>`

The SPA then redirects the browser to the @@name Instance ID authorize endpoint:

```http
GET /id/connect/authorize?
  client_id=spa.example.com&
  redirect_uri=https%3A%2F%2Fspa.example.com%2Fsignin-callback&
  response_type=code&
  scope=openid%20profile%20read%20update&
  state=YzQ5ZDc4M2QxN2Q0NDU3YjlhM2Y5OWE1ZTY4OTc0ZGM&
  nonce=ZTRjOTM2M2Y0ODFhNGQ0NGE4NmU1N2ExYjY2NjE3ZmQ&
  code_challenge=VZ9R6l3kPpJ6m2m9kR9cR9n8s3c7J4GmQ9FZp5mX1kQ&
  code_challenge_method=S256
Host: mycompany.my.erp.net
```

The requested scopes define the level of access the SPA will have to the instance APIs.

Common resource scopes:

- `read` – allows **read-only** access to instance resources
- `update` – allows **update** access to instance resources

Request only the scopes required by the application.  
Granting broader scopes than necessary increases the impact of a compromised token.

**Result:**
The user is redirected to the @@name login page and signs in.

## 2. Receive the sign-in response (callback)

After a successful sign-in, @@name Instance ID redirects the browser back to the configured callback URL.

Example redirect received by the SPA:

```http
GET /signin-callback?
  code=SplxlOBeZQQYbYS6WxSbIA&
  state=YzQ5ZDc4M2QxN2Q0NDU3YjlhM2Y5OWE1ZTY4OTc0ZGM
Host: spa.example.com
```

The SPA must:

- Validate that the returned `state` equals the original value
- Extract the `authorization code`
- Preserve the original `code_verifier` for the token request

## 3. Exchange the authorization code (token request)

The SPA exchanges the authorization code at the @@name Instance ID token endpoint.

Because the application is a public client, no client secret is sent.
The request must include the original `code_verifier`.

```http
POST /id/connect/token HTTP/1.1
Host: mycompany.my.erp.net
Content-Type: application/x-www-form-urlencoded

grant_type=authorization_code&
client_id=spa.example.com&
code=SplxlOBeZQQYbYS6WxSbIA&
redirect_uri=https%3A%2F%2Fspa.example.com%2Fsignin-callback&
code_verifier=dBjftJeZ4CVP-mB92K27uhbUJU1p1r_wW1gFWFOEjXk
```

Example token response:

```http
HTTP/1.1 200 OK
Content-Type: application/json

{
  "access_token": "eyJhbGciOiJSUzI1NiIsImtpZCI6IjVCMjc5MjBFNjUzREQ3QUM2N0QyRjY0QjMyQTE3OTkyIiwidHlwIjoiYXQrand0In0.eyJuYmYiOjE3NjY0OTQzOTQsImV4cCI6MTc2NjQ5Nzk5NCwiaXNzIjoiaHR0cHM6Ly9lMS1kZXYubG9jYWwvaWQiLCJhdWQiOlsiRG9tYWluQVBJIiwiVGFibGVBUEkiLCJPTEFQIiwiQXBwU2VydmVyIiwiaHR0cHM6Ly9lMS1kZXYubG9jYWwvaWQvcmVzb3VyY2VzIl0sImNsaWVudF9pZCI6IlBLIiwic3ViIjoiYWRtaW4iLCJhdXRoX3RpbWUiOjE3NjY0OTM0OTUsImlkcCI6ImxvY2FsIiwiaWQiOiI5ZGE2NDgzOS1hOGQwLTQ5MWQtYWViYi00ZDE4ZmE0MmIwMTQiLCJuYW1lIjoiSm9obiBEb2UiLCJlbWFpbCI6ImFkbWluQG1haWwuY29tIiwidXNlcl90eXBlIjoiSW50ZXJuYWxVc2VyIiwiaXNfYWRtaW4iOiJ0cnVlIiwiZW1haWxfdmVyaWZpZWQiOiJmYWxzZSIsImRiIjoiRTFfREVWLTEgVGVzdCIsImxvY2FsZSI6ImJnIiwianRpIjoiNUIyNzBBRDU2RDA5OTU1QzQ0Qzg0NjdFODI0NkM4ODEiLCJzaWQiOiJEMUZGRURGRTRDQjMyMzAwRDI2QzI2NUI5QkQzNzRGQiIsImlhdCI6MTc2NjQ5NDM5NCwic2NvcGUiOlsib3BlbmlkIiwicHJvZmlsZSIsInJlYWQiLCJ1cGRhdGUiXSwiYW1yIjpbInB3ZCJdfQ.VeTP6xc1tKxqybNxficqpNz2zvpgJ-zn0evArsYd7oJYTmEqWOiCa4cfM5w4NSpUr_TuSqcrs4o3TO0-aJYvhJ2l3-NhjSDtQTeevHLGw78V_xh5EDVS0JEhhCqNrl0tABXEa5Sn-bNNEc2UDIDlsEy0LPAKIbL9p2iCgZu1jPJcvZoVR_P3l9K0SjDm-DE_OHNEqju_hX7uV70J7O7ZiSoleaJVWK2l26YcwoII0VxtP-rOpUOIQx1RBCGMSFg4W5sUGvfj0MLnt1aMztQ2iI2Ai8pqyxq09te_UQOTSUxcpwHbsBcROCFc1Lb8s-uKwpPf8WkoZpOMtJg18gLe8Q",
  "id_token": "eyJhbGciOiJSUzI1NiIsImtpZCI6IjVCMjc5MjBFNjUzREQ3QUM2N0QyRjY0QjMyQTE3OTkyIiwidHlwIjoiSldUIn0.eyJuYmYiOjE3NjY0OTQzOTQsImV4cCI6MTc2NjQ5NDY5NCwiaXNzIjoiaHR0cHM6Ly9lMS1kZXYubG9jYWwvaWQiLCJhdWQiOiJQSyIsIm5vbmNlIjoiYjdjMTY3ZWU4NDVjNGFjMWJhYTZjNWFjMTA5Y2RlYTIiLCJpYXQiOjE3NjY0OTQzOTQsImF0X2hhc2giOiJIUU9RQVFnb3ZVbVE4U2ZCZFQteW9RIiwic19oYXNoIjoiV1hjUTc3aHlKb2o5M3YwT1o1UlpQZyIsInNpZCI6IkQxRkZFREZFNENCMzIzMDBEMjZDMjY1QjlCRDM3NEZCIiwic3ViIjoiYWRtaW4iLCJhdXRoX3RpbWUiOjE3NjY0OTM0OTUsImlkcCI6ImxvY2FsIiwiYW1yIjpbInB3ZCJdfQ.dgYjJV9KrEZ39RgXv7O-Fxux5Rk6vXGJN4MW6IkRzHQIP1hr_CivtOWKx8dMVWb1LTLLBh-Lxkuylu48C2i6lBv_gU8mp-NATHer-d5r2TZkYWeO-Ml7EoLStkHxND34mZG3u0fCjhhUAdBsCAr8yidct32G0YPRS6LBxL_EtiXAncabE0WzulhcbwXo0NzohQowFKL9V3yz3DaK0oKkfkmmvkuAJkAO0rSJYtai-q7G8eiTwCk0TwDTX6_TawMy29gMYuYnHMZ00-FjnG2kA9JBqpbcSSCw4PfFiWCOEPcPsBRn28Hdv46OWaxZ2nBbk2pjkaNjva0olTyItcAcdQ",
  "expires_in": 3600,
  "scope": "openid profile read update"
}
```

The SPA can now:

- Validate the ID token to identify the user
- Use the access token to call APIs on behalf of the signed-in user

## Security and implementation considerations

- SPAs are **public clients**. A client secret must **never** be used or expected.
- PKCE is **mandatory** and must not be omitted.
- The `state` value must be generated per request and **validated on callback**.
- The `nonce` value must be **validated against the ID token**.
- Tokens must be stored carefully (for example, in memory). Persistent browser storage increases exposure.
- The Instance ID **does not allow embedding**. If authentication is initiated from within an iframe, the sign-in flow must be opened in a **popup window or top-level navigation**.
- Redirect URIs must exactly match the configured value.
