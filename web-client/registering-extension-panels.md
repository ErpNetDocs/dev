---
title: Registering extension panels
description: How to register external applications as panels in ERP.net Web Client forms.
---

# Registering extension panels

An extension panel is an external web application displayed inside an iframe in an ERP.net Web Client form. The application can be registered as a main panel or as a side panel and can receive the current form context through an interpolated message.

This topic describes the `/forms/panels` extension point. Main-menu applications use a different extension point; see [Registering main-menu extensions](registering-mainmenu-extensions.md).

## Registration record

Create a record in [`Systems.Core.Extensions`](https://docs.erp.net/model/entities/Systems.Core.Extensions.html) with these values:

| Field | Value |
|---|---|
| `ApplicationUri` | `internal.erp.net/webclient` |
| `ExtensionPath` | `/forms/panels` |
| `ExtensionData` | JSON object described below, stored as a string |
| `IsActive` | `true` |

`Name`, `Title`, `Hint`, and `ExtensionUri` identify the registration and are also used when the panel is displayed and reported by the extension reload endpoint.

## Extension data

```json
{
  "forms": [
    {
      "kind": "form",
      "sidePanel": true,
      "visibleInViewModes": ["view", "fulfillment"]
    }
  ],
  "uri": "https://example.com/form-context-extension/?instance={$instance}",
  "message": "formKind={$formKind}&namespace={$namespace}&repository={$repository}&id={$id}&selectedids={$selectedids}&filter={$filter}&viewMode={$viewMode}&editMode={$editMode}",
  "icon": "puzzle-piece"
}
```

The fields are:

| Field | Required | Description |
|---|---:|---|
| `forms` | yes | One or more form targeting rules. |
| `forms[].kind` | yes | `form`, `namespace`, `repository`, `navigator`, or `entity`. |
| `forms[].repository` | no | Repository used to restrict a `repository`, `navigator`, or `entity` registration. Empty means all repositories of that kind. |
| `forms[].namespace` | no | Namespace used to restrict a `namespace` registration. Empty means all namespace forms. |
| `forms[].visibleInViewModes` | no | View modes in which the panel may be shown. `edit` is not a valid view mode. Omit it to allow all view modes. |
| `forms[].sidePanel` | no | `true` to register the panel as a side panel; otherwise it is a main panel. |
| `uri` | yes | Iframe URL. It may contain interpolated variables. |
| `message` | no | Interpolated data sent to the iframe. If omitted, no message is sent. |
| `icon` | no | Font Awesome icon name without the `fa-` prefix. Defaults to `puzzle-piece`. |

The same external application can provide multiple `forms` entries when it needs different targeting or presentation rules. The `form` kind is universal and can be used when the application wants to inspect `$formKind` and decide what to display.

### Form targeting

| Registration kind | Target |
|---|---|
| `form` | Any supported Web Client form. |
| `namespace` | `RepositoriesMenuForm` forms. `namespace` optionally restricts the registration. |
| `repository` | Repository forms. An empty `repository` applies to all repository forms. |
| `navigator` | Navigator forms. An empty `repository` applies to all navigator forms. |
| `entity` | Entity forms. An empty `repository` applies to all entity forms. |

The `repository` registration kind is a scope, not a runtime form type. For example, a registration with `kind: "repository"` can match repository-backed forms while `$formKind` reports the concrete runtime form kind.

## Interpolated variables

Web Client evaluates the `uri` and `message` against the current form context whenever the form context changes. The iframe URL changes only when the evaluated URI changes. If only the message changes, the existing iframe remains loaded and receives the new message.

Variable values are URL-encoded before they are inserted into both the URI and message. The application should parse message values as URL parameters rather than splitting or decoding them manually.

| Variable | Available value |
|---|---|
| `$instance` | Current ERP.net instance host. |
| `$formKind` | Runtime form kind: `form`, `namespace`, `navigator`, or `entity`. |
| `$namespace` | Namespace of a `RepositoriesMenuForm`; empty where no namespace is available. |
| `$repository` | Current repository name, when the form has a repository. |
| `$id` | Focused or current object identifier, when available. |
| `$selectedids` | Comma-separated identifiers of selected objects. |
| `$filter` | Current navigator OData filter. |
| `$viewMode` | Current view mode. |
| `$editMode` | `true` or `false`. |

The variables available in a particular form depend on its current context. An omitted value is represented as an empty value.

### Message format

The message is a standard URL-parameter string. Use `&` between parameters and parse the received value with `URLSearchParams`:

```text
formKind={$formKind}&repository={$repository}&id={$id}&filter={$filter}
```

The Web Client URL-encodes each interpolated variable value before constructing the message. Therefore, a value such as `$filter` may contain encoded spaces, quotes, ampersands, and other reserved characters without creating additional message parameters. The external application receives a browser `postMessage` event with the message payload and should parse it with `URLSearchParams`; it should not split or decode values manually. The application decides whether a particular change requires a state update and should ignore repeated messages whose evaluated data has not changed.

If the panel has a message registration, Web Client sends the current message when the panel is first rendered, after relevant form events, and when the user chooses **Reload** from the panel menu. If no `message` is registered, Web Client does not call `postMessage`.

## Authentication

The iframe application authenticates independently using the ERP.net authorization code flow with PKCE. Register the application as a public trusted application; do not put a client secret in browser code.

The redirect URL must contain the instance parameter used by the app:

```text
https://example.com/form-context-extension/?instance=<instance-name>&auth=callback
```

For local development, register the corresponding localhost URL as a separate redirect URL. Replace `<instance-name>` with the ERP.net instance host.

## Example application

The complete runnable example is available in the [form-context-extension sample folder](./samples/form-context-extension).

The sample demonstrates:

- a universal `form` registration;
- URI initialization with `$instance`;
- context updates through `postMessage`;
- URL-parameter parsing and duplicate-message suppression;
- loading an entity `DisplayText` through the Domain API;
- PKCE authentication, retry handling, and error logging.

For reloading registrations and inspecting per-extension results, see [Reloading Web Client extensions](reloading-extensions.md).

## Troubleshooting

- Verify `ApplicationUri` is `internal.erp.net/webclient`.
- Verify `ExtensionPath` is exactly `/forms/panels`.
- Verify `ExtensionData` is valid JSON stored as a string.
- Verify `forms` is present and contains a valid `kind`.
- Verify repository and namespace names use technical names, not localized captions.
- Verify `visibleInViewModes` contains valid view modes and does not contain `edit`.
- Verify the trusted application contains the exact callback URL, including `instance` and `auth=callback`.
- Verify the external application can reach the instance Domain API and that the instance permits the browser origin.
