# Application Types

Application types describe **how an application connects to an @@name instance**, not what business problem it solves.

They help you decide:

- whether users are involved at all
- whether API access happens **on behalf of a user**, **on behalf of the application**, or via a **reference token**
- which **identity authority** is involved
- which **authentication guide** applies

> [!IMPORTANT]
> @@name instance APIs are always accessed using tokens issued by the **instance identity service**  
> (`https://<instance>.my.erp.net/id`).
>
> The global @@name Identity (`id.erp.net`) is used **only for user authentication (SSO)** and does **not**
> issue access tokens for instance APIs.

## Summary

| Application type | Typical scenario | Who signs in | How API access is performed | Auth guide | Typical API |
| --- | --- | --- | --- | --- | --- |
| **SPA Applications** | Browser-only app, no backend | Internal users | On behalf of the signed-in user | [Instance ID for SPA Applications](../../auth/getting-started/instance-id-spa-apps.md) | Domain API |
| **Web (Confidential) Applications** | Web app with backend | Internal users | On behalf of the signed-in user | [Instance ID for Web Applications](../../auth/getting-started/instance-id-web-apps.md) | Domain API |
| **Web Portals** | Customer / partner portals | Internal and/or external users | On behalf of the application (service identity) | [Instance ID for Web Portals](../../auth/getting-started/instance-id-web-portals.md) | Domain API |
| **Service Applications** | Background integrations, sync, daemons | None | On behalf of the application (service identity) | [Service Applications](../../auth/getting-started/service-apps.md) | Domain API |
| **Automation** | Scripts, scheduled jobs, ops tooling | None | On behalf of a reference token | [Automations](../../auth/getting-started/automation.md) | Domain or Table API |

> [!NOTE]
> Business Intelligence, reporting, and backup are **usage patterns**, not standalone application types.
> They are implemented using **Service Applications** or **Automation**, depending on operational needs.

## Application Type Details

### SPA Applications

Use this type when your application:

- runs entirely in the browser
- has no backend component
- cannot keep a client secret

Characteristics:

- users sign in interactively
- access tokens represent the **signed-in user**
- Authorization Code flow with PKCE is mandatory

See:  
[Instance ID for SPA Applications](../../auth/getting-started/instance-id-spa-apps.md)

---

### Web (Confidential) Applications

Use this type when your application:

- has a backend capable of storing secrets
- serves internal users
- performs API calls as the signed-in user

Characteristics:

- users sign in interactively
- access tokens are issued **on behalf of the user**
- Authorization Code flow is used

See:  
[Instance ID for Web Applications](../../auth/getting-started/instance-id-web-apps.md)

---

### Web Portals

Use this type when your application:

- allows users to sign in for identification or session context
- supports internal and/or external users
- performs all API access from the backend

Characteristics:

- users authenticate for **identity only**
- external users never receive API access
- API calls are performed **on behalf of the application**
- combines Authorization Code (for users) and Client Credentials (for APIs)

See:  
[Instance ID for Web Portals](../../auth/getting-started/instance-id-web-portals.md)

---

### Service Applications

Use this type when your integration:

- runs without user interaction
- performs background processing or system integration
- needs controlled, scoped API access

Characteristics:

- no user sign-in
- Client Credentials flow
- access tokens represent a **service identity**

See:  
[Service Applications](../../auth/getting-started/service-apps.md)

---

### Automation

Use this type when your integration:

- is script-based or operational
- does not implement OAuth flows
- requires long-lived access

Characteristics:

- no user interaction
- uses manually issued **reference access tokens**
- suited for jobs, tooling, and controlled automation

See:  
[Automations](../../auth/getting-started/automation.md)

---

## BI, Reporting, and Backup Scenarios

Reporting, analytics, and backup workloads are implemented using:

- **Service Applications** for OAuth-based access, or
- **Automation** for long-running or scheduled tasks

In these cases:

- prefer the **Table API** for high-volume, read-only access
- restrict scopes to `read` only

See:

- [Table API Overview](../../table-api/index.md)
- [Power BI Usage Guidelines](../../table-api/usage-guide.md)

---

## Related Topics

- [Choosing the Right API](../getting-started/choose-right-api.md)
- [Trusted Applications](../../auth/configuration/trusted-apps-access.md)
- [Scopes](../../auth/concepts/scopes.md)
