# Overview

This section covers the instance-side configuration required to allow external applications to authenticate and access @@name APIs.

If you're implementing a concrete scenario (SPA, web app, service integration), see the step-by-step guides in [Quickstarts](../quickstarts/overview.md).

## What is configured here

### Trusted Applications

A **Trusted Application** is the @@name instance registration of an external application. It identifies the client (by `client_id`) and defines the access rules that apply when the instance issues tokens and authorizes API requests.

See: [Trusted Applications](./trusted-apps-access.md)

## Typical workflow

1. Register the app as a **Trusted Application** in the ERP.net instance.
2. Use the appropriate OAuth 2.0 flow to obtain tokens for that app.
3. Call @@name APIs using the issued tokens.

## Related topics

- [Scopes](../concepts/scopes.md)
- [Identity authorities (instance vs global)](../concepts/identity-authorities.md)
- [Tokens](../tokens/tokens-overview.md)
