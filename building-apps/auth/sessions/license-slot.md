# License Slot Usage

Each active session in @@name occupies one **license slot**.  

Licenses are consumed and released automatically based on session activity - not on token lifetime.

## How Licensing Works

- A license is used only when a **session** is opened.  
- A license is freed when that session **closes** (due to inactivity, timeout, or logout).  
- **Tokens do not consume licenses** on their own - they are only credentials used to start a session.  
- The number of concurrent sessions determines the total license usage in the system.

> [!NOTE]  
> A valid token does not guarantee license availability.  
> If all licenses are in use, opening a new session will fail.

## License Duration

Each session maintains its license as long as it remains active:

- **Interactive sessions**: renewed automatically with user activity.  
- **Service sessions (Client Credentials)**: last up to 1 hour absolute, even if active.  
- **Inactive sessions**: automatically released after 20 minutes of inactivity.

## Hybrid App Licensing

In a hybrid setup:

- The **frontend** uses user licenses for interactive sessions.  
- The **backend** uses a single **system user license** for automated operations.  
- External users never consume @@name licenses directly - they operate through the backend's system user.

This architecture allows thousands of external users to interact through one licensed backend identity.

## Licensing Compliance

- One @@name session = one license slot.  
- Each internal user must have their own license.  
- Background services must use dedicated system users.  
- Multiplexing (sharing one license among multiple users) is not allowed.  
- Close inactive or redundant sessions to free licenses faster.

## Reserved License Seats

Reserved license seats guarantee that specific users can always connect, even when all other licenses are in use.

These reservations are defined by administrators.

### How It Works

- A list of usernames can be assigned as **reserved accounts**.  
- Each reserved account is guaranteed at least one available license slot.  
- Other users share the remaining unreserved licenses on a first-come, first-served basis.  
- If all unreserved licenses are in use, additional users will be denied access until a slot is freed.

### Example

If a company has 10 total licenses and 2 reserved for specific users:

- Those 2 users can always log in.  
- The remaining 8 licenses are available to everyone else.  
- If all 8 are in use, new sessions from non-reserved users will fail until a license becomes free.

### Best Practices

- Reserve licenses for critical accounts (administrators, automation users, integrations).  
- Keep the reserved list small to avoid starving general users.  
- Monitor active sessions to ensure reserved users are actually utilizing their slots efficiently.

> [!NOTE]  
> Reserved licenses do **not** bypass inactivity or expiration rules - they simply guarantee availability when starting a session.

---

## Learn More

- [**Tokens and Sessions Relationship**](token-session-relationship.md)  
  How sessions start, expire, and reconnect.

- [**License Compliance and Violations**](license-compliance.md)  
  Understand correct license usage, avoid token sharing, and ensure compliance with @@name licensing terms.

- [**Session Revocation and Logout**](session-revocation.md)  
  How to explicitly close sessions and release their licenses.

- [**Trusted Applications and Access Control**](../configuration/trusted-apps-access.md)  
  Learn how system users and app modes determine license usage.
