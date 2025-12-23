# Common Errors – Service and Background Apps (Client Credentials Flow)

When using the **Client Credentials Flow**, errors typically relate to application configuration, credentials, or scope permissions.  

This table summarizes the most common issues and how to resolve them.

---

| **Error** | **Typical Message** | **Cause** | **Fix** |
|------------|--------------------|------------|----------|
| **invalid_client** | `Invalid client_id or client_secret` | The Trusted Application is not found, disabled, or the secret is incorrect. | Verify the `client_id` (ApplicationUri) matches the Trusted Application, and the `client_secret` is correct. Check that the app is enabled (`IsEnabled = true`) and `ClientType = Confidential`. |
| **unauthorized_client** | `The client is not authorized to use this grant type` | The Trusted Application is not allowed to use the Client Credentials flow. | In the Trusted Application, make sure `SystemUserAllowed = true` and a valid `SystemUser` is assigned. |
| **invalid_scope** | `Invalid scope: update` | The app requested a scope that is not allowed by the Trusted Application. | Check the `Scope` field in the Trusted Application record. Only request scopes defined there (for example, `read update DomainApi`). |
| **invalid_grant** | `Client credentials are invalid or missing` | The request is malformed, missing parameters, or uses an incorrect grant type. | Ensure your request uses `grant_type=client_credentials` and includes both `client_id` and `client_secret` as form fields. |
| **unsupported_grant_type** | `Unsupported grant type: password` | The flow is incorrect for this app. | Use only `grant_type=client_credentials` for service apps. Do not use password or authorization code grants. |
| **invalid_request** | `Missing client_id` or `Missing client_secret` | Required parameters were omitted from the request. | Include both `client_id` and `client_secret` in the POST body of the token request. |

---

## Learn More

- [**Overview**](overview.md)  
  When and why to use the Client Credentials flow.

- [**Step-by-Step Example**](service-apps-step-by-step.md)  
  Minimal sequence to obtain a token and call the APIs.

- [**Token Request and Response**](service-apps-token.md)  
  Request parameters and example responses.

- [**Trusted Applications and Access Control**](../../how-apps-connect/trusted-apps-access.md)  
  Configure System User and scopes properly for background apps.
