# Redirect URIs and PKCE

Interactive apps use browser redirects to sign users in and return an authorization code to the app.  

This page explains how **redirect URIs** are validated in @@name and how to secure the Authorization Code flow with **PKCE**.

## Redirect URIs

A redirect URI is where @@name Identity sends the user after a successful sign-in (or error).  

For security, it must match **exactly** one of the URIs registered in the app’s **Trusted Application**.

### Rules

- Exact, case-sensitive match against the **ImpersonateLoginUrl** list in the Trusted Application.
- Use **HTTPS** for public internet apps.
- You may register multiple URIs (for example, dev, staging, production).
- Query string may be present in the registered value, but matching is exact. Do not rely on wildcard suffixes.
- For native apps, use **loopback** or **custom scheme** redirects (see below).

> [!NOTE]  
> If the redirect URI does not match exactly, @@name Identity will reject the request.

### Common redirect URI patterns

| App type | Example redirect URI | Notes |
|---|---|---|
| Server-side web app | `https://app.example.com/auth/callback` | Confidential client; handles code exchange on server |
| SPA (browser) | `https://spa.example.com/index.html` | Public client; must use PKCE |
| Native desktop | `http://127.0.0.1:53177/callback` | Loopback with random port; PKCE required. Can also use embedded browsers such as WebView or Chromium for login and redirect handling. |
| Mobile app | `myapp://auth/callback` | Custom scheme; PKCE required |
| Local dev | `https://localhost:5001/signin-callback` | Use HTTPS with a dev certificate if possible |

> [!NOTE]  
> Desktop and mobile apps typically use system or embedded browsers to perform the OAuth redirect, such as WebView, Chromium, or OS-provided login components.

## PKCE overview

**PKCE (Proof Key for Code Exchange)** protects the authorization code from interception.  

It adds a one-time secret to the flow so only the app that started the login can finish the token exchange.

PKCE is **required** for:

- SPAs and other public clients that cannot keep a client secret.
- Native apps (desktop and mobile).

PKCE is **recommended** for confidential web apps too.

### How PKCE works (at a glance)

1. The app generates a random `code_verifier`.  
2. The app computes `code_challenge = BASE64URL(SHA256(code_verifier))`.  
3. The app starts the authorize request with `code_challenge` and `code_challenge_method=S256`.  
4. After sign-in, the app exchanges the returned `code` at the token endpoint and must present the original `code_verifier`.  
5. @@name Identity verifies that `SHA256(code_verifier)` matches the original `code_challenge`.

If an attacker steals the `code` from the redirect, it is useless without the `code_verifier`.

## Build the authorize request

Minimal parameters for Authorization Code + PKCE:

```http
GET /id/connect/authorize?
  response_type=code&
  client_id=my.trusted.app&
  redirect_uri=https://spa.example.com/index.html&
  scope=DomainApi read offline_access&
  state=kj82F3&
  code_challenge=2C5h2Bf2yq9e8q8WRzU7Hc8l4o7l5s2fC3VfLbaN9yM&
  code_challenge_method=S256 HTTP/1.1
Host: testdb.my.erp.net
```

- `state` is required for CSRF protection. Generate and validate it per request.
- Include `offline_access` only if you need a refresh token (interactive apps).

## Exchange the code with PKCE

Token request for a public client (no secret):

```http
POST /id/connect/token HTTP/1.1
Host: testdb.my.erp.net
Content-Type: application/x-www-form-urlencoded

grant_type=authorization_code&
client_id=my.trusted.app&
code=K0dE...short...GZmNjV&
redirect_uri=https://spa.example.com/index.html&
code_verifier=<the_exact_random_string_used_for_code_challenge>
```

For confidential server apps, you may use either PKCE or a `client_secret`:

```http
POST /id/connect/token HTTP/1.1
Host: testdb.my.erp.net
Content-Type: application/x-www-form-urlencoded

grant_type=authorization_code&
client_id=my.trusted.app/server&
client_secret=<your_client_secret>&
code=K0dE...short...GZmNjV&
redirect_uri=https://app.example.com/auth/callback
```

## Generating PKCE values

Example pseudocode:

```text
code_verifier = base64url(random_bytes(32))        # keep in memory/storage for this login
code_challenge = base64url(sha256(code_verifier))  # send in authorize request
```

Guidelines:

- `code_verifier` should be high-entropy (32 to 64 random bytes before Base64URL).
- Use **Base64URL** encoding (no `+`, `/`, or padding `=`).
- Store `code_verifier` only long enough to finish the exchange, then discard.

## Security checklist

- Register all **redirect URIs** in the Trusted Application and keep them in sync across environments.
- Use **PKCE** for all public clients (SPA, native, mobile). It is recommended for confidential web apps.
- Validate the `state` parameter on return to prevent CSRF.
- Do not allow open redirects. Keep redirect URIs fixed and exact.
- Never put access tokens in URLs. Tokens belong in the `Authorization` header only.

---

## Learn More

- [**Interactive Apps (Authorization Code Flow)**](overview.md)  
  Overview of the user-facing flow and when to use it.

- [**Step-by-Step Example**](interactive-apps-step-by-step.md)  
  Full walkthrough of authorize, callback, and token exchange.

- [**Refresh Tokens**](interactive-apps-refresh-tokens.md)  
  Renew access without forcing the user to sign in again.

- [**Common Errors**](interactive-apps-errors.md)  
  Troubleshooting invalid redirects, PKCE mismatches, and token issues.

- [**Trusted Applications and Access Control**](../../configuration/trusted-apps-access.md)  
  How app registrations define allowed redirects, users, and scopes.
