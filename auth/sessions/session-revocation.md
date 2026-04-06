# Session Revocation and Logout

Sessions in @@name represent active, licensed connections between an authenticated user or app and the @@name instance.  

When a session ends, its allocated license slot is immediately released and returned to the pool, becoming available for other users or integrations.

## How Sessions End

A session can end in several ways:

- **Interactive User Logout:** The user explicitly signs out via the @@name Identity provider.
- **Inactivity Timeout:** The session closes automatically if no API requests are made for **20 minutes**.
- **Absolute Expiration:** Service sessions (Client Credentials Flow) enforce a hard limit and expire after **1 hour**, regardless of activity.
- **Server Restart:** All active sessions are cleared if the application server is restarted.

> [!NOTE]
> **Tokens vs. Sessions:** Session lifetime and license usage are conceptually independent of Access Token validity.
> A token may still be cryptographically valid even after its associated @@name session is closed. If a valid token is used again, the system will automatically allocate a new session.

## Logging Out (Interactive Apps)

Interactive applications utilizing the **Authorization Code Flow** must end the user's identity and resource sessions by invoking the @@name Identity `endsession` endpoint.

```http
GET https://<instance>.my.erp.net/id/connect/endsession?id_token_hint=<id_token>&post_logout_redirect_uri=<your_callback_uri>
```

**Execution Flow:**

1. The user's browser is redirected to the Identity provider.
2. The Identity provider destroys the local authentication cookie (Identity Session).
3. @@name terminates the backend connection and immediately releases the license slot (@@name Session).
4. The user is redirected back to the client application (if `post_logout_redirect_uri` is provided).

> [!WARNING]
> This endpoint is strictly for **interactive flows** where a human user is present in a browser.
> **Service applications** using the Client Credentials flow cannot and should not invoke this mechanism.

## Service Sessions

Service (non-interactive) applications do not have a logout endpoint. Their license lifecycle is governed entirely by automated timeouts:

- **Inactivity**: The session ends automatically after 20 minutes of no API calls.
- **Absolute Limit**: The session is force-closed exactly 1 hour after it was created.

Once either threshold is reached, the license slot is released automatically without requiring client intervention.

## After Revocation: The Re-Activation Behavior

Because Access Tokens are stateless, closing a session (via logout or timeout) does not invalidate the token itself.

If a client application makes a new API call using an unexpired Access Token after the session has ended:

- The system will **reactivate** the connection and attempt to allocate a new license slot.
- The request will succeed if a license is available in the pool.
- A `401 Unauthorized` error will only be returned if the token itself has naturally expired or its cryptographic signature is invalid.

> [!CAUTION]
> **Licensing Compliance**: Programmatically forcing a logout and immediately reusing the token to cycle sessions is considered **License Multiplexing**.
> Relying on this re-activation behavior to artificially reduce concurrent license counts is a direct violation of @@name licensing terms.

---

## Learn More

- [**Tokens and Sessions Relationship**](token-session-relationship.md)  
  How tokens start sessions and how both lifetimes interact.

- [**License Slot Usage**](license-slot.md)  
  How licenses are consumed, released, and reserved.

- [**License Compliance and Violations**](license-compliance.md)  
  Understand correct license usage, avoid token sharing, and ensure compliance with @@name licensing terms.
