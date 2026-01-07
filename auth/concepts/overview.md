# Overview

This section explains the core concepts behind authentication and authorization in @@name: identity authorities, OAuth 2.0, and how permissions are expressed and enforced.

If you're looking for step-by-step scenario guides (SPA, web apps, service integrations), start with [Quickstarts](../quickstarts/overview.md). If you need instance-side setup, see [Configuration](../configuration/overview.md).

## What you'll find here

### OAuth 2.0

A conceptual overview of OAuth 2.0 in the @@name ecosystem.

See: [OAuth 2.0](./oauth2-overview.md)

### Identity authorities (instance vs global)

How @@name chooses the identity authority depending on application type and scenario.

See: [Identity authorities (instance vs global)](./identity-authorities.md)

### Instance Identity Service (per @@name instance)

Concepts for the per-instance identity service used in Instance ID scenarios.

See: [Instance Identity Service (per @@name instance)](./identity-server.md)

### @@name as an external Identity Provider (global)

Concepts for using @@name as a global external identity provider.

See: [@@name as an external Identity Provider (global)](./erpnet-as-external-idp.md)

### Scopes

How scopes define permissions, how they are requested, and how they affect API access.

See: [Scopes](./scopes.md)

## When to use this section

- You need to understand which identity authority applies to your scenario.
- You want a clear mental model before choosing a flow or configuring a trusted app.
- You're troubleshooting authorization issues (for example, missing or invalid scopes).

## See also

- [Quickstarts](../quickstarts/overview.md)
- [Configuration](../configuration/overview.md)
- [OAuth 2.0 Flows](../flows/overview.md)
- [Tokens](../tokens/tokens-overview.md)
- [Sessions](../sessions/overview.md)