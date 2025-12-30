# Token Lifetime and Renewal

@@name uses multiple token types, each with its own lifetime and renewal strategy.  

Understanding these durations helps design secure and reliable authentication flows.

## Access Tokens

Access tokens are **short-lived** credentials used to authorize API requests.

- **Lifetime:** 1 hour  
- **Purpose:** To authenticate API calls on behalf of a user or application.  
- **Renewal:** Obtain a new access token via a **refresh token** (interactive flows) or by repeating the **client credentials** request (service flows).  
- **Reasoning:** Short validity limits the impact of token leakage.

> [!NOTE]  
> Access tokens are automatically verified by @@name APIs on every request.  
> Expired tokens result in HTTP 401 (Unauthorized).

## Refresh Tokens

Refresh tokens are **longer-lived** credentials that allow obtaining new access tokens without requiring the user to log in again.

- **Lifetime:** 1 month for confidential apps / 24 hours for public apps
- **Issued To:** Confidential, interactive applications (authorization code flow).  
- **Not Available For:** Non-interactive (service) applications.  
- **Purpose:** Maintain seamless user sessions without re-authentication.  
- **Usage:** Exchange at `/id/connect/token` with `grant_type=refresh_token`.

Example:

```http
POST /id/connect/token
Content-Type: application/x-www-form-urlencoded

client_id=my.trusted.app
client_secret=<app_secret>
grant_type=refresh_token
refresh_token=<refresh_token_value>
```

## Reference Access Tokens (PAT, SAT)

Reference tokens are manually issued, long-lived credentials designed for persistent or automated access.

- **Lifetime:** User-specified (days, months, or indefinite).  
- **Types:**  
  - **PAT** – Personal Access Token (represents a specific user identity).  
  - **SAT** – Service Access Token (represents the system user defined in the Trusted Application).  
- **Revocation:** Immediate; can be manually revoked from the Profile Site (PATs) or the Instance Manager Site (SATs).  
- **Requirements:** The corresponding Trusted Application must allow issuing reference tokens through its **AccessTokens** policy (`AuthenticatedUsers` or `AdministratorsOnly`).  
- **Use Case:** Persistent integrations, automation services, and developer utilities that need stable credentials across sessions.

> [!NOTE]  
> Use reference tokens only when automation or persistent access is required.  
> For all other cases, prefer short-lived OAuth access tokens.

## Renewal Strategy

| Token Type | Lifetime | How to Renew | Typical Use Case |
|-------------|-----------|---------------|------------------|
| **Access Token** | 1 hour | Refresh token or new token request | API calls |
| **Refresh Token** | 1 month (confidential) / 24 hours (public) | User re-authentication | Interactive apps |
| **Reference Token (PAT, SAT)** | User-defined | Manual reissue | Long-lived integrations |

## Learn More

- [**Access Tokens**](access-tokens.md)  
  Understand token format, claims, and validation.

- [**Reference Access Tokens (PAT, SAT)**](reference-access-tokens.md)  
  Learn how to create and manage long-lived manual tokens.

- [**Scopes**](../configuration/scopes.md)  
  See how scopes define access levels in all token types.
