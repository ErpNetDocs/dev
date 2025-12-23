# Session Revocation and Logout

Sessions in @@name represent active, licensed connections between an authenticated user or app and the @@name instance.  

When a session ends, its license slot is immediately released and can be used by another user or integration.

## How Sessions End

A session can end in several ways:

- **User logout** – The user explicitly signs out via @@name Identity.  
- **Inactivity timeout** – After 20 minutes of inactivity, the session closes automatically.  
- **Absolute timeout** – Service sessions (Client Credentials Flow) expire after 1 hour, even if active.  
- **Server restart** – All sessions are cleared when the application server restarts.  

> [!NOTE]  
> Session lifetime and license usage are completely independent of token validity.  
> A token may still be valid even after its session is closed - a new session will be created automatically when it's used again.

## Logging Out (Interactive Apps)

Interactive applications using the **Authorization Code Flow** can explicitly end the user session via @@name Identity logout endpoint.

```http
GET https://<instance>.my.erp.net/id/connect/endsession
```

When this endpoint is called:

- The user is redirected to a logout confirmation page.  
- After confirmation, @@name Identity terminates the session.  
- The session license is released immediately.  
- The user can safely close the browser or sign in again.

> [!WARNING]  
> This logout flow only applies to **interactive sessions** where a user is present.  
> **Service applications** (using Client Credentials) do not use this mechanism.

## Service Sessions

Service (non-interactive) applications cannot log out interactively.  

Their sessions end automatically when:

- The 1-hour absolute expiration time passes, or  
- The application becomes inactive for 20 minutes.

Once the session ends, the license slot is released automatically.

## After Revocation

When a session closes - either due to logout, timeout, or inactivity:

- The license slot becomes available for other users or services.  
- If the **token is still valid**, the **next API call with the same token** will **reopen (reactivate) the same session**, and a license will be allocated again if one is available.  
- A `401 Unauthorized` occurs only if the token itself is **expired** or **invalid** (not simply because the previous session ended).

---

## Learn More

- [**Tokens and Sessions Relationship**](token-session-relationship.md)  
  How tokens start sessions and how both lifetimes interact.

- [**License Slot Usage**](license-slot.md)  
  How licenses are consumed, released, and reserved.

- [**License Compliance and Violations**](license-compliance.md)  
  Understand correct license usage, avoid token sharing, and ensure compliance with @@name licensing terms.

- [**Trusted Applications and Access Control**](../how-apps-connect/trusted-apps-access.md)  
  How trusted app configuration affects license behavior and user access.
