---
title: Registering Web Client extensions
description: Overview of the extension points available for extending ERP.net Web Client.
---

# Registering Web Client extensions

ERP.net Web Client extensions are registration records that connect an external application to a supported Web Client extension point.

All Web Client extensions are stored in [`Systems.Core.Extensions`](https://docs.erp.net/model/entities/Systems.Core.Extensions.html). The registration identifies the Web Client as the application being extended and selects an extension point through `ExtensionPath`.

## Extension types

| Extension type | Extension path | Purpose |
|---|---|---|
| Main-menu application | `/mainmenu/apps` | Adds an external application to a Web Client main-menu category. |
| Form panel | `/forms/panels` | Displays an external application inside a Web Client form as a main or side panel. |

All extension types use the following common registration fields:

| Field | Description |
|---|---|
| `Name` | Internal name of the registration. |
| `ApplicationUri` | The application being extended. Use `internal.erp.net/webclient`. |
| `ExtensionPath` | The Web Client extension point. |
| `ExtensionUri` | Identifier of the external extension application. |
| `ExtensionData` | Extension-point-specific JSON configuration stored as a string. |
| `IsActive` | Enables or disables the registration. |
| `Title` | Optional display title. |
| `Hint` | Optional display hint. |

## Choose a topic

- [Registering main-menu extensions](registering-mainmenu-extensions.md) — add external applications to Web Client main-menu categories.
- [Registering extension panels](registering-extension-panels.md) — display external applications inside navigator, entity, namespace, repository, or universal form contexts.

## Common considerations

- The external application URL may use Web Client interpolated variables such as `$instance` and `$rooturl`.
- Browser applications should use the ERP.net authorization code flow with PKCE and be registered as public trusted applications.
- The exact redirect URL must be registered in the ERP.net instance, including any query parameters required by the application.
- When a registration is saved through the Web Client, it is reloaded automatically after the transaction commits. For registrations changed through another client or directly in the database, use [Reloading Web Client extensions](reloading-extensions.md).

## Related topic

See [Reloading Web Client extensions](reloading-extensions.md) to refresh registrations and inspect registration results.
