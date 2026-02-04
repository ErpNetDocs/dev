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

| Parameter | Type | Default | Description |
|---|---|---|---|
| `applicationName` | string | *(none)* | Name of the application. Used to set the Trusted Application name/display name when the registration is created. |
| `applicationUri` | string | *(none)* | External application identifier used to create its registration in the @@name instance. **Required.** |
| `clientType` | string | `Public` | Client type of the application. Supported values: `Confidential`, `Public`. |
| `redirectUri` | URL | *(none)* | Redirect URL used as the application's sign-in/impersonation callback and as the lifecycle event callback endpoint. |
| `impersonate` | string | `none` | Allows interactive sign-in for no users, internal users only, or all users (internal + community). Supported values: `none`, `internal`, `all`. |
| `requestSecret` | bool | `false` | If `true`, Instance Manager will issue credentials and include them in the lifecycle event payload. |
| `serviceAccess` | string | `none` | Credential type to issue when `requestSecret=true`. Supported values: `none`, `clientCredentials`, `referenceToken`. |
| `accessTokens` | string | `none` | Defines who is allowed to issue reference access tokens. Supported values: `none`, `authenticatedUsers`, `administratorsOnly`. |
| `scope` | string | *(empty)* | Space-delimited list of scopes associated with issued tokens. |

---

## Uninstall URL parameters

| Parameter | Type | Default | Description |
|---|---|---|---|
| `applicationUri` | string | *(none)* | External application identifier to uninstall. **Required.** |

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
- If `accessTokens` is not specified, it defaults to `none`.

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
| `clientSecret` | string | when `serviceAccess=clientCredentials` | Issued client secret (sensitive). |
| `referenceToken` | string | when `serviceAccess=referenceToken` | Issued service access token (reference token, sensitive). |
| `request` | object | always | Echo of the original install request parameters. |

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
| `accessTokens` | string | Who can issue reference access tokens (`none`, `authenticatedUsers`, `administratorsOnly`). |
| `scope` | string | Space-delimited scope string.

---

## Examples

### 1) Confidential without requesting credentials

Request:

```http
https://mycompany.my.erp.net/manage/apps/install
  ?applicationUri=MyExternalAppIdentifier
  &redirectUri=https://my-external-app.com/callback/
  &applicationName=My External App
  &clientType=Confidential
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
    "clientType": "confidential",
    "redirectUri": "https://my-external-app.com/callback/",
    "impersonate": "none",
    "requestSecret": false,
    "serviceAccess": "none",
    "accessTokens": "none",
    "scope": ""
  }
}
```

---

### 2) Confidential requesting a reference token

Request:

```http
https://mycompany.my.erp.net/manage/apps/install
  ?applicationUri=MyExternalAppIdentifier
  &redirectUri=https://my-external-app.com/callback/
  &applicationName=My External App
  &clientType=Confidential
  &requestSecret=true
  &serviceAccess=referenceToken
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
  "referenceToken": "enrt_XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX",
  "request": {
    "applicationName": "My External App",
    "applicationUri": "MyExternalAppIdentifier",
    "clientType": "confidential",
    "redirectUri": "https://my-external-app.com/callback/",
    "impersonate": "none",
    "requestSecret": true,
    "serviceAccess": "referenceToken",
    "accessTokens": "none",
    "scope": ""
  }
}
```

---

### 3) Confidential requesting client credentials

Request:

```http
https://mycompany.my.erp.net/manage/apps/install
  ?applicationUri=MyExternalAppIdentifier
  &redirectUri=https://my-external-app.com/callback/
  &applicationName=My External App
  &clientType=Confidential
  &requestSecret=true
  &serviceAccess=clientCredentials
```

Response:

```json
{
  "schema": "erpnet.appLifecycleEvent.v1",
  "eventId": "148709ff-c9a2-49dc-96bc-88ee90fda10a",
  "event": "installed",
  "occurredAt": "2026-01-21T14:40:05.024675Z",
  "instanceBaseUrl": "https://mycompany.my.erp.net",
  "user": "admin",
  "clientSecret": "XXXXXXXXXXXXXXXXXXXXXXXX",
  "request": {
    "applicationName": "My External App",
    "applicationUri": "MyExternalAppIdentifier",
    "clientType": "confidential",
    "redirectUri": "https://my-external-app.com/callback/",
    "impersonate": "none",
    "requestSecret": true,
    "serviceAccess": "clientCredentials",
    "accessTokens": "none",
    "scope": ""
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

## Access token issuance (`accessTokens`)

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
