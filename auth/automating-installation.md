# Automating installation

## Overview

@@name supports a browser-initiated lifecycle flow for external applications via **@@name Instance Manager**.

This flow allows an administrator to quickly configure your application by opening a specially crafted installation link in their web browser.

Using these links, an administrator can:

- **Install** an external application (Instance Manager automatically creates a Trusted Application registration in the target @@name instance based on your predefined parameters).
- **Uninstall** an external application (Instance Manager removes the app registration from the instance).

> [!NOTE]
> **Onboarding Callbacks**
> If your application is installed via the **@@name Marketplace**, a secure, cryptographically signed **app lifecycle event** payload (`schema = erpnet.appLifecycleEvent.v2`) is sent to your `redirectUri` to complete onboarding.
>
> Handling this callback is optional.
> However, if you need to acknowledge deployment or securely receive dynamically generated credentials (like a `clientSecret`), you should implement a webhook listener.
> For details on handling the secure payload, see [Developing for @@name Marketplace](../marketplace/index.md).

Related topics:

- [Developing for @@name Marketplace](../marketplace/index.md)
- [Trusted Applications (configuration)](./configuration/trusted-apps-access.md)
- [Scopes](./concepts/scopes.md)
- [Service Access Tokens](./tokens/reference-access-tokens.md#service-access-tokens-sat)
- [OAuth2 Client Credentials Flow](./flows/client-credentials/overview.md)
- [Security Best Practices](./security-best-practices.md)

---

## Getting Started

### Prerequisites

To construct an installation link and complete the flow, you need the following:

- An @@name instance base URL (for example: `https://mycompany.my.erp.net`).
- An @@name **administrator** account to authenticate to **@@name Instance Manager**.
- Your external application must have a stable `applicationUri` (this acts as your unique identifier).
- If your app requires user login (Public client) or supports Marketplace onboarding, you must provide an absolute HTTPS `redirectUri`.

---

## Constructing Installation Links

To initiate an installation or uninstallation, your application must construct a specific URL and redirect the user's browser to it.

If the user is not currently authenticated, @@name Instance Manager will automatically prompt them to log in before proceeding.

### Install link format

The install link is a standard web URL pointing to the `/manage/apps/install` path on the target instance, appended with your configuration parameters.

**Format:**
`{instanceBaseUrl}/manage/apps/install?args...`

**Example:**

```http
https://mycompany.my.erp.net/manage/apps/install
  ?applicationUri=MyExternalAppIdentifier
  &redirectUri=https://my-external-app.com/callback/
  &applicationName=My%20External%20App
  &clientType=Confidential
```  

### Uninstall link format

The uninstall link targets the `/manage/apps/uninstall` path and only requires your application identifier.

**Format:**
`{instanceBaseUrl}/manage/apps/uninstall?applicationUri={yourAppId}`

**Example:**

```http
https://mycompany.my.erp.net/manage/apps/uninstall
  ?applicationUri=MyExternalAppIdentifier
```

---

## Install URL parameters

These parameters define a **Trusted Application registration** in the @@name instance.

The registration establishes how the application is identified, how users authenticate through it, and what access the application may request when tokens are later issued during authentication flows.

| Parameter | Type | Default | Description |
|---|---|---|---|
| `applicationName` | string | *(none)* | Display name of the Trusted Application created during registration. |
| `applicationUri` | string | *(none)* | External identifier for the application. Used as the primary key for its Trusted Application registration. **Required.** |
| `clientType` | string | `None` | Client type assigned to the Trusted Application. Supported values: `Confidential`, `Public`, `None`. |
| `redirectUri` | URL | *(none)* | Callback URL used during user authentication flows and for receiving lifecycle event notifications (via Marketplace). |
| `impersonate` | string | `none` | Controls which users may authenticate through this Trusted Application. Supported values: `none`, `internal`, `all`. |
| `requestSecret` | bool | `false` | If `true`, credentials are generated and delivered in the onboarding callback payload. *(Note: Only sent if initiated via Marketplace).* |
| `serviceAccess` | string | `none` | Service credential type associated with the Trusted Application. Supported values: `none`, `clientCredentials`, `referenceToken`. |
| `referenceTokens` | string | `none` | Defines which principals may obtain reference access tokens. Supported values: `none`, `authenticatedUsers`, `administratorsOnly`. |
| `scope` | string | *(empty)* | Space-delimited list of scopes the Trusted Application is allowed to request during authentication. Supported values: `openid`, `profile`, `read`, `update`, `offline_access`. |

### Scope Descriptions

Scopes define the maximum permissions this Trusted Application may request during authentication.
They do not issue tokens themselves, but constrain the contents and capabilities of tokens issued later.

| Scope | Description |
|------|-------------|
| `openid` | Allows the application to perform OpenID Connect authentication and receive a stable user identifier. |
| `profile` | Allows access to basic user profile attributes during authentication. |
| `read` | Allows tokens issued via this application to read protected resources. |
| `update` | Allows tokens issued via this application to modify protected resources. |
| `offline_access` | Allows the application to request refresh tokens for continued access without user interaction. |

---

## Uninstall URL parameters

| Parameter | Type | Default | Description |
|---|---|---|---|
| `applicationUri` | string | *(none)* | External application identifier to uninstall. **Required.** |

## Trusted Application setup algorithm

During the installation process, the system reads incoming URL parameters to automatically configure the new Trusted Application's core attributes. The following table details the mapping logic between these external installation parameters and the resulting internal system properties.

| Trusted App Attribute | Mapping Logic & Effect |
|---|---|
| **Name** | Reads the `applicationName` URL parameter to set the application's display name. Defaults to `"(unnamed)"` if left blank. |
| **ApplicationUri** | Uses the `applicationUri` URL parameter as the unique identifier (`client_id`) for the trusted app. |
| **ClientType** | Maps the `clientType` URL parameter to set the client as either `Public` or `Confidential`. |
| **ImpersonateLoginUrl** | Uses the `redirectUri` URL parameter to define the allowed login redirect URI during authentication. |
| **ImpersonateAsInternalUserAllowed** | Evaluates the `impersonate` URL parameter. Set to `true` if the value is `internal` or `all`. |
| **ImpersonateAsCommunityUserAllowed** | Evaluates the `impersonate` URL parameter. Set to `true` if the value is `all`. |
| **ApplicationSecretHash** | Evaluates the `requestSecret` URL parameter. If `true`, the system generates a random 24-character secret and stores its SHA-256 hash. Note that secrets are only delivered in the onboarding callback if the flow is initiated via the Marketplace. |
| **SystemUserAllowed** | Evaluates the `serviceAccess` URL parameter. Set to `true` if the value is anything other than `none`. |
| **SystemUser** | Pre-populates with `SYSTEM_APPLICATION_USER` if `serviceAccess` is anything other than `none`. |
| **ReferenceTokens** | Maps the `referenceTokens` URL parameter to internal enum values: `None`, `AuthenticatedUsers`, or `AdministratorsOnly`. |
| **Scope** | Stores the space-delimited list of allowed scopes provided by the `scope` URL parameter. |

---

## Validation rules

@@name Instance Manager validates the request before allowing the user to proceed.

### Generic

- Only @@name instance **administrators** can perform install or uninstall actions.
- `applicationUri` is required.

### Install-specific

- If `clientType=Public`:
  - `redirectUri` must be present and valid.
  - `impersonate` must not be `none` (must be `internal` or `all`).
- If `clientType=Public`, `requestSecret=true` is not allowed.
- If `referenceTokens` is not specified, it defaults to `none`.

---

## User flow (interactive approval)

1. Your external application initiates a **browser** request to the install or uninstall endpoint.
2. @@name Instance Manager opens.
   - If the user is not authenticated, Instance Manager requires login.
3. Instance Manager displays a confirmation page with the operation details and an Install / Uninstall action button.
4. When the user clicks the action button:
   - @@name Instance Manager performs the operation in the @@name instance.
   - **If the installation was initiated directly:** The process ends here. No HTTP POST or webhook is triggered.
   - **If the installation was initiated via the @@name Marketplace:** An **HTTP POST** is sent to your `redirectUri` containing the lifecycle event payload. Your application can optionally handle this onboarding callback. For more information on this payload and how to process it, read [Developing for @@name Marketplace](../marketplace/index.md).

---

## Concepts

### Client types

- `Confidential`
  - Intended for server-side applications that can securely store credentials.
  - May request credentials via `requestSecret=true`.

- `Public`
  - Intended for apps that cannot securely store credentials.
  - Must provide `redirectUri`.
  - Must enable impersonation for at least one user type.
  - Cannot request credentials.

- `None`
  - Intended for standalone applications that do not require authentication with the @@name instance.
  - Serves primarily as a registration "note" or placeholder indicating the app is installed.  

### Service access types

- `referenceToken`  
  Issues a service access token (reference token).

- `clientCredentials`  
  Issues a client secret intended for use with the OAuth 2.0 Client Credentials flow.

---

## Secret handling

If the payload contains `clientSecret` or `referenceToken`, treat it as a password:

- do not log it
- store it in a secret manager or vault
- restrict access (least privilege)

---

## Reference access token issuance (`referenceTokens`)

Controls who may issue reference access tokens.

Supported values:

- `none`
- `authenticatedUsers`
- `administratorsOnly`

---

## Troubleshooting

### Only administrators can perform this action

**Cause**:
- The user is not an administrator.

**Resolution**:
- Sign in with an administrator account in @@name Instance Manager.

### Missing required parameter: applicationUri

**Cause**:
- The install or uninstall URL does not include `applicationUri`.

**Resolution**:
- Provide `applicationUri` in the query string.

### Public clients require a valid redirectUri

**Cause**:
- `clientType=Public` but `redirectUri` is missing or invalid.

**Resolution**:
- Provide a valid HTTPS `redirectUri`.

### Public clients cannot request credentials

**Cause**:
- `clientType=Public` with `requestSecret=true`.

**Resolution**:
- Use `clientType=Confidential` or set `requestSecret=false`.

### Callback fails (non-2xx response)

**Cause**:
- The external application's `redirectUri` endpoint returns an error or is unreachable.

**Resolution**:
- Ensure the endpoint is reachable and returns a successful (2xx) response.
- Ensure `redirectUri` is an absolute HTTPS URL in production.
