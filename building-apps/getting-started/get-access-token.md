# Get an Access Token

In this step, you obtain a **working access token** that your app can use to call @@name APIs.

Access tokens for @@name APIs are issued **only by the identity service of a specific @@name instance**.  
The purpose of this step is to follow the correct authentication flow for your scenario and verify that token issuance works end to end.

## Use the Correct Identity Authority

When calling @@name APIs, your app must authenticate against the **instance identity service**.

- The **instance identity service** issues access tokens that are accepted by the instance APIs.
- The **global @@name authority** is used only for authentication scenarios where @@name acts as an external identity provider.
  It does **not** issue access tokens for @@name APIs.

If you are unsure which authority applies to your scenario, see:  
[Identity Authorities (Instance vs Global)](../../auth/concepts/identity-authorities.md).

## Verify Trusted Application Configuration

Before requesting a token, ensure that the app is correctly registered in the target instance:

- A **Trusted Application** exists and is enabled
- The correct access modes are enabled (interactive and/or service)
- The required **scopes** are allowed

If this configuration is incorrect, token requests will fail regardless of the authentication flow.

Reference:

- [Trusted Applications](../../auth/configuration/trusted-apps-access.md)
- [Scopes](../../auth/concepts/scopes.md)

## Follow the Tutorial for Your Scenario

The exact steps for obtaining a token depend on your application type and authentication model.

Use the tutorial that matches your scenario:

- [Web Applications](../../auth/quickstarts/erpnet-id-web-apps.md)
- [Single Page Applications (SPA)](../../auth/quickstarts/instance-id-spa-apps.md)
- [Web Portals](../../auth/quickstarts/instance-id-web-portals.md)
- [Service Applications](../../auth/quickstarts/service-apps.md)
- [Automations](../../auth/quickstarts/automation.md)

These tutorials are the **source of truth** for request parameters, flows, and examples.

---

## Validate the Token

Before continuing, confirm that:

- A token is issued successfully (no authentication error)
- The token issuer matches the **instance identity service**
- The token is accepted by the target API
- Your first API request returns **HTTP 200**

If the API responds with **401** or **403**, verify:

- The identity authority (issuer) is correct
- The Trusted Application configuration matches your scenario
- The required scopes are granted

## Next Step

Once you have a valid access token, continue with  
[Make Your First API Call](./first-api-call.md).
