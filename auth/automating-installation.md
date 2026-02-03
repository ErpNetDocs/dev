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

## Getting Started

### Prerequisites

- You have an @@name instance base URL (for example: `https://mycompany.my.erp.net`).
- You can authenticate to **@@name Instance Manager** with an **administrator** account.
- Your external application has:
  - a stable `applicationUri` identifier
  - an HTTPS `redirectUri` endpoint that can receive an **HTTP POST** with a JSON body and return a successful (2xx) response

> [!NOTE]
> In production, `redirectUri` must be an **absolute HTTPS URL**.

### Endpoints

#### Install endpoint

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
  &requestSecret=true
```

#### Uninstall endpoint

```http
GET {instanceBaseUrl}/manage/apps/uninstall
```

Example:

```http
https://mycompany.my.erp.net/manage/apps/uninstall
  ?applicationUri=MyExternalAppIdentifier
```

### Install URL parameters

| Parameter | Type | Default | Description |
|---|---:|---:|---|
| `applicationName` | string | *(none)* | Name of the application. Used to set the Trusted Application name/display name when the registration is created. |
| `applicationUri` | string | *(none)* | External application identifier used to create its registration in the @@name instance. **Required.** |
| `clientType` | string | `Public` | Client type of the application. Supported values: `Confidential`, `Public`. |
| `redirectUri` | URL | *(none)* | Redirect URL used as the application's sign-in/impersonation callback. @@name Instance Manager also uses this URL to **POST** the lifecycle event payload (JSON) after a successful operation. |
| `impersonateAsInternalUserAllowed` | bool | `false` | For `Public` clients: whether impersonation as an **internal** user is allowed. |
| `impersonateAsCommunityUserAllowed` | bool | `false` | For `Public` clients: whether impersonation as a **community** user is allowed. |
| `requestSecret` | bool | `false` | If `true`, Instance Manager will attempt to issue credentials and include them in the lifecycle event payload. |
| `secretType` | string | `SAT` | Credential kind to issue when `requestSecret=true`. `SAT` = **Service Access Token** (a reference token). `clientCredentials` = a **client secret** that the app uses in the OAuth **Client Credentials** flow to obtain an access token. Supported values: `SAT`, `clientCredentials` (case-insensitive). |
| `accessTokens` | string | `none` | Defines who is allowed to issue **reference access tokens** for this Trusted Application. Supported values: `none`, `authenticatedUsers`, `administratorsOnly`. |

### Uninstall URL parameters

| Parameter | Type | Default | Description |
|---|---:|---:|---|
| `applicationUri` | string | *(none)* | External application identifier to uninstall. **Required.** |

### Validation rules

@@name Instance Manager validates the request before allowing the user to proceed:

**Generic**
- Only @@name instance **administrators** can perform install/uninstall.
- `applicationUri` is required.

**Install-specific**
- If `clientType=Public`:
  - `redirectUri` must be present and valid.
  - At least one impersonation flag must be enabled:
    - `impersonateAsInternalUserAllowed=true` **or**
    - `impersonateAsCommunityUserAllowed=true`
- If `requestSecret=true` for a `Public` client, the request is rejected:
  - Error: `Cannot request credentials for a public client.`
- If `accessTokens` is not specified, it defaults to `none`.

### User flow (interactive approval)

1. Your external application initiates a **browser** request to the install/uninstall endpoint.
2. @@name Instance Manager opens.
   - If the user is not authenticated, Instance Manager requires login.
3. Instance Manager displays a confirmation page with the operation details and an Install / Uninstall action button.
4. When the user clicks the action button:
   - @@name Instance Manager performs the operation in the @@name instance.
   - @@name Instance Manager sends an **HTTP POST** to `redirectUri` with a JSON payload describing the lifecycle event.
   - The callback must return a successful response (2xx). Otherwise, the operation is treated as failed.

### Lifecycle event payload (onboarding callback)

@@name Instance Manager sends the lifecycle event payload to `redirectUri` using HTTP POST with JSON.

**Payload fields**

| Field | Type | Present | Description |
|---|---|---|---|
| `schema` | string | always | Always `erpnet.appLifecycleEvent.v1`. |
| `eventId` | string (GUID) | always | Unique event identifier. |
| `event` | string | always | Event type, e.g. `installed`, `uninstalled`. |
| `occurredAt` | string (UTC timestamp) | always | When the event occurred (UTC). |
| `instanceBaseUrl` | string | always | Instance host/base URL (example: `mycompany.my.erp.net`). |
| `user` | string | always | The approving user (example: `admin`). |
| `secret` | string | only when credentials are issued | Issued credential value (sensitive). |
| `secretType` | string | only when credentials are issued | Credential type (`SAT` or `ClientCredentials`). |

### Examples

#### 1) Confidential without requesting a secret

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
  "occuredAt": "2026-01-21T14:34:09.6573137Z",
  "instanceBaseUrl": "mycompany.my.erp.net",
  "user": "admin"
}
```

---

#### 2) Confidential requesting a secret and specifying SAT

Request:

```http
https://mycompany.my.erp.net/manage/apps/install
  ?applicationUri=MyExternalAppIdentifier
  &redirectUri=https://my-external-app.com/callback/
  &applicationName=My External App
  &clientType=Confidential
  &requestSecret=true
  &secretType=sat
```

Response:

