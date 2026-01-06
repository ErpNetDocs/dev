# Sessions overview

Tokens, sessions, and licenses in @@name are related but **not the same thing**.

An access token only represents **authorization** - proof that a user or application has the right to call an API.

A session, on the other hand, represents **an active connection** between @@name and that user or app.  

Each session consumes a **license slot** while active.

Understanding how these three elements interact helps ensure your integrations are efficient, compliant, and predictable.

## Key Concepts

| Concept | Description |
|----------|--------------|
| **Access Token** | A signed credential proving that the caller (user or app) is authorized. Obtaining a token does **not** reserve a license. |
| **Session** | Created automatically on the first API request using a valid token. Each session represents an active logical connection to the @@name server. |
| **License Slot** | A limited resource consumed by an open session. Each session occupies exactly one license slot. When the session closes, the slot is released. |

## Typical Lifecycle

1. **App obtains an access token** from @@name Identity.  
   No license or session is created yet.
2. **First API call** using the token.  
   @@name opens a session and reserves a license slot.
3. **Subsequent API calls** reuse the same session.  
   Session lifetime is extended automatically.
4. **Inactivity or explicit logout** closes the session.  
   License slot is released.

> [!NOTE]  
> You can hold multiple valid access tokens without consuming any licenses,  
> until one of them is actually used to access @@name data.

## Session Expiration Basics

A session has two independent expiration timers:

- **Sliding Expiration:** 20 minutes of inactivity.  
  Each API request refreshes this timer. If no requests occur within 20 minutes, the session is closed.
- **Absolute Expiration:** Applies only to background or service sessions (Client Credentials flow).  
  The session ends automatically after **1 hour**, even if continuously active.

These timers control **session lifetime**, not token validity.

> [!NOTE]  
> A valid token does not guarantee an active session.  
> If the session has expired, the next API request will attempt to open a new one.

## Summary

- Access tokens authorize - sessions consume licenses.  
- A token can be used **only within the same @@name instance** where it was issued.  
- Sessions automatically close after inactivity or absolute expiration.  
- Revoking a token does **not** immediately free a license - only closing the session does.

---

## Learn More

- [**Tokens and Sessions Relationship**](token-session-relationship.md)  
  Understand how @@name links API calls, tokens, and sessions.

- [**License Slot Usage**](license-slot.md)  
  Learn how sessions consume and release license slots.

- [**Session Revocation and Logout**](session-revocation.md)  
  See how sessions are closed automatically or by user action.

- [**License Compliance and Violations**](license-compliance.md)  
  Understand correct license usage, avoid token sharing, and ensure compliance with @@name licensing terms.

- [**Token Lifetime and Renewal**](../tokens/token-lifetime.md)  
  Learn how access and refresh tokens expire and renew independently from sessions.
