# How Apps Connect to @@name

@@name integrations are built on a simple contract:

1. **The instance knows your app** (via a Trusted Application registration).
2. **Your app obtains tokens** (via OAuth 2.0 / OIDC from the correct identity authority).
3. **Your app calls APIs with those tokens**, and the APIs enforce the access rules.

This topic explains the moving parts and how they relate, so you can pick the right authority, flow, and access model.

> **Start here if you feel “which Identity URL do I use?” confusion:**  
> [Choose an identity authority (instance vs global)](./identity-authorities.md)

## The three building blocks

### 1) Identity (the token issuer)

An OAuth 2.0 / OIDC **authority** issues tokens. In @@name there are *different* authorities depending on the scenario:

- **Instance Identity Service** (`https://{instance}.my.erp.net/id`) – used for tokens that access a specific instance’s APIs.
- **@@name global IdP** (`https://id.erp.net/id`) – used when @@name acts as an external identity provider for SSO into your application.

See:

- [Choose an identity authority (instance vs global)](./identity-authorities.md)
- [Instance Identity Service](./identity-server.md)
- [@@name as an external Identity Provider](./erpnet-as-external-idp.md)

### 2) OAuth 2.0 / OpenID Connect (OIDC) (the protocol)

OAuth 2.0 defines how access tokens are requested and issued. OpenID Connect extends OAuth 2.0 with user sign-in semantics.

This is the protocol layer—**it does not grant access by itself**. Access is granted only when the token is accepted by the target APIs under the configured rules.

See: [OAuth 2.0 and OpenID Connect (OIDC)](../../../auth/concepts/oauth2-overview.md)

### 3) Trusted Applications (the instance-side trust and access definition)

A **Trusted Application** is the instance-side registration that tells @@name:

- which app is connecting (client identity)
- which authorization behavior is allowed for that app
- which scopes/access rules can be granted for API calls

Trusted Applications are the *control point* for instance API access.

See: [Trusted Applications and Access Control](../../../auth/configuration/trusted-apps-access.md)

## End-to-end flow (what happens when an app connects)

1. **Registration (once)**  
   The app is registered as a Trusted Application in the target instance.

2. **Token acquisition (when needed)**  
   The app makes an OAuth 2.0 / OIDC request to the chosen authority and receives a token.

3. **API access (for each call)**  
   The app calls @@name APIs with `Authorization: Bearer <token>`.  
   The APIs validate the token and enforce:
   - the configured Trusted Application rules
   - the granted scopes
   - any instance security policies

---

## How to proceed (recommended reading order)

1. [Choose an identity authority (instance vs global)](./identity-authorities.md)  
2. [OAuth 2.0 and OpenID Connect (OIDC)](../../../auth/concepts/oauth2-overview.md)  
3. [Trusted Applications and Access Control](../../../auth/configuration/trusted-apps-access.md)  
4. Deep dive by scenario:
   - [Instance Identity Service](./identity-server.md)
   - [@@name as an external Identity Provider](./erpnet-as-external-idp.md)
