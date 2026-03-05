---
title: Developing for ERP.net Marketplace
description: Guidance for building and integrating applications with ERP.net Marketplace.
---

# Developing for Marketplace

## Overview

The **@@name Marketplace** acts as a centralized hub for administrators to discover and install external applications into their @@name instances.  

When a user installs or uninstalls your application via the Marketplace, the underlying setup is handled by the @@name Instance Manager. However, unlike direct manual installations, the Marketplace flow acts as a broker and automatically triggers a secure webhook to your application.  

Upon a successful installation or uninstallation, the Marketplace sends an **app lifecycle event** payload (`schema = erpnet.appLifecycleEvent.v2`) to your application's `redirectUri`. This payload is delivered as a cryptographically signed envelope, allowing your application to securely complete onboarding, acknowledge the deployment, and receive dynamically generated credentials (such as a `clientSecret` or `referenceToken`).

> [!NOTE]
> Processing this onboarding callback is completely **optional**.
> You only need to implement a webhook listener if your application requires automated deployment acknowledgement or needs to securely receive generated credentials.

Related topics:

- [Automating installation](../auth/automating-installation.md)
- [Trusted Applications (configuration)](../auth/configuration/trusted-apps-access.md)

---

## Getting Started

### Prerequisites

To list your application in the @@name Marketplace, you need:

- A registered developer account and application entry in the @@name Marketplace portal.

If you choose to handle the optional **lifecycle event callbacks**, you will also need:

- An **HTTPS endpoint** (`redirectUri`) capable of receiving an **HTTP POST** request with a JSON body and returning a successful (2xx) response.
- The ability to verify cryptographic signatures (e.g., using RSA public keys) to validate the incoming webhook payloads.  

## The Marketplace Flow

When a user initiates an installation or uninstallation from the @@name Marketplace, the following sequence occurs:

1. The Marketplace opens a popup window pointing to the @@name Instance Manager so the user can authorize the operation.
2. The @@name Instance Manager performs the requested operation (creating or removing the Trusted Application registration) within the target @@name instance.
3. Once the operation is complete, the Instance Manager securely reports the result back to the Marketplace.
4. **(Optional)** If you have configured a webhook listener, the Marketplace sends an **HTTP POST** to your application's `redirectUri` containing a secure, cryptographically signed JSON payload describing the lifecycle event.
5. Your endpoint must validate the signature, process the payload, and return a successful HTTP status code (2xx) to acknowledge receipt.

---

## Lifecycle Event Payload (v2)

To ensure that the lifecycle event genuinely originated from the @@name Marketplace and has not been tampered with, the callback uses a secure `v2` payload envelope.

The Marketplace sends this payload to your `redirectUri` using HTTP POST with JSON (camelCase).

### The Envelope

The outer JSON structure acts as an envelope containing the cryptographic signature and the encoded event data.

| Field | Type | Description |
|---|---|---|
| `schema` | string | Always `erpnet.appLifecycleEvent.v2`. |
| `signature` | string | The cryptographic signature (e.g., RSA base64) used to verify the payload's authenticity. |
| `payload` | string | The base64-encoded or stringified JSON object containing the actual lifecycle event details. |

### The Inner Event Payload

Once you verify the signature and decode the `payload` field, you will find the actual lifecycle event details. 

| Field | Type | Present | Description |
|---|---|---|---|
| `eventId` | string (GUID) | always | Unique event identifier. |
| `event` | string | always | Event type, e.g., `installed`, `uninstalled`. |
| `occurredAt` | string (UTC) | always | When the event occurred (UTC timestamp). |
| `instanceBaseUrl` | string | always | Instance base URL (example: `https://mycompany.my.erp.net`). |
| `user` | string | always | The approving user (example: `admin`). |
| `clientSecret` | string | when requested | Issued client secret (sensitive). *Only present if requested during install.* |
| `referenceToken` | string | when requested | Issued service access token (sensitive). *Only present if requested during install.* |
| `request` | object | always | Echo of the original install request parameters. |

### `request` object fields

The `request` object echoes the application configuration that was successfully registered in the @@name instance.

