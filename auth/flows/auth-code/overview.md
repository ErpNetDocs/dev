# Interactive Apps (Authorization Code Flow)

Interactive applications connect to @@name using the **Authorization Code flow** - the standard OAuth 2.0 pattern for securely authenticating users and obtaining tokens on their behalf.

This flow is used by web, desktop, and mobile apps that have a **user interface** and require access to data under the signed-in user's permissions.

## Current support and migration

Starting with **ERP.net 27.1.2.8**, the instance's Identity site accepts `response_type=code` with PKCE for registrations that support Hybrid. Public Trusted Applications already configured for Authorization Code could use PKCE on earlier builds.

Use Authorization Code with PKCE for new integrations and migrate existing interactive clients to it. Send a PKCE challenge with `code_challenge_method=S256` and include the original `code_verifier` when exchanging the code. Confidential apps also authenticate with their client secret. See the [step-by-step guide](interactive-apps-step-by-step.md).

| Sign-in request | Current behavior |
|-----------------|------------------|
| Authorization Code with PKCE | Supported for public and confidential interactive apps. A Hybrid-capable registration requires PKCE when it requests `response_type=code`. |
| OpenID Connect Hybrid (`response_type=code id_token`) | Remains available for backward compatibility where the existing registration permits it. The sign-in callback includes both the authorization code and the ID token. |
| Sign-in without PKCE | Remains available for backward compatibility where the existing registration permits it. |

With `response_type=code`, the app receives the code in the callback. Read it from the query string when using the default response mode. For OpenID Connect sign-in with the `openid` scope, read the ID token from the token endpoint response after exchanging the code. See [OpenID Connect Core](https://openid.net/specs/openid-connect-core-1_0.html#CodeFlowAuth).

**Authorization Code with PKCE will become mandatory** once all interactive clients, including external integrations, have migrated. Apps that also run background jobs continue to use [Client Credentials](../client-credentials/overview.md) for that work.

## When to Use

Use this flow when your application:

- Has a user interface where people sign in (browser, desktop, or mobile).  
- Needs access to user-specific data or permissions.  
- Should never handle user passwords directly.  
- Can securely handle redirect URIs and (if confidential) a client secret.

## How It Works

The Authorization Code flow separates user authentication from API access.  

The app redirects the user to the @@name instance's **Identity**, where the user signs in.  

After successful authentication, @@name Identity issues a short-lived **authorization code**, which the app exchanges for an **access token** (and optionally a refresh token).

The app then uses the access token to call @@name APIs on behalf of the user.

```mermaid
sequenceDiagram
    participant User
    participant App
    participant IDP as ERP.net Identity
    participant API as ERP.net APIs

    User->>App: Open app and click "Sign In"
    App->>IDP: Redirect to /connect/authorize with a PKCE challenge
    IDP->>User: Prompt for login and consent
    User->>IDP: Authenticate with credentials
    IDP->>App: Redirect back with authorization code
    App->>IDP: POST /connect/token with code and verifier plus client authentication if confidential
    IDP-->>App: Return access token (and optional refresh token)
    App->>API: Call ERP.net APIs with the access token
    API-->>App: Return protected data
```

> [!NOTE]  
> Tokens are issued by **@@name Identity** of the target @@name instance.  
> Each instance validates and authorizes the app through its Trusted Application configuration.

## Trusted Application Relationship

Before an interactive app can connect to an @@name instance, it must be registered as a **Trusted Application**.  
The Trusted Application record defines:

- The app's **identity** (`ApplicationUri`, used as `client_id`).  
- Whether the app is **Public** (no secret) or **Confidential** (uses a client secret).  
- The **redirect URIs** allowed for login and logout.  
- Which **users** can authenticate (internal or community).  
- The **scopes** and permissions the app can request.

During the sign-in process, @@name Identity checks the app's registration, validates the redirect URI, and ensures the scopes requested match the Trusted Application configuration.

> [!NOTE]  
> If the app is not registered or its configuration does not match, @@name Identity will reject the authentication request.

## Security Highlights

- **Redirect URIs** must exactly match the ones registered in the Trusted Application.  
- Use **PKCE (Proof Key for Code Exchange)** for interactive public and confidential clients.
- **Client secrets** must only be used by secure, confidential clients (for example, backend servers).  
- Access tokens are **short-lived** and can be renewed via **refresh tokens** if permitted.  

## Example Clients

| App Type | Example | Client Type | Notes |
|-----------|----------|-------------|-------|
| Web App (Server-side) | Internal portal, admin dashboard | Confidential | Uses PKCE and a client secret. Can receive refresh tokens. |
| SPA (Browser-based) | Customer portal, analytics dashboard | Public | Uses PKCE; no client secret |
| Mobile App | Field service or sales app | Public | Uses PKCE; must secure redirect handling |

## Typical Sequence Summary

1. User opens the app and selects **Sign In**.  
2. App redirects to @@name Identity with a PKCE challenge.
3. @@name Identity checks the Trusted Application configuration (redirects, scopes, client type).  
4. The user signs in and grants consent.  
5. @@name Identity redirects back to the app with an authorization code.  
6. The app exchanges the code for an access token using the PKCE verifier.
7. The app uses the token to call @@name APIs.  
8. If allowed, the app silently renews the token using a refresh token.

> [!WARNING]  
> Always request only the **scopes** your app truly needs.  
> Smaller scopes mean safer tokens and fewer consent prompts.

---

## Learn More

- [**Step-by-Step Example**](interactive-apps-step-by-step.md)  
  Walk through the full login and token exchange sequence.

- [**Redirect URIs and PKCE**](interactive-apps-redirects-pkce.md)  
  How to secure redirects and apply PKCE correctly.

- [**Refresh Tokens**](interactive-apps-refresh-tokens.md)  
  How to renew tokens without requiring the user to sign in again.

- [**Trusted Applications and Access Control**](../../configuration/trusted-apps-access.md)  
  Learn how app registrations define access modes, users, and permissions.

- [**Common Errors**](interactive-apps-errors.md)  
  Troubleshooting invalid redirects, PKCE mismatches, and token issues.
