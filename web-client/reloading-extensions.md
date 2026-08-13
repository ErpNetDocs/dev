---
title: Reloading Web Client extensions
description: How to reload registered Web Client extensions and inspect registration results.
---

# Reloading Web Client extensions

After adding or changing a Web Client extension registration, the Web Client automatically requests a reload when the extension record is saved through the Web Client. This makes the updated registration available without waiting for the normal **10-minute refresh cycle**.

The automatic reload is asynchronous and coalesced: several saves made close together result in one reload request. The save operation is not blocked while the extension registrations are being rebuilt.

If an extension is changed through another client or directly in the database, use the reload endpoint or the **Reload extensions** action on the Web Client extensions screen.

## Request

The endpoint is a `GET` request relative to the Web Client site:

```http
GET /cl/api/webclient/apps/reload
```

The endpoint requires an authenticated Web Client API request. It accepts either the current Web Client authentication cookie or an API access token. For a browser request made inside the Web Client, the existing session cookie is used automatically.

For an external application, send the access token in the `Authorization` header:

```http
Authorization: Bearer <access-token>
```

The external extension application can call the endpoint after it authenticates with ERP.net using the authorization code flow with PKCE.

## Response

The response contains the active extensions found and the result of registering each extension:

```json
{
  "reloadedAt": "2026-08-13T10:30:00Z",
  "extensionsFound": 2,
  "applicationsRegistered": 1,
  "panelsRegistered": 1,
  "extensions": [
    {
      "id": "00000000-0000-0000-0000-000000000000",
      "name": "Customer context panel",
      "title": "Customer context",
      "hint": "Shows customer information",
      "extensionUri": "erpnet.demo-extension.3e1",
      "extensionPath": "/forms/panels",
      "extensionData": "{\"forms\":[{\"kind\":\"form\",\"sidePanel\":true}]}",
      "success": true,
      "errors": []
    }
  ]
}
```

`extensions` includes both main-menu applications and form-panel registrations. Each entry reports its own result:

| Property | Description |
|---|---|
| `id` | Extension registration identifier. |
| `name` | Internal registration name. |
| `title` | Optional display title. |
| `hint` | Optional display hint. |
| `extensionUri` | External extension application identifier. |
| `extensionPath` | Extension point, such as `/mainmenu/apps` or `/forms/panels`. |
| `extensionData` | Raw extension-point-specific JSON configuration. |
| `success` | `true` when the registration was processed successfully. |
| `errors` | Errors specific to this registration. Empty when `success` is `true`. |

The response also contains these values. `reloadedAt` is the UTC timestamp of the reload operation.

- `extensionsFound` — active extension records loaded from `Systems.Core.Extensions`.
- `applicationsRegistered` — successfully registered main-menu applications.
- `panelsRegistered` — successfully registered form panels.

## Web Client extensions screen

Authenticated users can inspect the same registration result from the Web Client UI. Open the user profile menu, select the ellipsis menu next to **Sign out**, and then select **Extensions**. The page is available at `/cl/extensions`.

The screen displays:

- the time of the last reload;
- counts of discovered extensions, registered applications, registered panels, and failed registrations;
- every active extension, including its name, title, hint, extension URI, and extension path;
- the raw `ExtensionData` configuration;
- registration-specific errors;
- a **Registered** or **Not registered** status and an error count when validation or registration fails.

The page keeps the last reload result in the server-side Web Client application container. Selecting **Reload extensions** performs a new reload and replaces the displayed result. This is the same operation and result returned by the reload endpoint. The page also reflects automatic reloads after extension records are saved through the Web Client. Use the **Web Client home** link in the page header to return to the Web Client home page.

## After reloading

The endpoint and the automatic post-save reload refresh the server-side extension registrations. The browser may still need to be refreshed because the already-running Web Client initializes its application and panel lists when the page or form is created.

For main-menu applications, a user with permission to manage the menu layout may also need to enable the application from the main-menu settings.

## Troubleshooting

If an entry has `success: false`, inspect its `errors` array and verify the registration associated with that entry. Common causes include:

- invalid JSON in `ExtensionData`;
- an invalid or missing `uri`;
- an invalid extension-panel `forms` definition;
- an unknown repository or namespace;
- an invalid view mode;
- an invalid main-menu slug or category;
- an incorrect `ExtensionPath`.

Every active registration is validated against the supported Web Client extension paths. The current supported values are `/mainmenu/apps` and `/forms/panels`. A record with another path is shown as **Not registered** and includes a path-validation error.

The endpoint is intended for reloading registration state. It does not validate the external application's trusted-app configuration or test whether its URL is reachable from the browser.
