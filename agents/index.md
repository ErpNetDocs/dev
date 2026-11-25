# Docs for agents

Use these guides when asking a coding agent to configure authentication or build requests for ERP.net APIs.

## Start with the documentation index

Give your agent the [Developer Docs index](../llms.txt) and a specific task. The index points it to the Markdown versions of the relevant guides.

```text
Start at https://docs.erp.net/dev/llms.txt. Find the Client Credentials setup
and token request for a read-only background integration. Then show an
authenticated Domain API request for a page of up to 10 products. Cite the
ERP.net guides you used.
```

## Obtain an access token

Choose the flow based on whose permissions the API calls need.

| API identity | Flow and request guide |
|--------------|------------------------|
| The Trusted Application's configured System User | **Client Credentials.** Enable `SystemUserAllowed` and assign the app's `SystemUser`. Authenticate with the app's client ID and secret using `grant_type=client_credentials`. [Application setup](../auth/flows/client-credentials/service-apps-step-by-step.md#prerequisites) and [token request](../auth/flows/client-credentials/service-apps-token.md) |
| An active Agent | **Token Exchange.** Authenticate with the confidential app's client ID and secret. Pass the Agent's login in the subject JWT. [Supported use cases and JWT claims](../auth/flows/token-exchange-use-cases.md) and [Agent HTTP sample](../auth/samples/token-exchange-agent-example.md) |
| A user who signs in interactively | **Authorization Code.** Start with browser-based sign-in and redeem the authorization code at the token endpoint. [Authorization Code walkthrough](../auth/flows/auth-code/interactive-apps-step-by-step.md) |

Find the token URL in the `token_endpoint` field of the target instance's [Identity discovery document](../auth/concepts/identity-server.md#1-discovery-endpoint). Request the [API scopes](../auth/concepts/scopes.md) needed for the task and check the scopes granted in the response.

## Build a Domain API request

1. Find `ODataServiceRoot` for `DomainAPI` in the target instance's [site discovery response](../building-apps/concepts/application-server-endpoints.md).
2. Send `Authorization: Bearer <access_token>` on metadata and data requests. Fetch `$metadata` relative to the service root to confirm the instance's entity sets, fields, and relationships. See the [Domain Model](https://docs.erp.net/model/entities/) for standard entities.
3. Combine the service root, resource path, and [supported query options](../domain-api/querying-data/query-options/index.md). Use the names from the instance metadata. In one `$filter`, join conditions with `and` and use `in` for alternative values on a field that supports it. General `or` expressions are unsupported. See the [Domain API filter guide](../domain-api/querying-data/query-options/filter.md) for field support and functions such as `equalnull`. [OData request URLs](../domain-api/querying-data/url-components.md)
4. For multi-page reads, set `$top` and follow the returned `@odata.nextLink` unchanged to fetch the next page. [Paging results](../domain-api/querying-data/paging.md)

Use `POST` for actions and `GET` for functions. Check the instance metadata for the operation's signature and whether it is bound to an entity. [Domain API operations](../domain-api/operations/index.md)

For instance-specific fields, check the current model and select the fields by name. [Custom properties](../domain-api/common-tasks/stored-attributes.md) explains how to set or filter stored values, and [Calculated attributes](../domain-api/common-tasks/calculated-attributes.md) shows how to read computed values. When several writes must succeed together, use a [Domain API transaction](../domain-api/data-manipulation/transactions.md) and send its ID with each request.

For general setup, see [Building Apps](../building-apps/overview.md). For table data and reporting, see the [Table API](../table-api/index.md).

## Read a page as Markdown

The page footer links to the documentation index and to **View as Markdown**, which opens the original Markdown file. An agent can fetch that URL directly. Use the rendered HTML for resolved cross-references and generated lists.
