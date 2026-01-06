# Prerequisites

This page lists the minimum prerequisites for building an @@name integration.

## 1) Know your target instance

You will need the base URL of the @@name instance you will connect to, for example:

- `https://{instance}.my.erp.net`

From it, you will derive:

- the **instance Identity Service authority** (see [Instance Identity Service](../concepts/how-apps-connect/identity-server.md))
- the **API endpoints** you will call (see [Choosing the right API](../getting-started/choosing-the-right-api.md))

If you are unsure whether you should use the instance identity service or the global @@name IdP, read:

- [Identity authorities (instance vs global)](../concepts/how-apps-connect/identity-authorities.md)

## 2) Choose your app scenario (so you pick the right auth tutorial)

Pick the scenario that matches your app:

- interactive apps (users sign in): web app, SPA, portal
- non-interactive integrations: service apps, automations

Then follow the matching tutorial in:

- [Getting Started (Authentication)](../../auth/getting-started/overview.md)

## 3) Required access and permissions

To proceed, you typically need:

- the ability to register (or request) a **Trusted Application** in the @@name instance
- the ability to use (or request) the required **Scopes** for your integration

See:

- [Trusted Applications](../../auth/configuration/trusted-apps-access.md)
- [Scopes](../../auth/configuration/scopes.md)

## 4) Tools

Any HTTP client is sufficient to validate the first token + API call:

- curl (CLI)
- Postman (GUI)
- your application code (recommended once the first call is validated)

## 5) What you should not do

- Do not start by guessing endpoints or mixing authorities.
- Do not request broad permissions "just to make it work".
  Prefer least-privilege via the right scopes and the right app type.

See: [Security Best Practices](../../auth/concepts/security-best-practices.md)
