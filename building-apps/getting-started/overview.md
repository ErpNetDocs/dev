# Overview

This Getting Started guide is a practical path to a first working @@name integration:

1. Prepare your environment and tools.
2. Register your app as a **Trusted Application** in the target @@name instance.
3. Obtain an **access token** from the correct identity authority.
4. Call an @@name API successfully.

If you need the mental model first, see [Concepts](../concepts/overview.md), especially:

- [How apps connect](../concepts/how-apps-connect/overview.md)
- [Identity authorities (instance vs global)](../concepts/how-apps-connect/identity-authorities.md)

For scenario-specific authentication tutorials (SPA, web app, portals, service apps), see:

- [Getting Started (Authentication)](../auth/getting-started/overview.md)

## What you will need

- A target @@name instance to connect to.
- Permission to configure (or request) a **Trusted Application** and **Scopes** in that instance.
- A basic HTTP client tool (e.g., curl/Postman) and/or your preferred programming stack.

## Expected outcome

At the end of this guide, you will have:

- a registered Trusted Application for your integration
- a working token acquisition method for your scenario
- a verified API call (HTTP 200) using `Authorization: Bearer <token>`

## Start here

1. [Prerequisites](./prerequisites.md)
2. [Create a Trusted Application](../auth/configuration/trusted-apps-access.md)
3. [Get an access token](./get-access-token.md)
4. [Make your first API call](./first-api-call.md)
5. [Next steps](./next-steps.md)