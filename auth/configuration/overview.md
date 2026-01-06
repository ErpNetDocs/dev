# Overview

This section explains how @@name controls access for external applications through **Trusted Applications** and **Scopes**. These settings define *who* an app is, *what* it can access, and *how* the instance authorizes API requests.

If you're implementing a concrete auth scenario (SPA, web app, service integration), see the tutorial guides in [Getting Started (Authentication)](../getting-started/overview.md).

## What is configured here

### Trusted Applications

A **Trusted Application** is the instance-side registration of an external app. It identifies the client (e.g., `client_id`) and defines the baseline trust and access rules used during authentication and API authorization.

See: [Trusted Applications](./trusted-apps-access.md)

### Scopes

**Scopes** define the permissions your app can request and the instance can grant. Scopes are used to implement least-privilege access by limiting what APIs and operations a token allows.

See: [Scopes](./scopes.md)

## Typical workflow

1. Register (or identify) the app as a **Trusted Application** in the @@name instance.
2. Define or select the required **Scopes** for the integration scenario.
3. Use an appropriate OAuth 2.0/OIDC flow to obtain tokens that include the approved scopes.
4. Call @@name APIs using those tokens.

## Related topics

- [Identity authorities (instance vs global)](../../building-apps/concepts/how-apps-connect/identity-authorities.md)
- [Authentication and Authorization](../overview.md)
- [Tokens](../tokens/tokens-overview.md)