| Field | Type | Description |
|---|---|---|
| `applicationName` | string | Application display name. |
| `applicationUri` | string | External application identifier. |
| `clientType` | string | Client type (`confidential`, `public`, or `none`). |
| `redirectUri` | string | Redirect/callback URL provided during install. |
| `impersonate` | string | Impersonation scope (`none`, `internal`, `all`). |
| `requestSecret` | bool | Whether credentials were requested. |
| `serviceAccess` | string | Service access mode (`none`, `clientCredentials`, `referenceToken`). |
| `referenceTokens` | string | Who can issue reference access tokens (`none`, `authenticatedUsers`, `administratorsOnly`). |
| `scope` | string | Space-delimited scope string. |

## Signature Validation

Because your `redirectUri` is a publicly accessible endpoint, you must verify the authenticity of incoming webhooks to ensure they were actually sent by the @@name Marketplace and haven't been tampered with.

The @@name Marketplace computes its cryptographic signature over the **compact (no whitespace) JSON serialization** of the inner `payload` object. To successfully verify the signature, your application must recreate this exact compact string.

### Fetching the Public Key

The @@name Marketplace's public key URL is discoverable via its well-known configuration endpoint.  

**Configuration URL:**
`https://marketplace.erp.net/.well-known/marketplace.json`

**Example Response:**

```json
{
  "name": "ERP.net Marketplace",
  "api_version": "1.0",
  "endpoints": {
    "public_key": "https://api.marketplace.erp.net/functions/v1/public-key",
    "app_lifecycle_callback": "https://api.marketplace.erp.net/functions/v1/app-lifecycle-callback"
  }
}
```

**Verification Steps:**

1. **Discover the key URL:** Fetch the well-known configuration JSON and extract the URL from `endpoints.public_key`.
2. **Fetch the public key:** Make a GET request to that extracted URL to retrieve the @@name Marketplace's current RSA public key. (*Note: You should cache this key locally in your application to avoid fetching it on every webhook*).
3. **Parse the webhook:** Read the incoming HTTP POST request body into a JSON object and extract the `signature` string and the nested `payload` object.
4. **Serialize the payload:** Convert the extracted `payload` object back into a compact JSON string with absolutely no whitespace, line breaks, or formatting. (*E.g., in JavaScript/Node.js, this is the exact output of JSON.stringify(payload)*).
5. **Verify the signature:** Confirm that the cryptographic `signature` matches your newly generated compact string using the @@name Marketplace's public key (RSA-SHA256).
6. **Process or reject:** If the signature is valid, you can safely process the event details. If invalid, reject the request with an `HTTP 401 Unauthorized` or `400 Bad Request`.

---

## Examples

The following examples demonstrate how the `v2` envelope wraps the underlying `v1` event data as a nested JSON object based on different installation requests.

### 1) Confidential requesting credentials (Install)

**Request:**

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

**Onboarding callback payload:**

```json
{
  "schema": "erpnet.appLifecycleEvent.v2",
  "signature": "MIGfMA0GCSqGSIb3DQEBAQUAA4GNADCBiQKBgQC3...",
  "payload": {
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
}
```

### 2) Confidential requesting a client secret and a reference token (Install)

**Request:**

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

**Onboarding callback payload:**

```json
{
  "schema": "erpnet.appLifecycleEvent.v2",
  "signature": "k3DQEBAQUAA4GNADCBiQKBgQC3MIGfMA0GCSqGSI...",
  "payload": {
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
}
```

### 3) Public with interactive sign-in = all

**Request:**

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

**Onboarding callback payload:**

```json
{
  "schema": "erpnet.appLifecycleEvent.v2",
  "signature": "BiQKBgQC3MIGfMA0GCSqGSIk3DQEBAQUAA4GNADC...",
  "payload": {
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
}
```

### 4) Uninstall

**Request:**

```http
https://mycompany.my.erp.net/manage/apps/uninstall
  ?applicationUri=MyExternalAppIdentifier
```

**Onboarding callback payload:**

```json
{
  "schema": "erpnet.appLifecycleEvent.v2",
  "signature": "CBiQKBgQC3MIGfMA0GCSqGSIk3DQEBAQUAA4GNAD...",
  "payload": {
    "schema": "erpnet.appLifecycleEvent.v1",
    "eventId": "0e3bf686-4abc-4230-9ca0-47c4efa12b09",
    "event": "uninstalled",
    "occurredAt": "2026-01-21T14:42:23.1597831Z",
    "instanceBaseUrl": "https://mycompany.my.erp.net",
    "user": "admin"
  }
}
```

## Secret Handling

If the decrypted payload contains a `clientSecret` or `referenceToken`, treat it as a password:

- Do not log it.
- Store it securely in a secret manager or vault.
- Restrict access following the principle of least privilege.
