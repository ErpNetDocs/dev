# Concepts overview

This section explains the core ideas behind building apps and integrations for @@name. Use it to understand *how the pieces fit together* before you dive into implementation details.

If you want a step-by-step setup, start with [Getting Started](../getting-started/overview.md). For token flows, scopes, and access rules, see [Authentication and Authorization](../auth/overview.md).

## What you'll learn here

### What an @@name App is

An **@@name App** is any application, integration, or service that connects to an @@name instance to extend, customize, or interact with its business data and functionality. Apps operate outside (or alongside) the instance, communicate through the instance's public APIs, and are recognized by the system through a **Trusted Application**. Authentication and authorization are handled through the built-in @@name Identity.

See: [What is an @@name App](./what-is-erpnet-app.md)

### Application types

@@name supports several **application types**, depending on **where they run**, **who develops them**, and **how they are distributed**. All types follow the same core security/connectivity model (Identity Server + APIs + Trusted Application representation), but differ in deployment and distribution:

- **Internal applications** – part of the @@name platform (e.g., Web Client, Client Center).
- **External applications** – built outside @@name (customer/partner/third-party) and registered per instance.
- **Marketplace applications** – external apps distributed via @@name Marketplace and auto-registered on installation.
- **Embedded (in-client) applications** – external web apps displayed inside the Web Client (with special behavior depending on hosting domain and trust).

See: [Application Types](./app-types.md)

### How apps connect

Apps connect through standard OAuth 2.0 / OIDC mechanisms and must choose the correct identity authority depending on the scenario (instance API access vs external SSO).

See:

- [How apps connect - Overview](./how-apps-connect/overview.md)
- [OAuth 2.0](./how-apps-connect/oauth2-overview.md)
- [Identity authorities (instance vs global)](./how-apps-connect/identity-authorities.md)
- [Instance Identity Server](./how-apps-connect/identity-server.md)
- [@@name as an external Identity Provider](./how-apps-connect/erpnet-as-external-idp.md)

### API access options

@@name provides more than one API surface. The choice affects how you model data, how you query, and what kinds of operations are available.

See: [API Access (Domain API vs Table API)](./api-access/overview.md).

### Calling APIs safely

A secure integration is not only about choosing the right OAuth flow-your app must also handle tokens correctly, protect secrets, and apply least-privilege access.

See: [Best practices for calling APIs securely](./api-best-practices.md).

### Shared terminology

This topic uses consistent terms for apps, identities, tokens, API endpoints, and access control. The glossary is the fastest way to align on wording when troubleshooting or reviewing integration designs.

See: [Glossary](./glossary.md).

## Suggested reading paths

- **I'm new to @@name integrations**
  1. [What is an @@name App](./what-is-erpnet-app.md)
  2. [Application Types](./app-types.md)
  3. [How apps connect - Overview](./how-apps-connect/overview.md)
  4. [API Access (Domain API vs Table API)](./api-access/overview.md)  

- **I need to decide "which identity authority do I use?"**
  1. [Identity authorities (instance vs global)](./how-apps-connect/identity-authorities.md)
  2. [Instance Identity Server](./how-apps-connect/identity-server.md) or [@@name as an external Identity Provider](./how-apps-connect/erpnet-as-external-idp.md)  

- **I'm reviewing an integration for security**
  1. [Best practices for calling APIs securely](./api-best-practices.md)
  2. [Authentication and Authorization](../auth/overview.md)  

## See also

- [Getting Started](../getting-started/overview.md)
- [Authentication and Authorization](../auth/overview.md)