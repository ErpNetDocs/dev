# Identity authorities (instance vs global)

When building @@name apps and integrations, it's important to choose the correct **identity authority** (the OAuth 2.0 / OIDC "issuer"). @@name supports two different identity setups, which are used for different goals.

This page explains **which authority to use**, **where the client is registered**, and **what the issued tokens are meant for**.

## Quick decision

| You want to… | Use this authority | Client registration / where you configure access |
|---|---|---|
| Call the APIs of a **specific @@name instance** (Domain API / Table API) | **Instance Identity Service**: `https://{instance}.my.erp.net/id` | Configure a **Trusted Application** inside that @@name instance (incl. scopes/access rules) |
| Enable **SSO / sign-in** to your external application using @@name accounts | **@@name global IdP**: `https://id.erp.net/id` | Client registration is managed **centrally** (see the external IdP page for the process/requirements) |

## Instance Identity Service (per @@name instance)

Use this when your app must access data or operations in a particular @@name instance.

- **Authority (issuer):** `https://{instance}.my.erp.net/id`
- **Audience:** tokens are intended for that same instance's APIs
- **Where the client is registered:** in the **same @@name instance**, as a **Trusted Application**
  - The `client_id` corresponds to the Trusted Application identifier (see the Trusted Applications page for the exact rule used in @@name).
- **Typical use cases:**
  - Background integration/service (sync, automation, ETL)
  - Internal app/portal that needs to call the instance APIs
  - Any scenario where API access must be controlled per instance and per scope

See: [Instance Identity Service](./identity-server.md) and [Trusted Applications](../../../auth/configuration/trusted-apps-access.md).

## @@name global identity provider (external IdP)

Use this when @@name must act as an **external identity provider** for your application (SSO/sign-in), not when your primary goal is "call a specific instance API".

- **Authority (issuer):** `https://id.erp.net/id`
- **Primary purpose:** user authentication / SSO for external applications
- **Where the client is registered:** centrally (not via instance Trusted Applications)
- **Typical use cases:**
  - Your application needs users to sign in with their @@name identity
  - You want OIDC login and user claims issued by @@name as an IdP

See: [ERP.net as an external Identity Provider](./erpnet-as-external-idp.md).

## Common pitfalls (and how to avoid them)

- **Mixing client registrations:**  
  A client registered as a **Trusted Application** is instance-scoped and applies to the **instance Identity Service**. It is not the same as a client registered for the **global IdP**.
- **Using the wrong authority URL:**  
  If you need to call a specific instance API, always start from that instance’s authority (`https://{instance}.my.erp.net/id`), not the global one.
- **Unclear "@@name Identity" wording:**  
  In @@name docs, always interpret "Identity" in context:
  - "Instance Identity Service" -> per-instance API access
  - "External IdP" (`id.erp.net`) -> SSO/sign-in for external apps

## See also

- [OAuth 2.0](../../../auth/concepts/oauth2-overview.md)
- [Instance Identity Service](./identity-server.md)
- [@@name as an external Identity Provider](./erpnet-as-external-idp.md)
- [Trusted Applications and Access Control](../../../auth/configuration/trusted-apps-access.md)
- [Scopes](../../../auth/configuration/scopes.md)
