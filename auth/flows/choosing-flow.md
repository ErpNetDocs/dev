# Choosing the Right Flow

Every app that connects to @@name must use one of the supported **OAuth 2.0 authentication flows**.  

Choosing the right one depends on **how your app interacts with users**, **how it runs**, and **what type of access identity it needs** - a signed-in user or a configured System User.

## How to Choose

Start by asking these three questions:

1. Does your app have a **user interface** where people sign in?  
2. Does your app **run unattended** (like a service, integration, or background job)?  
3. Does your app need **both user access and system-level automation**?

Your answers map directly to the correct flow.

| App Type | Recommended Flow | Who Authenticates | Interaction | Typical Scenario |
|-----------|------------------|-------------------|--------------|------------------|
| **Interactive App** | Authorization Code | User | Interactive | Web app, SPA, or mobile client where a user signs in |
| **Service or Background App** | Client Credentials | Application (via its configured **System User**) | Non-interactive | Integration microservice, background sync, scheduled job |
| **Hybrid App** | Authorization Code + Client Credentials | Both user and System User | Mixed | Web or mobile app with backend services performing elevated or automated operations |

> [!NOTE]  
> All flows are handled by @@name **Identity**, which authenticates users, validates app identities, and issues tokens.

## Example Scenarios

### Interactive App - Authorization Code Flow

Your app presents a **Sign In** button.  
A user logs in through the @@name Identity, and your app receives an **access token** to act on behalf of that specific user.

Use this flow when:

- Your app has a web or mobile interface.  
- You need access to data tied to the signed-in user.  
- You want to refresh tokens silently without reauthenticating.  

> [!WARNING]  
> Use **PKCE** for all SPAs and public clients.  
> Never embed a client secret in front-end or mobile code.

### Service or Background App - Client Credentials Flow

Your app runs automatically, without direct user interaction.  
It authenticates directly with the **@@name Identity** using its credentials from the Trusted Application record.

When this flow is used, the instance issues tokens that represent the app's **configured System User**.  
All API calls are executed under that user's permissions.

Use this flow when:

- The app runs as a background process, scheduler, or integration service.  
- You need continuous access without human login.  
- You require controlled access under a known **System User** identity.  

> [!NOTE]  
> The Trusted Application must have `SystemUserAllowed = true` and a valid `SystemUser` assigned.  
> Tokens obtained via this flow always execute as that System User.

> [!WARNING]  
> Use a **least-privilege System User** dedicated to automation.  
> Never assign an administrative user account.

### Hybrid App - Combining Both Flows

Some apps need both: interactive user access and background service capabilities.  

For example, a web portal where users sign in via Authorization Code flow, combined with a backend service that performs scheduled imports or privileged operations via Client Credentials.

Use this pattern when:

- Your front end requires user-specific access.  
- Your backend performs background or elevated tasks.  
- You want both flows under the same logical Trusted Application.

> [!WARNING]  
> Keep **user tokens** and **service tokens** completely separate.  
> Never forward or reuse a user's access token for backend processes.

## Technical Comparison

| Category | Authorization Code Flow | Client Credentials Flow |
|-----------|-------------------------|--------------------------|
| **Who authenticates** | User | Application's configured **System User** |
| **User involvement** | Required (interactive login) | None |
| **Typical clients** | Web apps, SPAs, mobile or desktop clients | Integrations, background services, daemons |
| **Token context** | Represents the signed-in user | Represents the System User assigned in Trusted Application |
| **Refresh tokens** | Supported for confidential clients | Not supported |
| **Session creation** | On first API use | On first API use |
| **Security requirements** | Redirect URIs, PKCE, secure redirect handling | Secure secret storage, restricted System User |
| **When to use** | App acts on behalf of a signed-in user | App acts as its configured System User |
| **Example** | Web portal for employees or partners | Integration or synchronization service |

## Token Behavior and Renewal

| Flow | Token Lifetime | Refresh Tokens | Renewal Method |
|------|----------------|----------------|----------------|
| **Authorization Code** | Short-lived | Yes | Use refresh token for silent renewal |
| **Client Credentials** | Short-lived | No | Request a new token when needed |
| **Hybrid** | Independent per flow | Mixed | Each flow renews tokens separately |

> [!NOTE]  
> All tokens are short-lived by default to reduce exposure risk.  
> Refresh tokens are available only to interactive clients.

### Reference Tokens (API Keys)

@@name also supports **reference access tokens** – manually issued API keys that grant long-lived access to the APIs.  

They are not part of any OAuth 2.0 flow and are created manually by a user or administrator.

- **PAT (Personal Access Token)** – represents a specific user identity.  
- **SAT (Service Access Token)** – represents a service or system user.

Use them only when OAuth flows are not practical (for example, automation scripts or CI/CD pipelines).

See [**Reference Tokens**](../tokens/reference-access-tokens.md) for details on how they are issued, secured, and revoked.

## Security Best Practices

- Always register your app as a **Trusted Application** before connecting.  
- Use **Confidential** clients for server apps; **Public** for SPAs and device apps.  
- Store secrets securely.  
- Rotate **secrets** periodically.  
- Assign a **least-privilege System User** to all background apps.  
- Restrict redirect URIs to exact HTTPS endpoints.  
- Use **PKCE** for all public OAuth clients.  
- Never expose or share access tokens.  

## Summary

- Every app must be **registered as a Trusted Application** in @@name.  
- **@@name Identity** validates the app and issues tokens according to its configuration.  
- **Authorization Code Flow** -> user-driven, interactive access.  
- **Client Credentials Flow** -> system-driven, using the app's **System User**.  
- **Hybrid Flow** -> combines both, with strict separation between contexts.  

Choose the flow that matches how your app operates - and let the @@name Identity handle authentication securely and consistently.

---

## Learn More

- [**Flows Overview**](./overview.md)  
  Understand how the main OAuth 2.0 flows fit into @@name.

- [**Trusted Applications and Access Control**](../configuration/trusted-apps-access.md)  
  Learn how to configure System Users, scopes, and authentication policies.

- [**Interactive Apps (Authorization Code Flow)**](./auth-code/overview.md)  
  Step-by-step guide for apps with user sign-in.

- [**Service and Background Apps (Client Credentials Flow)**](./client-credentials/overview.md)  
  How to authenticate backend or automated services.

- [**Hybrid Apps**](./combined/overview.md)  
  Combine user and backend flows in one secure architecture.
