# License Compliance and Violations

@@name uses a strict **one session = one license** model.  

Each active session consumes a concurrent license slot, and every session must represent a single user or service identity.  

Improper token or session usage can lead to **license violations**.

## Common Violations

- **Token sharing**  
  Using the same access token for multiple users, machines, or processes.  
  Access tokens are tied to a single identity and session context - sharing them spreads one license across multiple users.

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

- [**Trusted Applications and Access Control**](../how-apps-connect/trusted-apps-access.md)  
  Learn how system users and app modes determine license usage.
