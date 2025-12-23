# Authentication and Authorization FAQ

This page answers the most common questions developers have when connecting apps to @@name.

---

## Why do I need to register a Trusted Application?

Every app that signs in users or calls @@name APIs must be registered as a **Trusted Application** in your @@name instance.  

It defines who the app is, what permissions (scopes) it has, and which authentication flows it may use.  

Without this registration, @@name Identity will reject all authentication requests.

See: [Trusted Applications and Access Control](./how-apps-connect/trusted-apps-access.md)

---

## Which authentication flow should I use?

It depends on the type of app you're building:

- **Interactive web or desktop apps:** use **Authorization Code Flow** (with PKCE).  
- **Background services or automations:** use **Client Credentials Flow**.  
- **Apps with both frontend and backend:** use the **Hybrid Flow** (combined).

See: [Choosing the Right Flow](./flows/choosing-flow.md)

---

### Can I call @@name APIs without OAuth?

No.  

Basic authentication and API keys are deprecated.  

All external access must go through the @@name Identity using OAuth 2.0.

The only exception is **reference tokens** (PAT or SAT), which are manually issued and act like long-lived API keys - but even they require proper scope and authorization.

See: [Reference Access Tokens](./tokens/reference-access-tokens.md)

---

### What's the difference between a user token and a service token?

- **User token (Authorization Code Flow):** represents a person who logged in.  
- **Service token (Client Credentials Flow):** represents an application identity (system user).  

They are not interchangeable - user tokens have personal access; service tokens have system-level access.

See: [Tokens Overview](./tokens/tokens-overview.md)

---

## My login works, but API calls return 401 Unauthorized. Why?

You are probably sending an expired or missing token in your API request.

Check these:

1. Your request must include the header `Authorization: Bearer <access_token>`
2. Access tokens expire after **1 hour**. Refresh or re-authenticate the user before calling again.
3. Make sure the token was issued for **the same instance** you're calling - tokens are instance-specific.

---

### My app says "invalid_client". What does that mean?

It means your app's credentials don't match what @@name expects.

**Common reasons:**

- The **client_id** is wrong.
- The app type is **Public**, but you're sending a secret.
- The secret has been rotated or deleted.

**Fix:**  

Check your Trusted Application settings and make sure you use the correct client ID and secret.  

Public (JavaScript) apps must not send a secret at all.

See: [Interactive Apps Common Errors](./flows/auth-code/interactive-apps-errors.md)
See: [Service Apps Common Errors](./flows/client-credentials/service-apps-errors.md)

---

### I get "invalid_scope". What does that mean?

You're asking for permissions your app is not allowed to have.

**Example:**  

You request `update sec`, but your Trusted Application only has `read`.

**Fix:**  

Update the app's allowed scopes, or request only those configured in @@name.  

Common valid scopes are:  
`read`, `update`, `offline_access`, `profile`.

See: [Interactive Apps Common Errors](./flows/auth-code/interactive-apps-errors.md)
See: [Service Apps Common Errors](./flows/client-credentials/service-apps-errors.md)

---

### My redirect URI is rejected as invalid. Why?

The `redirect_uri` you send must exactly match one of the URIs defined in the Trusted Application - including **https**, casing, and trailing slash.

**Fix:**  

Edit your Trusted Application and add the correct URI, for example:

```http
https://myapp.example.com/auth/callback
```

---

### My token expired after one hour. Can I make it live longer?

No - access tokens are intentionally short-lived (1 hour) for security.  

However, **confidential** apps can use a **refresh token** to get a new one automatically.

If your app is **public** (for example, runs only in the browser), it must ask the user to sign in again when the token expires.

See: [Token Lifetime and Renewal](./tokens/token-lifetime.md)

---

### I have a refresh token, but it stopped working. Why?

Refresh tokens can expire or be revoked:

- Confidential apps: 30-day lifetime.  
- Public apps: 24-hour lifetime.  
- If the user logs out or changes their password, old tokens stop working.

Always be ready to prompt the user to sign in again if refreshing fails.

See: [Token Lifetime and Renewal](./tokens/token-lifetime.md)

---

### What's the difference between a token and a session?

- The **token** proves you're authorized.  
- The **session** is the actual connection that consumes a license.

You can have a valid token but fail to open a session if all licenses are in use.  

Sessions close automatically after inactivity (about 20 minutes).

See: [Tokens and Sessions Relationship](./sessions/token-session-relationship.md)

---

### Can I log out programmatically?

Yes - for interactive apps, redirect the user to:

```http
https://<your-instance>.my.erp.net/id/connect/endsession
```

The user will see a confirmation page and, after confirming, their session will close and release the license.

Service (non-interactive) apps do not use this endpoint - their sessions close automatically after expiration.

See: [Session Revocation and Logout](./sessions/session-revocation.md)

---

### Why does @@name reject my request with "MaximumLicenseCountExceeded"?

All licenses are currently in use.  

Even with a valid token, the system cannot open a new session until a slot becomes free.

**Fix:**  

- Wait a few minutes - inactive sessions release automatically after 20 minutes.  
- Plan license capacity based on concurrency.

See: [License Slot Usage](./sessions/license-slot.md)

---

### Can I use one token for multiple apps?

No - this violates ERP.net's license agreement.  

Each user or service must have their own session and token.  

Parallel requests with the same token share the same session and license.

See: [License Compliance and Violations](./sessions/license-compliance.md)

---

### How can I safely test authentication?

Use a **sandbox instance** or a dedicated test tenant.  

Register a separate Trusted Application there, and use dummy users or test accounts.  

Never reuse production secrets or tokens in test environments.

---

### Where can I see all available endpoints?

Each @@name Identity exposes a discovery document with full endpoint metadata:  

```http
https://<your-instance>.my.erp.net/id/.well-known/openid-configuration
```

This includes URLs for token exchange, authorization, logout, and JSON Web Keys (JWKS) for token validation.

---

## Learn More

- [OAuth 2.0 Overview](./how-apps-connect/oauth2-overview.md)  
- [Trusted Applications and Access Control](./how-apps-connect/trusted-apps-access.md)  
- [Token Lifetime and Renewal](./tokens/token-lifetime.md)  
- [License Slot Usage](./sessions/license-slot.md)
