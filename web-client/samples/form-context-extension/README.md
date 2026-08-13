# ERP.net form context extension

Production-oriented universal extension-panel example. It displays all received interpolated form arguments and, when `$repository` and `$id` are available, loads the object `DisplayText` from the corresponding Domain API repository.

Register it through the Web Client `/forms/panels` extension point.

```json
{
  "forms": [{ "kind": "form", "sidePanel": true }],
  "uri": "https://example.com/form-context-extension/?instance={$instance}",
  "message": "formKind={$formKind}&namespace={$namespace}&repository={$repository}&id={$id}&selectedids={$selectedids}&filter={$filter}&viewMode={$viewMode}&editMode={$editMode}&saveCounter={$saveCounter}",
  "icon": "puzzle-piece"
}
```

The Web Client URL-encodes every interpolated variable value before evaluating both the URI and the message. The message uses standard URL parameters separated by `&`, and the app parses it with `URLSearchParams`. Values such as `$filter` remain safe even when they contain spaces, quotes, ampersands, or other reserved characters. The `$saveCounter` value changes after each successful save and allows the app to react to saved data through `postMessage` without reloading the iframe.

The app deduplicates unchanged messages, aborts stale requests, uses short-lived in-memory caching, logs errors, supports retry, and uses the local PKCE-enabled `erpnet-client.js` included in this folder. It requires `instance` in the URL and has no fallback instance.

## Example trusted application

Register the public application once in each ERP.net instance where the extension will be used. Replace `<instance-name>` with the instance host, such as `e1-nbeta.local`.

The application is a public PKCE client, so it does not require a client secret:

```json
{
  "ApplicationUri": "erpnet.demo-extension.3e1",
  "Name": "ERP.net Form Context Demo",
  "ClientType": "Public",
  "Scope": "openid profile offline_access read",
  "ImpersonateAsCommunityUserAllowed": true,
  "ImpersonateAsInternalUserAllowed": true,
  "ImpersonateLoginUrl": "https://example.com/form-context-extension/?instance=<instance-name>&auth=callback,http://localhost:5173/form-context-extension/?instance=<instance-name>&auth=callback",
  "ImpersonateLogoutUrl": "https://example.com/form-context-extension/?instance=<instance-name>,http://localhost:5173/form-context-extension/?instance=<instance-name>",
  "SystemUserAllowed": false
}
```

The login callback URL must match the URL used by the app, including the `instance` query parameter. If the extension is hosted at another path, replace `/form-context-extension/` in both the panel registration and trusted application configuration. The localhost URL is optional and is useful only for local development.
