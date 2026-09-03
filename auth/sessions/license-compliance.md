# License Compliance and Violations

@@name uses a strict **one user on one device = one license** model.  

Every session must represent a single user or service identity. Sessions of the same user from the same device share one concurrent license slot; every other session consumes a slot of its own.  

Improper token or session usage can lead to **license violations**.

## What Is Allowed

- One user working in **several windows, tabs, or applications on the same device**. These share a single license, which is why an embedded application inside the Web Client costs nothing extra.  
- One user signed in on a **second device**, such as a phone next to a workstation. This takes a second license, because each device is licensed separately.

## Common Violations

- **Token sharing**  
  Using the same access token for multiple users, machines, or processes.  
  Access tokens are tied to a single identity and session context - sharing them spreads one license across multiple users.  
  The device is fixed when the user signs in and travels with the token, so sessions started from a shared token all report the same device and collapse into a single license. That is exactly the multiplexing these rules forbid.

- **Session multiplexing**  
  Sending parallel or concurrent API requests with the same token to perform actions for different users or clients.  
  This effectively uses one licensed session for many users and violates @@name licensing terms.

- **Shared system users**  
  It is acceptable for multiple service applications to use the **same system user**, as long as each establishes its **own token and session**.  
  What is **not** allowed is reusing the same access token or session across multiple running instances or processes.  
  Each concurrent service instance must authenticate separately and maintain its own session.

License violations can lead to denied connections, data integrity issues, or noncompliance with @@name license agreements.

---

## Learn More

- [**Tokens and Sessions Relationship**](token-session-relationship.md)  
  How sessions start, expire, and reconnect.

- [**License Slot Usage**](license-slot.md)  
  How licenses are consumed, released, and reserved.

- [**Session Revocation and Logout**](session-revocation.md)  
  How to explicitly close sessions and release their licenses.
  