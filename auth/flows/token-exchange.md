# OAuth 2.0 Token Exchange

With the standard OAuth 2.0 Token Exchange grant, a confidential application authenticates with its client credentials and exchanges a subject token for an @@name API access token. The issued token represents the subject. [RFC 8693](https://www.rfc-editor.org/rfc/rfc8693.html)

@@name currently supports [Agent impersonation](token-exchange-use-cases.md#agent-impersonation). The [Agent sample](../samples/token-exchange-agent-example.md) shows a complete HTTP request and response.

## Configure the client

1. In the target instance, enable a [Trusted Application](../configuration/trusted-apps-access.md) with `ClientType = Confidential`.
2. Set `ApplicationSecretHash` to the Base64-encoded SHA-256 hash of the application's client secret. Configure the API scopes the application needs.
3. Use `ApplicationUri` as `client_id` and keep the client secret on the server.

Public clients and the built-in Windows desktop client are ineligible for this grant.

## Request an access token

Send an `application/x-www-form-urlencoded` POST over HTTPS to `/id/connect/token`. Use `token_endpoint` from the instance's [discovery document](../concepts/identity-server.md#1-discovery-endpoint) to find the full URL. The table shows client authentication in the form body.

| Form field | Value |
|------------|-------|
| `grant_type` | `urn:ietf:params:oauth:grant-type:token-exchange` |
| `client_id` | The Trusted Application's `ApplicationUri`. |
| `client_secret` | The app's client secret. |
| `subject_token_type` | `urn:ietf:params:oauth:token-type:jwt` |
| `subject_token` | The [subject JWT](token-exchange-use-cases.md#subject-jwt) that identifies the user to impersonate. |
| `scope` | Optional space-delimited list of API scopes. |
| `requested_token_type` | Optional. The supported value is `urn:ietf:params:oauth:token-type:access_token`. |

Request only [API scopes](../concepts/scopes.md), such as `read` or `update`. Omit the `audience`, `resource`, `actor_token`, and `actor_token_type` form fields.

## Read the response

A successful request returns HTTP `200`.

| Response field | Value |
|----------------|-------|
| `access_token` | The token to send to the API. |
| `issued_token_type` | `urn:ietf:params:oauth:token-type:access_token` |
| `token_type` | `Bearer` |
| `expires_in` | The token lifetime in seconds. |
| `scope` | The space-delimited API scopes granted to the token. |

The instance grants the API scopes common to the Trusted Application and the subject. If the subject's scope setting is blank, the application's API scopes apply. Supplying `scope` narrows the result to the requested scopes. At least one API scope must remain, and scope names are case-sensitive.

Send `Authorization: Bearer <access_token>` when calling the API. Find the API base URL through [site auto-discovery](../../building-apps/concepts/application-server-endpoints.md). Repeat the exchange when the access token expires.

## Troubleshoot the request

| Error | What to check |
|-------|---------------|
| `invalid_client` | Verify the client ID and secret for the target instance. |
| `unsupported_grant_type` | Check that the instance supports token exchange and that the Trusted Application is eligible. |
| `invalid_request` | Check `subject_token_type`, the [JWT format and claims](token-exchange-use-cases.md#subject-jwt), and whether the subject is an active Agent. |
| `invalid_scope` | Check that the app can request the scopes and that at least one eligible API scope remains. |
| `invalid_target` | Remove `resource` and `audience` from the request. |
