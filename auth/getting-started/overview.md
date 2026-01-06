# Overview

This section contains step-by-step guides for implementing authentication in common @@name app scenarios. Each guide focuses on a concrete application type and shows the recommended identity authority and integration approach.

If you're looking for the underlying concepts (authorities, flows, tokens, scopes), see:

- [Identity authorities (instance vs global)](../../building-apps/concepts/how-apps-connect/identity-authorities.md)
- [Authentication and Authorization](../overview.md)

## Choose your scenario

### Interactive applications (user sign-in)

- [@@name ID for Web Applications](./erpnet-id-web-apps.md)  
  Use @@name as an external Identity Provider for user sign-in in your web application.

- [Instance ID for SPA Applications](./instance-id-spa-apps.md)  
  Authenticate users in a Single Page Application against a specific @@name instance.

- [Instance ID for Web Applications](./instance-id-web-apps.md)  
  Authenticate users in a web application against a specific @@name instance.

- [Instance ID for Web Portals](./instance-id-web-portals.md)  
  Implement authentication for portal-style applications that access a specific @@name instance.

### Non-interactive applications (no user sign-in)

- [Service Applications](./service-apps.md)  
  Authenticate service-to-service integrations (background sync, integrations) against an @@name instance.

- [Automations](./automation.md)  
  Authenticate automated processes that run without direct user interaction and need controlled API access.

## What you'll get from each guide

Each scenario guide provides:

- which identity authority to use
- the recommended flow and token type for the scenario
- required configuration (e.g., trusted application, scopes)
- implementation notes and common pitfalls

## Next steps

- Start with the guide that matches your app type.
- If you are unsure which authority applies, read [Identity authorities (instance vs global)](../../building-apps/concepts/how-apps-connect/identity-authorities.md) first.
