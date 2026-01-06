# Get an access token

This page helps you choose the correct identity authority and tutorial to obtain your first working access token.

## 1) Choose the correct authority

Use the decision guide here:

- [Identity authorities (instance vs global)](../concepts/how-apps-connect/identity-authorities.md)

In short:

- If you need to call APIs of a specific @@name instance, you typically use the **instance Identity Server**.
- If you need @@name to act as an **external Identity Provider** for SSO into your app, you use the **global** authority.

## 2) Ensure your Trusted Application is configured

Before you can reliably obtain tokens for instance API access, ensure the app is registered and has the correct access configuration:

- [Create / configure a Trusted Application](../../auth/configuration/trusted-apps-access.md)
- [Scopes](../../auth/configuration/scopes.md)

## 3) Follow the tutorial that matches your scenario

Use the scenario tutorials as the source of truth for the exact steps:

- [@@name ID for Web Applications](../../auth/getting-started/erpnet-id-web-apps.md)
- [Instance ID for SPA Applications](../../auth/getting-started/instance-id-spa-apps.md)
- [Instance ID for Web Applications](../../auth/getting-started/instance-id-web-apps.md)
- [Instance ID for Web Portals](../../auth/getting-started/instance-id-web-portals.md)
- [Service Applications](../../auth/getting-started/service-apps.md)
- [Automations](../../auth/getting-started/automation.md)

## 4) Validate you actually have a usable token

Regardless of scenario, validate:

- you can obtain a token successfully (no auth error)
- the token is accepted by the target API (first call returns HTTP 200)

If your first API call returns 401/403, do not guess.
Check:

- authority (correct issuer)
- trusted app registration
- scopes

See: [Configuration overview](../../auth/configuration/overview.md)
