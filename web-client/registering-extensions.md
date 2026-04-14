---
title: Registering Extensions
description: How to register extensions for ERP.net WebClient main menu and how to fill the extension registration record.
---

# Registering Extensions

## Overview

An **extension** is a registration record that allows an application to extend the behavior of a **main application** at a predefined extension point.

Currently, WebClient supports registering additional applications in its **Main menu** through the extension point described in this article.

## Getting Started

To register an application in the WebClient main menu, create a record in [`Systems.Core.Extensions`](https://docs.erp.net/model/entities/Systems.Core.Extensions.html).

For WebClient main menu extensions, fill the registration record as follows:

| Field | Value |
|---|---|
| `Name` | Internal name of the registration. Use a stable, descriptive value. |
| `ApplicationUri` | `internal.erp.net/webclient` |
| `ExtensionPath` | `/mainmenu/apps` |
| `ExtensionUri` | URI of the extension application |
| `ExtensionData` | JSON payload described below |
| `IsActive` | `true` |

You can also optionally fill:

| Field | Purpose |
|---|---|
| `Title` | Multi-language title used for UI display |
| `Hint` | Multi-language hint used for UI display |

### Example registration record

```text
Name: WebClient Extension Test
ApplicationUri: internal.erp.net/webclient
ExtensionPath: /mainmenu/apps
ExtensionUri: test-extension-app
IsActive: true
ExtensionData:
{
  "category": "Crm",
  "uri": "https://bing.com?q={$enterprisecompany}%20{$enterprisecompanylocation}",
  "slug": "test-extension",
  "icon": "globe"
}
```

This example opens a Bing search for the current enterprise company and enterprise company location.

### What each field means

- `ApplicationUri` identifies the **main application being extended**.  
  For WebClient main menu extensions, use `internal.erp.net/webclient`.

- `ExtensionPath` identifies the **extension point inside WebClient**.  
  For main menu applications, use `/mainmenu/apps`.

- `ExtensionUri` identifies the **extension application**.

- `ExtensionData` contains the **main-menu-specific configuration** for the extension.

### Creating the record through Domain API

You can create the extension registration through the Domain API by sending a `POST` request to the `Systems_Core_Extensions` collection of [`Systems.Core.Extensions`](https://docs.erp.net/model/entities/Systems.Core.Extensions.html).

Example request:

```http
POST /api/domain/odata/Systems_Core_Extensions HTTP/1.1
Host: <instance-url>
Authorization: Bearer <access-token>
Content-Type: application/json
```

Request body:

```json
{
  "ApplicationUri": "internal.erp.net/webclient",
  "ExtensionData": "{\n\"category\": \"Crm\",\n\"slug\": \"test-extension\",\n\"uri\":\"https://bing.com?q={$enterprisecompany}%20{$enterprisecompanylocation}\",\n\"icon\": \"globe\"\n}",
  "Name": "WebClient Extension Test",
  "ExtensionPath": "/mainmenu/apps",
  "ExtensionUri": "test-extension-app",
  "Hint": "Test extension for ERP.net Web Client",
  "IsActive": true,
  "Title": "Extension Test"
}
```

> Note  
> `ExtensionData` is stored as a **string**, whose content must be a valid JSON object.

Example created record:

```json
{
  "@odata.id": "Systems_Core_Extensions(b0558cb4-3ab3-4f73-adf7-c4ff222cd705)",
  "Id": "b0558cb4-3ab3-4f73-adf7-c4ff222cd705",
  "ApplicationUri": "internal.erp.net/webclient",
  "ExtensionData": "{\n\"category\": \"Crm\",\n\"slug\": \"test-extension\",\n\"uri\":\"https://bing.com?q={$enterprisecompany}%20{$enterprisecompanylocation}\",\n\"icon\": \"globe\"\n}",
  "Name": "WebClient Extension Test",
  "ExtensionPath": "/mainmenu/apps",
  "ExtensionUri": "test-extension-app",
  "Hint": "Test extension for ERP.net Web Client",
  "IsActive": true,
  "Title": "Extension Test"
}
```

## Configuration

## WebClient main menu extension point

### ExtensionPath

For WebClient main menu apps, the extension point is identified by the constant:

- `ExtensionPath = "/mainmenu/apps"`

WebClient resolves main menu extensions by looking up records with **exact match** on `ExtensionPath`.

### ExtensionData for main menu apps

For `ExtensionPath="/mainmenu/apps"`, WebClient expects `ExtensionData` to be a JSON object with the following shape:

```json
{
  "category": "Crm",
  "uri": "some-app-uri?{interpolated-string}",
  "slug": "my-extension",
  "icon": "globe"
}
```

### Main menu categories

The `category` field determines in which **top-level WebClient main menu category** the extension app is shown.

The value written in `category` is the **internal category key**, not the visible label shown in the menu.

For example, to place the extension under the visible **CRM** category, use:

- `Crm`

not:

- `CRM`

### Internal category keys vs. visible menu labels

Use the **internal category key** from the left column in `ExtensionData.category`.

| Internal category key | Visible menu label |
|---|---|
| `My` | `My` |
| `Crm` | `CRM` |
| `Finance` | `Finance` |
| `Logistics` | `Logistics` |
| `Production` | `Production` |
| `Projects` | `Work Management` |
| `Applications` | `Business Apps` |
| `Systems` | `System` |
| `General` | `Foundation` |
| `Regulatory` | `Regulatory` |
| `HumanResources` | `Human Resources` |

> Note  
> Category matching is **case-sensitive**.

### JSON fields

- `category` (required)  
  The internal WebClient main menu category key. The value must exactly match the internal category name, including correct letter casing.

- `uri` (required)  
  The URL/URI that WebClient opens for the extension application.

  If missing or empty, WebClient rejects the extension registration.

  `uri` may contain **interpolated string parameters** (see [Standard interpolation parameters](#standard-interpolation-parameters)).

- `slug` (required)  
  Identifies the extension application target.

  **Validation rules:**
  - must start with a lowercase letter or digit
  - must end with a lowercase letter or digit
  - allowed characters: lowercase letters (`a-z`), digits (`0-9`), hyphen (`-`), underscore (`_`)

  Regex:

  - `^[a-z0-9]([a-z0-9_-]*[a-z0-9])?$`

- `icon` (optional)  
  Font Awesome icon **name without the `fa-` prefix**.

  If missing, WebClient uses the system default icon:

  - `puzzle-piece`

## Concepts

### ApplicationUri

For WebClient main menu extensions, set:

- `ApplicationUri = "internal.erp.net/webclient"`

This tells ERP.net that the extended application is the **WebClient**.

### ExtensionUri

`ExtensionUri` is the URI of the extension application.

For WebClient main menu extensions, this field is part of the registration record in [`Systems.Core.Extensions`](https://docs.erp.net/model/entities/Systems.Core.Extensions.html) and should identify the extension app being registered.

### URI interpolation

`uri` can include placeholders which WebClient replaces at runtime using the current user/context.

Example:

```json
{
  "category": "Crm",
  "uri": "https://example.app/?ec={$enterprisecompany}&ecl={$enterprisecompanylocation}&u={$user}&ru={$rooturl}",
  "slug": "example-extension",
  "icon": "puzzle-piece"
}
```

### Standard interpolation parameters

The following parameters are available for interpolation inside `uri`:

| Parameter | Description |
|---|---|
| `$dbname` | Current database name. |
| `$enterprisecompany` | The enterprise company in the current (transaction) context. |
| `$enterprisecompanylocation` | The enterprise company location in the current (transaction) context. |
| `$instance` | Current database instance (equals `$rooturl`, but without the scheme prefix). |
| `$newguid` | Serialized random GUID. |
| `$role` | Role instance of the current user's role. |
| `$rooturl` | Current database URL. |
| `$time` | Server system time. |
| `$user` | User instance of the current user. |

> Note  
> Use these parameters only where appropriate for your application.  
> For example, pass enterprise context only if the extension app needs it.

### How WebClient resolves and renders main menu extensions

When the user opens the WebClient main menu, WebClient:

1. Loads extension registrations where `ExtensionPath = "/mainmenu/apps"`
2. Parses `ExtensionData` JSON
3. Filters records by `ExtensionData.category` equal to the currently opened main menu category
4. Renders the matching extensions as additional apps in the menu

## Reloading registered apps

WebClient provides an endpoint that forces a refresh/reload of the registered WebClient apps:

- `/cl/api/webclient/apps/reload`

Use this endpoint after adding or updating extension registrations when you need WebClient to reload the apps list without waiting for the natural refresh cycle (10 mins).

> Note  
> Calling `/cl/api/webclient/apps/reload` reloads the registered extension apps on the server side, so the extension becomes available in the list of apps for the specified category.  
> However, it may still not be visible in the menu on screen.
>
> If the extension is not shown, a user with permission to manage the menu layout must enable it from the main menu settings (the **gear** button).
>
> Also, the browser page running WebClient must be refreshed after the reload. This is required because the Blazor Server application initializes the menu when the page is loaded, so menu changes do not appear in an already loaded page automatically.

## Troubleshooting

### Symptom: Extension does not appear in the menu

**Checklist:**

- The registration record exists in [`Systems.Core.Extensions`](https://docs.erp.net/model/entities/Systems.Core.Extensions.html)
- The registration record is active (`IsActive = true`)
- `ApplicationUri` is exactly `internal.erp.net/webclient`
- `ExtensionPath` is exactly `/mainmenu/apps`
- `ExtensionData` is valid JSON
- `ExtensionData.category` matches an existing internal WebClient main menu category key
- `ExtensionData.category` uses the correct letter casing
- `ExtensionData.category` uses the internal key, not the visible menu label
- `ExtensionData.uri` is present and non-empty
- Interpolation placeholders in `ExtensionData.uri` are well-formed
- `ExtensionData.slug` matches the validation rules
- `ExtensionData.slug` is not already used by another registered extension

### Validation errors logged in Info Messages

When WebClient detects an invalid extension registration for `ExtensionPath="/mainmenu/apps"`, it logs an error entry in [**Info Messages**](https://docs.erp.net/model/entities/Systems.Monitoring.InformationMessages.html).

Each error is logged as a multi-line block in the following format:

- First line: the **error message** (see the exact strings below)
- Then context fields:
  - `ExtensionId:{Id}`
  - `ExtensionUri:{ExtensionUri}`
  - `Name:{Name}`
  - `Path:{ExtensionPath}`
  - `ExtensionData:` followed by the raw JSON payload
- If the failure is caused by an exception (for example JSON parsing), the log entry also contains:
  - `Error:` followed by the exception details (stack trace)

### Exact error messages

- `Failed to deserialize extension data.`
  - Logged when `ExtensionData` cannot be parsed/deserialized.
  - The log entry includes an `Error:` section with the exception details.

- `Extension does not privide "uri".`
  - Logged when the JSON payload does not contain a non-empty string field `uri`.

- `Extension does not privide "slug".`
  - Logged when the JSON payload does not contain a non-empty string field `slug`.

- `Invalid slug "{slug}". Use only lowercase letters, numbers, hyphens, or underscores (must start/end with a letter or number).`
  - Logged when `slug` fails validation.
  - Validation regex: `^[a-z0-9]([a-z0-9_-]*[a-z0-9])?$`

- `Duplicate slug "{slug}". Another extension with the same slug has already been registered.`
  - Logged when another extension with the same `slug` is already registered.

- `Invalid category "{category}". The provided category is not found among Web Client main menu categories.`
  - Logged when `category` does not match any of the available WebClient main menu categories.
  - This also happens when the category name is written with incorrect casing or when a visible menu label is used instead of the internal category key.
