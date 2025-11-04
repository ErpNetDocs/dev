# Tokens and Sessions Relationship

In @@name, an **access token** is only a proof of authorization - it does **not** open a session or consume a license.  

A **session** is created only when the token is actually used to call an @@name API.

This distinction is important for understanding why a user may still hold a valid token but no active session.

## How It Works

| Step | Action | What Happens |
|------|---------|---------------|
| 1 | The app obtains an **access token** from @@name Identity Server. | No session yet. The token only proves identity and permissions. |
| 2 | The app makes its **first API call** using that token. | @@name opens a new session, assigns it to the token's user or system identity, and consumes one license slot. |
| 3 | The app continues making API calls. | The same session is reused and its sliding expiration timer (20 minutes) is refreshed after each request. |
| 4 | The app becomes inactive. | After 20 minutes without requests, the session closes automatically and releases the license slot. |
| 5 | The app makes a new API call with the **same still-valid token**. | @@name opens a new session (if a license is available) and continues execution. If all licenses are in use, an error is returned. |

> [!NOTE]  
> Tokens and sessions are independent: a token may still be valid even if its session has expired.

## Token Expiration vs Session Expiration

| Event | What It Affects | Description |
|--------|----------------|-------------|
| **Token expires** | Authorization | The app must renew or refresh the token. Existing sessions are unaffected but cannot be reused once expired. |
| **Session expires** | Connection and licensing | The session is destroyed after inactivity or reaching absolute lifetime. A new session will be created on the next request. |

In short:  

- Token = proof of access  
- Session = active connection consuming a license

## Refreshing Tokens

When a refresh token is used to obtain a new access token:

- The **session remains the same**, if still active.  
- The new token continues using the same session identity.  
- No additional license is consumed.

> [!NOTE]  
> Refreshing a token does **not** open a new session - it simply extends authorization for the existing one.

## Example Timeline

| Time | Event | Token State | Session State | License |
|------|--------|--------------|----------------|----------|
| 10:00 | User logs in and obtains access token | Valid | No session | Not used |
| 10:01 | First API call | Valid | Session created | 1 license used |
| 10:15 | Active usage | Valid | Session alive | License held |
| 10:35 | Inactive for 20+ min | Valid | Session closed | License released |
| 10:40 | New API call with same token | Valid | New session opened | New license used |
| 11:00 | Token expires | Expired | Session may still exist | License may still be held until inactivity timeout |

> [!WARNING]  
> A valid token does **not guarantee** that a license is available.  
> If all licenses are used, a new session cannot be opened even with a valid token.

## Summary

- Sessions are created **on demand**, not when tokens are issued.  
- Token validity and session lifetime are separate mechanisms.  
- Refreshing a token keeps the same session identity.  
- If a session expires, the next API call will attempt to create a new one.  
- Each session consumes one license while active.

---

## Learn More

- [**License Slot Usage**](license-slot.md)  
  Understand how sessions consume and release license slots.

- [**License Compliance and Violations**](license-compliance.md)  
  Understand correct license usage, avoid token sharing, and ensure compliance with @@name licensing terms.

- [**Session Revocation and Logout**](session-revocation.md)  
  Learn how users and apps can close sessions explicitly.

- [**Token Lifetime and Renewal**](../tokens/token-lifetime.md)  
  See how tokens expire and refresh independently from sessions.
