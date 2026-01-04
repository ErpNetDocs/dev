# Make your first API call

This page shows how to validate end-to-end access: token -> API call -> successful response.

## 1) Choose which API you will call first

@@name exposes multiple API surfaces. Before you implement anything, decide which one you are targeting:

- [API Access (Domain API vs Table API)](../concepts/api-access/overview.md)

Pick one API and stick with it for the first validation call.

## 2) Call the API with a Bearer token

Use the token you obtained in the previous step and call an endpoint that is safe and easy to validate.

A typical request has this shape:

```http
GET {api-endpoint}
Authorization: Bearer {access_token}
Accept: application/json
```

Where:

- `{access_token}` is the token you acquired from the selected identity authority
- `{api-endpoint}` is the endpoint from the API surface you chose (Domain API or Table API)

Important:

- Use the API URLs from the canonical API documentation pages to avoid mixing base URL variants. See [API Access](../concepts/api-access/overview.md).

## 3) What success looks like

- HTTP 200 OK
- JSON response body
- no redirects to login pages

## 4) If it fails (quick triage)

- **401 Unauthorized** usually means:
  - wrong authority / wrong token type
  - token missing/expired
  - request missing Authorization: Bearer ...
- **403 Forbidden** usually means:
  - token is valid but lacks required access (trusted app config / scopes / rules)

Start troubleshooting from:

- [Trusted Applications](../auth/configuration/trusted-apps-access.md)
- [Scopes](../auth/configuration/scopes.md)
- [Authentication and Authorization](../auth/configuration/overview.md)

> [!WARNING]
> Do not log access tokens in production and do not embed secrets in frontend code. See: [Security Best Practices](../auth/concepts/security-best-practices.md)
