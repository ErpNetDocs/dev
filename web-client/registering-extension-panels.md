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

## Passing form context to the panel

The panel can receive the current form context in two complementary ways:

- **`uri`** — the evaluated value becomes the iframe `src`. Use it for values the application needs when it starts, such as the ERP.net instance or an initial object identifier. When the evaluated URI changes, the Web Client assigns the new value to the iframe and the external application is loaded again.
- **`message`** — the evaluated value is sent to the iframe with `window.postMessage`. Use it for context changes that the running application can handle without being loaded again, such as a focused object, selected objects, a filter, a view-mode change, or a saved-data notification.

The two mechanisms can be used together. A common pattern is to put only the instance in `uri` and the changing form context in `message`:

```json
{
  "uri": "https://example.com/form-context-extension/?instance={$instance}",
  "message": "formKind={$formKind}&repository={$repository}&id={$id}&selectedids={$selectedids}&filter={$filter}&saveCounter={$saveCounter}"
}
```

The Web Client reevaluates `uri` and `message` whenever the form context changes. If the evaluated URI is unchanged, the iframe remains loaded. If only the evaluated message changes, the Web Client sends the new message to the existing iframe. The external application decides which changes require a state update and can ignore messages whose data is unchanged.

The first message is sent after the panel iframe is initially rendered. A message is also sent after relevant form events and when the user chooses **Reload** from the panel menu. If `message` is not registered, the Web Client does not send `postMessage` events.

The application should listen for the agreed message type and parse the payload as URL parameters:

```javascript
window.addEventListener("message", (event) => {
  if (event.data?.type !== "erpnet.extension.message")
    return;

  const context = new URLSearchParams(String(event.data.data ?? ""));
  const repository = context.get("repository") ?? "";
  const id = context.get("id") ?? "";
  const saveCounter = context.get("saveCounter") ?? "0";
  // Update the application state as needed.
});
```

The Web Client URL-encodes every interpolated variable value before inserting it into either `uri` or `message`. This is especially important for `$filter`, which can contain spaces, quotes, ampersands, and other reserved characters. Always use URL parsing APIs such as `URL` and `URLSearchParams`; do not split or decode interpolated values manually.

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
| `$saveCounter` | Number of successfully completed saves in an editable data form. It starts at `0` and increases after each successful commit. |

The variables available in a particular form depend on its current context. An omitted value is represented as an empty value.

### Message format

The message is a standard URL-parameter string. Use `&` between parameters and parse the received value with `URLSearchParams`:

```text
formKind={$formKind}&repository={$repository}&id={$id}&filter={$filter}&saveCounter={$saveCounter}
```

The message payload is URL-encoded as described in [Passing form context to the panel](#passing-form-context-to-the-panel).

When `$saveCounter` is included in `message`, its value changes after a successful save and the running iframe receives a new `postMessage`. The iframe is not reloaded unless a variable used in `uri` also changes. `$saveCounter` is available only for forms derived from `EditableDataForm`.

## Authentication

The iframe application authenticates independently using the ERP.net authorization code flow with PKCE. Register the application as a public trusted application; do not put a client secret in browser code.

The redirect URL must contain the instance parameter used by the app:

```text
https://example.com/form-context-extension/?instance=<instance-name>&auth=callback
```

For local development, register the corresponding localhost URL as a separate redirect URL. Replace `<instance-name>` with the ERP.net instance host.

## Example application

The complete runnable example is available in the [form-context-extension sample](https://github.com/ErpNetDocs/dev/tree/master/web-client/samples/form-context-extension). 

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

## Related topic

See [Reloading Web Client extensions](reloading-extensions.md) to refresh registrations and inspect registration results.
