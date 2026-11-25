# Supported Token Exchange use cases

@@name currently supports one use case for Token Exchange, Agent impersonation.

## Agent impersonation

Use Agent impersonation when a backend needs to call @@name APIs under an Agent account's permissions. For example, a catalog assistant can read product data under the permissions of its dedicated Agent account.

The backend authenticates as an enabled confidential Trusted Application using its own client credentials. It supplies the login of an active Agent in the target instance. The backend chooses which Agent to impersonate, and the ID site issues an access token that represents that Agent. API calls use the Agent's permissions and the scopes granted by the exchange. [Request and scope rules](token-exchange.md)

### Subject JWT

In the `subject_token` form field, send a compact unsigned JWT. Set the header's `alg` to `none` and leave the signature segment empty, so the JWT ends with a period.

The ID site uses these payload claims to identify the user and check when the assertion is valid.

| Claim | Required | Value |
|-------|----------|-------|
| `sub` | Yes | The login of an active Agent in the target instance, as a non-empty JSON string. |
| `nbf` | No | The earliest time the assertion can be used, as a Unix timestamp in seconds. |
| `exp` | No | The time the assertion expires, as a Unix timestamp in seconds. |

The Trusted Application's client credentials establish trust in the assertion, so the payload can omit `iss` and `aud`. The issued access token gets its user claims from the Agent's account.

For a complete example, see [Get an access token for an Agent](../samples/token-exchange-agent-example.md).
