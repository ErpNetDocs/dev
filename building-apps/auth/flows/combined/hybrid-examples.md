# Example Scenarios

Hybrid applications combine user-facing interaction with system-level automation.  

They are especially useful when the app serves **external @@name users** but also needs to perform **privileged background operations** - all under a single, controlled app identity.

Below are several practical hybrid patterns you can adapt for your integration or portal design.

## Scenario 1: External Partner Portal with Automated Sync

**Use case:**  

An **external partner portal** allows external vendors to review orders, update shipment data, and view invoices.  
Meanwhile, a backend service synchronizes stock data nightly using elevated permissions.

**Frontend flow:**

- **External users** (partners) log in via the Authorization Code flow.  
- Their identity and permissions are validated by the backend, but they do not receive an access token to call @@name directly.

**Backend flow:**

- A **backend service** (using Client Credentials flow) performs all privileged tasks, like syncing stock or processing orders.
- The backend uses a **SystemUser** identity to authenticate and access @@name data.
- **No @@name data is directly exposed to external users** - all access is controlled via the service account.

**Result:**  

External users interact with their data securely through the portal, while the backend service updates the data automatically, without requiring manual intervention.

## Scenario 2: Customer Self-Service Portal (External Authentication)

**Use case:**  

A **self-service portal for external customers** lets users view invoices, service tickets, and orders.  
The frontend authenticates the customer, but the backend manages all data access.

**Frontend flow:**

- **External customers** authenticate via the Authorization Code flow.
- They receive a valid **id token**, but this token **can't** be used to access @@name directly.

**Backend flow:**

- The **backend** uses Client Credentials flow to obtain a **service token**. This token is used for all interactions with @@name APIs.
- The backend is responsible for checking the customer's access rights and fetching the relevant data from @@name.
- **External customers never directly interact with @@name APIs** - all access is managed by the backend.

**Result:**  

External customers authenticate to the system, but the frontend only passes relevant identity context to the backend. The backend makes all the calls to @@name using a service token.

## Scenario 3: Internal Operations Dashboard

**Use case:**  

An **internal dashboard** provides managers with real-time KPIs, while backend services push updates and alerts.

**Frontend flow:**

- Internal users log in through Authorization Code flow with `ImpersonateAsInternalUserAllowed = true`.
- Their roles define access to analytics and reports.

**Backend flow:**

- Uses Client Credentials to poll @@name for new events, update cached reports, or send notifications.  
- Operates with a SystemUser that has limited `read` access only.

**Result:**  

User dashboards show data securely in real time while backend services run independently without session issues.

## Scenario 4: Mobile Field App with Cloud Sync

**Use case:**  

A **mobile app for service technicians** allows users to log visits, attach photos, and sync offline data.

**Frontend flow:**

- Each technician signs in via Authorization Code flow (PKCE).  
- The app works offline and queues updates to send later.

**Backend flow:**

- The **cloud service** authenticates with Client Credentials.  
- Applies business validation and commits technician updates to @@name APIs.

**Result:**  

The app handles users securely without exposing secrets, while all writes to @@name happen through a secure backend channel.

## Scenario 5: Hybrid Integration Gateway

**Use case:**  

A **middleware gateway** connects external systems.  
It provides a simple frontend for admin configuration, plus a backend for high-volume data movement.

**Frontend flow:**

- Administrators log in interactively via Authorization Code flow.
- Manage configuration, mappings, and monitoring UI.

**Backend flow:**

- The **integration engine** runs continuously via Client Credentials flow.
- Pushes and pulls data to and from @@name APIs using the SystemUser.

**Result:**  

Operations run under service credentials, while the UI reflects real-time sync status with user-specific audit trails.

## Best Practices Recap

| Principle | Why it matters |
|------------|----------------|
| Separate user and service tokens | Prevents privilege escalation or token leakage |
| Use a SystemUser for backend work | Keeps API sessions efficient and auditable |
| Do not expose secrets in frontend | Public apps must never hold client secrets |
| Limit scopes | Only request `read` or `update` as needed |
| Enable both impersonation and SystemUser | Required for hybrid trusted apps |
| Centralize token handling | Backend controls renewal and caching for both flows |

---

## Learn More

- [**Hybrid Apps Overview**](overview.md)  
  Understand the concept and advantages of hybrid design.

- [**Typical Architecture**](hybrid-architecture.md)  
  Visualize frontend and backend interactions with Identity Server.

- [**Trusted Applications and Access Control**](../../how-apps-connect/trusted-apps-access.md)  
  Configure impersonation and SystemUser settings for hybrid apps.

- [**Choosing the Right Flow**](../choosing-flow.md)  
  Decide when hybrid architecture is the right fit.