```json
{
  "schema": "erpnet.appLifecycleEvent.v1",
  "eventId": "3857aa99-881c-4798-b888-7ed72d137691",
  "event": "installed",
  "occuredAt": "2026-01-21T14:39:20.6048228Z",
  "instanceBaseUrl": "mycompany.my.erp.net",
  "user": "admin",
  "secret": "enrt_CE17A40CBEBB9F59EECA1EF199F438D64FC42618B1677BE9279E8E4351BA9811",
  "secretType": "SAT"
}
```

---

#### 3) Confidential requesting a secret and specifying ClientCredentials

Request:

```http
https://mycompany.my.erp.net/manage/apps/install
  ?applicationUri=MyExternalAppIdentifier
  &redirectUri=https://my-external-app.com/callback/
  &applicationName=My External App
  &clientType=Confidential
  &requestSecret=true
  &secretType=clientCredentials
```

Response:

```json
{
  "schema": "erpnet.appLifecycleEvent.v1",
  "eventId": "148709ff-c9a2-49dc-96bc-88ee90fda10a",
  "event": "installed",
  "occuredAt": "2026-01-21T14:40:05.024675Z",
  "instanceBaseUrl": "mycompany.my.erp.net",
  "user": "admin",
  "secret": "dTscMD5yK7eMSw3jUKCKGgc1",
  "secretType": "ClientCredentials"
}
```

---

#### 4) Confidential requesting a secret and restricting token issuance to administrators

Request:

```http
https://mycompany.my.erp.net/manage/apps/install
  ?applicationUri=MyExternalAppIdentifier
  &redirectUri=https://my-external-app.com/callback/
  &applicationName=My External App
  &clientType=Confidential
  &requestSecret=true
  &secretType=sat
  &accessTokens=administratorsOnly
```  

Response:

```json
{
  "schema": "erpnet.appLifecycleEvent.v1",
  "eventId": "3857aa99-881c-4798-b888-7ed72d137691",
  "event": "installed",
  "occuredAt": "2026-01-21T14:39:20.6048228Z",
  "instanceBaseUrl": "mycompany.my.erp.net",
  "user": "admin",
  "secret": "enrt_CE17A40CBEBB9F59EECA1EF199F438D64FC42618B1677BE9279E8E4351BA9811",
  "secretType": "SAT"
}
```

---

#### 5) Uninstall

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
  "occuredAt": "2026-01-21T14:42:23.1597831Z",
  "instanceBaseUrl": "mycompany.my.erp.net",
  "user": "admin"
}
```

## Concepts

### Client types

- `Confidential`
  - Intended for server-side applications that can securely store credentials.
  - May request credentials via `requestSecret=true`.

- `Public`
  - Intended for apps that cannot securely store credentials (e.g. browser-based apps).
  - Must provide `redirectUri`.
  - Must enable impersonation for at least one user type:
    - internal and/or community
  - Cannot request credentials (`requestSecret=true` is not allowed).

### Credential types

When `requestSecret=true` (for confidential clients), Instance Manager can issue credentials:

- `SAT`  
  Service Access Token (reference token). In the current implementation, it is issued with a long validity window (example: 10 years).

- `ClientCredentials`  
  Client secret intended for use with the Client Credentials flow.

### Callback delivery (redirectUri)

After the user approves the action, Instance Manager sends the lifecycle event payload to `redirectUri` via **HTTP POST** (JSON, camelCase). The external application must:

- accept the POST request
- validate and persist the onboarding data as appropriate
- return a successful HTTP status code (2xx)

In production environments, `redirectUri` must be an absolute HTTPS URL.

### Secret handling

If the payload contains `secret`, treat it as a password:

- do not log it
- store it in a secret manager / vault
- restrict access (least privilege)

### Access token issuance (`accessTokens`)

The `accessTokens` parameter controls **who is allowed to issue reference access tokens** for the Trusted Application.

Supported values:

- `none`  
  Reference access tokens cannot be issued.  
  *(Default)*

- `authenticatedUsers`  
  Any authenticated (logged-in) user may issue reference access tokens.

- `administratorsOnly`  
  Only administrators may issue reference access tokens.

## Troubleshooting

### Only administrators can perform this action.

Cause:

- The user is not an administrator.

Resolution:

- Sign in with an administrator account in @@name Instance Manager.

### Missing required parameter: applicationUri.

Cause:

- The install/uninstall URL does not include `applicationUri`.

Resolution:

- Provide `applicationUri` in the query string.

### Public clients require a valid redirectUri.

Cause:

- `clientType=Public` but `redirectUri` is missing or invalid.

Resolution:

- Provide a valid `redirectUri` (and ensure it meets the HTTPS requirement in production).

### Public clients must allow impersonation for at least one user type (internal or community).

Cause:

- `clientType=Public` and both impersonation flags are `false`.

Resolution:

- Set at least one of:
  - `impersonateAsInternalUserAllowed=true`
  - `impersonateAsCommunityUserAllowed=true`

### Cannot request credentials for a public client.

Cause:

- `clientType=Public` with `requestSecret=true`.

Resolution:

- Use `clientType=Confidential` if you need credentials issued, or set `requestSecret=false` for public clients.

### Callback fails (non-2xx response)

Cause:

- The external application's `redirectUri` endpoint returns an error response, or cannot be reached.

Resolution:

- Ensure `redirectUri` is reachable from @@name Instance Manager and returns a successful response (2xx).
- Ensure `redirectUri` is absolute HTTPS in production.
