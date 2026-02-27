# Automating installation

## Overview

@@name supports a browser-initiated lifecycle flow for external applications via **@@name Instance Manager**.

This flow allows an administrator to:

- **Install** an external application (Instance Manager creates a Trusted Application in the target @@name instance).
- **Uninstall** an external application (Instance Manager removes the app registration from the instance).

After a successful operation, Instance Manager sends an **app lifecycle event** payload (`schema = erpnet.appLifecycleEvent.v1`) to the external application's `redirectUri`, so the external application can complete onboarding.

Related topics:

- [Trusted Applications (configuration)](./configuration/trusted-apps-access.md)
- [Scopes](./concepts/scopes.md)
- [Service Access Tokens](./tokens/reference-access-tokens.md#service-access-tokens-sat)
- [OAuth2 Client Credentials Flow](./flows/client-credentials/overview.md)
- [Security Best Practices](./security-best-practices.md)

---

## Getting Started

### Prerequisites

- You have an @@name instance base URL (for example: `https://mycompany.my.erp.net`).
- You can authenticate to **@@name Instance Manager** with an **administrator** account.
- Your external application has:
  - a stable `applicationUri` identifier
  - an HTTPS `redirectUri` endpoint that can receive an **HTTP POST** with a JSON body and return a successful (2xx) response

> [!NOTE]
> In production, `redirectUri` must be an **absolute HTTPS URL**.

---

## Endpoints

### Install endpoint

```http
GET {instanceBaseUrl}/manage/apps/install
```

Example:

```http
https://mycompany.my.erp.net/manage/apps/install
  ?applicationUri=MyExternalAppIdentifier
  &redirectUri=https://my-external-app.com/callback/
  &applicationName=My External App
  &clientType=Confidential
```

### Uninstall endpoint

```http
GET {instanceBaseUrl}/manage/apps/uninstall
```

Example:

```http
https://mycompany.my.erp.net/manage/apps/uninstall
  ?applicationUri=MyExternalAppIdentifier
```

---

## Install URL parameters

These parameters define a **Trusted Application registration** in the @@name instance.
The registration establishes how the application is identified, how users authenticate through it,
and what access the application may request when tokens are later issued during authentication flows.

| Parameter | Type | Default | Description |
|---|---|---|---|
| `applicationName` | string | *(none)* | Display name of the Trusted Application created during registration. |
| `applicationUri` | string | *(none)* | External identifier for the application. Used as the primary key for its Trusted Application registration. **Required.** |
| `clientType` | string | `Public` | Client type assigned to the Trusted Application. Supported values: `Confidential`, `Public`. |
| `redirectUri` | URL | *(none)* | Callback URL used during authentication flows and for lifecycle event notifications. |
| `impersonate` | string | `none` | Controls which users may authenticate through this Trusted Application. Supported values: `none`, `internal`, `all`. |
| `requestSecret` | bool | `false` | If `true`, credentials are generated for the Trusted Application and delivered in the response payload. |
| `serviceAccess` | string | `none` | Service credential type associated with the Trusted Application. Supported values: `none`, `clientCredentials`, `referenceToken`. |
| `referenceTokens` | string | `none` | Defines which principals may obtain reference access tokens through this Trusted Application. Supported values: `none`, `authenticatedUsers`, `administratorsOnly`. |
| `scope` | string | *(empty)* | Space-delimited list of scopes the Trusted Application is allowed to request during authentication. These scopes determine the capabilities of tokens issued later. Supported values: `openid`, `profile`, `read`, `update`, `offline_access`. |

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
| **ImpersonateAsInternalUserAllowed** | Evaluates the `impersonate` URL parameter. Set to `true` if the value is `internal` or `all`. Set to `false` if the value is `none` or omitted. |
| **ImpersonateAsCommunityUserAllowed** | Evaluates the `impersonate` URL parameter. Set to `true` if the value is `all`. Set to `false` if the value is `internal`, `none`, or omitted. |
| **ApplicationSecretHash** | Evaluates the `requestSecret` URL parameter. If `true`, the system generates a random 24-character secret, computes its SHA-256 hash, and stores the resulting Base64 string. If `false` or omitted, no secret is generated or stored. Additionally, if `false`, no secret or reference token is returned in the response payload, regardless of the `serviceAccess` parameter value. |
| **SystemUserAllowed** | Evaluates the `serviceAccess` URL parameter. Set to `true` if the value is anything other than `none`. Set to `false` if the value is exactly `none` or omitted. |
| **SystemUser** | Evaluates the `serviceAccess` URL parameter. Pre-populates with `SYSTEM_APPLICATION_USER` if the value is anything other than `none`. Remains unassigned (or null) if the value is exactly `none` or omitted. |
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
   - @@name Instance Manager sends an **HTTP POST** to `redirectUri` with a JSON payload describing the lifecycle event.
   - The callback must return a successful response (2xx). Otherwise, the operation is treated as failed.

---

## Lifecycle event payload (onboarding callback)

@@name Instance Manager sends the lifecycle event payload to `redirectUri` using HTTP POST with JSON (camelCase).

### Payload fields

| Field | Type | Present | Description |
|---|---|---|---|
| `schema` | string | always | Always `erpnet.appLifecycleEvent.v1`. |
| `eventId` | string (GUID) | always | Unique event identifier. |
| `event` | string | always | Event type, e.g. `installed`, `uninstalled`. |
| `occurredAt` | string (UTC timestamp) | always | When the event occurred (UTC). |
| `instanceBaseUrl` | string | always | Instance base URL (example: `https://mycompany.my.erp.net`). |
| `user` | string | always | The approving user (example: `admin`). |
| `clientSecret` | string | when `requestSecret=true` | Issued client secret (sensitive). |
| `referenceToken` | string | when `serviceAccess=referenceToken` | Issued service access token (reference token, sensitive). |
| `request` | object | always | Echo of the original install request parameters. |

> [!NOTE]
> If the `requestSecret` install parameter is omitted or set to `false`, the `clientSecret` and `referenceToken` will not be included in the response payload, regardless of the `serviceAccess` value.

### `request` object fields

The `request` object echoes the original install request (normalized and serialized in camelCase), including `requestSecret=true` when credentials are successfully issued.

| Field | Type | Description |
|---|---|---|
| `applicationName` | string | Application display name. |
| `applicationUri` | string | External application identifier. |
| `clientType` | string | Client type (`confidential` or `public`). |
| `redirectUri` | string | Redirect/callback URL provided during install. |
| `impersonate` | string | Impersonation scope (`none`, `internal`, `all`). |
| `requestSecret` | bool | Whether credentials were requested. |
| `serviceAccess` | string | Service access mode (`none`, `clientCredentials`, `referenceToken`). |
| `referenceTokens` | string | Who can issue reference access tokens (`none`, `authenticatedUsers`, `administratorsOnly`). |
| `scope` | string | Space-delimited scope string. |

---

## Examples

### 1) Confidential requesting credentials

Request:

```http
https://mycompany.my.erp.net/manage/apps/install
  ?applicationUri=MyExternalAppIdentifier
  &redirectUri=https://my-external-app.com/callback/
  &applicationName=My External App
  &impersonate=internal
  &requestSecret=true
  &serviceAccess=clientCredentials
  &scope=read%20update
```

Response:

```json
{
  "schema": "erpnet.appLifecycleEvent.v1",
  "eventId": "0799261b-6a1a-4a7a-afc6-6a9b7fcb8a8c",
  "event": "installed",
  "occurredAt": "2026-01-21T14:34:09.6573137Z",
  "instanceBaseUrl": "https://mycompany.my.erp.net",
  "user": "admin",
  "clientSecret": "dTscMD5yK7eMSw3jUKCKGgc1",
  "request": {
    "applicationName": "My External App",
    "applicationUri": "MyExternalAppIdentifier",
    "clientType": "confidential",
    "redirectUri": "https://my-external-app.com/callback/",
    "impersonate": "internal",
    "requestSecret": true,
    "serviceAccess": "clientCredentials",
    "referenceTokens": "none",
    "scope": "read update"
  }
}
```

---

### 2) Confidential requesting a client secret and a reference token

Request:

```http
https://mycompany.my.erp.net/manage/apps/install
  ?applicationUri=MyExternalAppIdentifier
  &redirectUri=https://my-external-app.com/callback/
  &applicationName=My External App
  &clientType=Confidential
  &requestSecret=true
  &serviceAccess=referenceToken
  &scope=read
```

Response:

```json
{
  "schema": "erpnet.appLifecycleEvent.v1",
  "eventId": "3857aa99-881c-4798-b888-7ed72d137691",
  "event": "installed",
  "occurredAt": "2026-01-21T14:39:20.6048228Z",
  "instanceBaseUrl": "https://mycompany.my.erp.net",
  "user": "admin",
  "clientSecret": "XXXXXXXXXXXXXXXXXXXXXXXX",
  "referenceToken": "enrt_XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX",
  "request": {
    "applicationName": "My External App",
    "applicationUri": "MyExternalAppIdentifier",
    "clientType": "confidential",
    "redirectUri": "https://my-external-app.com/callback/",
    "impersonate": "none",
    "requestSecret": true,
    "serviceAccess": "referenceToken",
    "referenceTokens": "none",
    "scope": "read"
  }
}
```

---

### 3) Public with interactive sign-in = all

Request:

```http
https://mycompany.my.erp.net/manage/apps/install
  ?applicationUri=MyExternalAppIdentifier
  &redirectUri=https://my-external-app.com/callback/
  &applicationName=My External App
  &clientType=Public
  &impersonate=all
  &requestSecret=false
  &scope=openid%20profile
```

Response:

```json
{
  "schema": "erpnet.appLifecycleEvent.v1",
  "eventId": "0799261b-6a1a-4a7a-afc6-6a9b7fcb8a8c",
  "event": "installed",
  "occurredAt": "2026-01-21T14:34:09.6573137Z",
  "instanceBaseUrl": "https://mycompany.my.erp.net",
  "user": "admin",
  "request": {
    "applicationName": "My External App",
    "applicationUri": "MyExternalAppIdentifier",
    "clientType": "public",
    "redirectUri": "https://my-external-app.com/callback/",
    "impersonate": "all",
    "requestSecret": false,
    "serviceAccess": "none",
    "referenceTokens": "none",
    "scope": "openid profile"
  }
}
```

---

### 4) Uninstall

Request:

```http
https://mycompany.my.erp.net/manage/apps/uninstall
  ?applicationUri=MyExternalAppIdentifier
```

Response:

```json
{
  "schema": "erpnet.appLifecycleEvent.v1",
  "eventId": "0e3bf686-4abc-4230-9ca0-47c4efa12b09",
  "event": "uninstalled",
  "occurredAt": "2026-01-21T14:42:23.1597831Z",
  "instanceBaseUrl": "https://mycompany.my.erp.net",
  "user": "admin"
}
```

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

Cause:
- The user is not an administrator.

Resolution:
- Sign in with an administrator account in @@name Instance Manager.

### Missing required parameter: applicationUri

Cause:
- The install or uninstall URL does not include `applicationUri`.

Resolution:
- Provide `applicationUri` in the query string.

### Public clients require a valid redirectUri

Cause:
- `clientType=Public` but `redirectUri` is missing or invalid.

Resolution:
- Provide a valid HTTPS `redirectUri`.

### Public clients cannot request credentials

Cause:
- `clientType=Public` with `requestSecret=true`.

Resolution:
- Use `clientType=Confidential` or set `requestSecret=false`.

### Callback fails (non-2xx response)

Cause:
- The external application's `redirectUri` endpoint returns an error or is unreachable.

Resolution:
- Ensure the endpoint is reachable and returns a successful (2xx) response.
- Ensure `redirectUri` is an absolute HTTPS URL in production.
