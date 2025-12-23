# Common Errors in Interactive Apps

When implementing **Authorization Code flow** in an interactive app, errors can occur at different stages of the process.  

Most are caused by incorrect configuration in the **Trusted Application**, invalid redirects, or token misuse.

This table lists the most common issues, their causes, and how to fix them.

---

| **Error** | **Typical Message** | **Cause** | **Fix** |
|------------|--------------------|------------|----------|
| **Invalid Redirect URI** | `invalid_redirect_uri` | The `redirect_uri` in your request doesn’t match any value in `ImpersonateLoginUrl` or `ImpersonateLogoutUrl` of the Trusted Application. | Make sure the URI matches exactly (case-sensitive). Use HTTPS in production. Example: `https://app.example.com/auth/callback`. |
| **Missing or Invalid Client Credentials** | `invalid_client` | Wrong `client_id` or `client_secret`. Trying to use a secret for a Public client or missing it for a Confidential one. | Verify `client_id` matches the app’s `ApplicationUri`. Use the correct secret for Confidential clients. |
| **Unauthorized Grant Type** | `unsupported_grant_type` | The grant type is not allowed by the Trusted Application. | Only use `authorization_code` or `refresh_token`. Check flags like `SystemUserAllowed` and impersonation settings. |
| **PKCE Validation Failed** | `invalid_grant: PKCE verification failed` | The PKCE challenge or verifier does not match. | Make sure your SPA/mobile app uses the same `code_verifier` and `code_challenge` (method `S256`). |
| **Invalid Scope** | `invalid_scope` | The requested scopes aren’t allowed for the app. | Verify the Trusted Application’s `Scope` field includes them. Example: `openid profile offline_access DomainApi`. |
| **Expired or Invalid Authorization Code** | `invalid_grant` | Code expired, was reused, or redirect URIs don’t match. | Exchange the code immediately, match `redirect_uri` exactly, and don’t reuse codes. |
| **Invalid or Expired Refresh Token** | `invalid_grant: refresh token is expired or invalid` | The refresh token expired, was revoked, or the app config changed. | Use a new authorization code (user sign-in). Always store the latest refresh token. |
| **Token Expired** | API returns `401 Unauthorized` | The access token expired or was revoked. | Use refresh tokens to get a new access token or reauthenticate. |
| **Disabled or Missing Trusted Application** | `unauthorized_client` | The Trusted Application record is disabled or missing. | Verify the app exists, `IsEnabled = true`, and you’re using the correct instance. |

---

## Learn More

- [**Interactive Apps (Authorization Code Flow)**](overview.md)  
  Overview of how interactive apps authenticate users.

- [**Redirect URIs and PKCE**](interactive-apps-redirects-pkce.md)  
  How to secure redirects and apply PKCE.

- [**Refresh Tokens**](interactive-apps-refresh-tokens.md)  
  How to renew tokens without requiring user reauthentication.

- [**Trusted Applications and Access Control**](../../how-apps-connect/trusted-apps-access.md)  
  Learn how app registration affects permissions and validation.
