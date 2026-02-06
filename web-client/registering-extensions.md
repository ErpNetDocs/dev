---
title: Registering Extensions
description: How to register extensions for ERP.net WebClient and how WebClient consumes them.
---

# Registering Extensions

## Overview

An **extension** is a registration record that allows an application to extend the behavior of a **main application** at a predefined extension point.

Currently, WebClient supports registering additional applications in its **Main menu** through the extension point described in this article.

## WebClient main menu extension point

### ExtensionPath (constant)

For WebClient main menu apps, the extension point is identified by a **constant**:

- `ExtensionPath = "/mainmenu/apps"`

WebClient resolves main menu extensions by looking up records with **exact match** on `ExtensionPath`.

### ExtensionData (JSON) for main menu apps

For `ExtensionPath="/mainmenu/apps"`, WebClient expects `ExtensionData` to be a JSON object with the following shape:

```json
{
  "category": "Forms_Crm",
  "uri": "some-app-uri?{interpolated-string}",
  "slug": "my-extension",
  "icon": "globe"
}
```

#### JSON fields

- `category` (required)  
  The WebClient main menu category key. Must be one of:

  - `My`
  - `Forms_Crm`
  - `Forms_Finance`
  - `Forms_General`
  - `Forms_Logistics`
  - `Forms_Production`
  - `Forms_Projects`
  - `Forms_HumanResources`
  - `Forms_Regulatory`
  - `Forms_Applications`
  - `Forms_Systems`

- `uri` (required)  
  Required string field in the JSON payload. If missing or empty, WebClient rejects the extension registration.

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

## URI interpolation

`uri` can include placeholders which WebClient replaces at runtime using the current user/context.

Example:

```json
{
  "category": "Forms_Crm",
  "uri": "https://example.app/?ec={$enterprisecompany}&ecl={$enterprisecompanylocation}&u={$user}&ru={$rooturl}",
  "slug": "example-extension",
  "icon": "puzzle-piece"
}
```

### Standard interpolation parameters

The following parameters are available for interpolation inside `uri`:

| Parameter | Description |
|---|---|
| `$enterprisecompany` | The enterprise company in the current (transaction) context. |
| `$enterprisecompanylocation` | The enterprise company location in the current (transaction) context. |
| `$instance` | Current database instance (equals `$rooturl`, but without the scheme prefix). |
| `$newguid` | Serialized random GUID. |
| `$role` | Role instance of the current user’s role. |
| `$rooturl` | Current database URL. |
| `$time` | Server system time. |
| `$user` | User instance of the current user. |

> Note  
> Use these parameters only where appropriate for your application. For example, pass enterprise context only if the extension app needs it.

## How WebClient resolves and renders main menu extensions

When the user opens WebClient main menu, WebClient:

1. Loads extension registrations where `ExtensionPath = "/mainmenu/apps"`
2. Parses `ExtensionData` JSON
3. Filters records by `ExtensionData.category` equal to the currently opened main menu category
4. Renders the matching extensions as additional apps in the menu

## Reloading registered apps

WebClient provides an endpoint that forces a refresh/reload of the registered WebClient apps:

- `/cl/api/webclient/apps/reload`

Use this endpoint after adding/updating extension registrations when you need WebClient to reload the apps list without waiting for a natural refresh cycle (10 mins).

## Troubleshooting

### Symptom: Extension does not appear in the menu

**Checklist:**
- The registration record is active (`IsActive = true`)
- `ExtensionPath` is exactly `/mainmenu/apps`
- `ExtensionData` is valid JSON
- `ExtensionData.category` is one of the supported category keys
- `ExtensionData.uri` is present and non-empty (and interpolation placeholders are well-formed)
- `ExtensionData.slug` matches the validation rules

### Validation errors logged in Info Messages (exact strings)

When WebClient detects an invalid extension registration for `ExtensionPath="/mainmenu/apps"`, it logs an error entry in **Info Messages**.

Each error is logged as a multi-line block in the following format:

- First line: the **error message** (see the exact strings below)
- Then context fields:
  - `ExtensionId:{Id}`
  - `ExtensionUri:{ExtensionUri}`
  - `Name:{Name}`
  - `Path:{ExtensionPath}`
  - `ExtensionData:` followed by the raw JSON payload
- If the failure is caused by an exception (e.g. JSON parsing), the log entry also contains:
  - `Error:` followed by the exception details (stack trace)

#### Exact error messages

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

- `Invalid category "{category}". The provided category is not found among Web Client main menu categories.`
  - Logged when `category` does not match any of the available WebClient main menu categories.
