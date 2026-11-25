# Docs for agents

Use this page to give your coding agent the ERP.net documentation. You can ask it to build an integration or work with data in your ERP.net instance.

## Give your agent the docs

1. **Share a documentation link.** Give your agent [https://docs.erp.net/dev/llms.txt](../llms.txt) so it can find the guides for your task. To share one specific guide, open **View as Markdown** in that guide's footer and give the agent the URL it opens.
2. **Describe what you want done.** Tell the agent what to do and what result to report. For tasks that use your ERP.net data, specify which instance it should use. You can start with an [example prompt](#example-prompts).

## Choose a guide for your task

Share a specific guide when you already know what the agent needs to work on.

### Authentication

| What your app needs to do | Guide to share |
|---------------------------|----------------|
| Run a background job as the app's configured System User | **Client Credentials.** Configure `SystemUserAllowed` and `SystemUser`. Send the app's client ID and secret with `grant_type=client_credentials`. [Application setup](../auth/flows/client-credentials/service-apps-step-by-step.md#prerequisites) and [token request](../auth/flows/client-credentials/service-apps-token.md) |
| Call the API as an active ERP.net Agent | **Token Exchange.** Use a confidential app's client ID and secret and pass the Agent's login in the subject JWT. [Supported use case and JWT claims](../auth/flows/token-exchange-use-cases.md) and [HTTP example](../auth/samples/token-exchange-agent-example.md) |
| Sign in a person and call the API on their behalf | **Authorization Code with PKCE.** Use browser sign-in and exchange the code with the PKCE verifier. [Sign-in walkthrough](../auth/flows/auth-code/interactive-apps-step-by-step.md) |

Use the target instance's Identity discovery document to find `token_endpoint`. Request the scopes needed for the task and check the scopes granted in the token response. [Identity discovery](../auth/concepts/identity-server.md#1-discovery-endpoint) and [API scopes](../auth/concepts/scopes.md)

### Domain API

- **Build a request.** Find `ODataServiceRoot` for `DomainAPI` in site discovery. Send `Authorization: Bearer <access_token>` on metadata and data requests. Use the target instance's `$metadata` to check entity sets, fields, and relationships. [Site discovery](../building-apps/concepts/application-server-endpoints.md), [request URLs](../domain-api/querying-data/url-components.md), and [standard entities](https://docs.erp.net/model/entities/)
- **Filter data.** Combine conditions with `and`. Use `in` for alternative values on a field that supports it. General `or` expressions are unsupported. The filter guide also explains functions such as `equalnull`. [Supported query options](../domain-api/querying-data/query-options/index.md) and [filter guide](../domain-api/querying-data/query-options/filter.md)
- **Read more than one page.** Set `$top` and follow each `@odata.nextLink` unchanged. [Paging results](../domain-api/querying-data/paging.md)
- **Work with instance-specific fields.** Find them in the current model and select them by name. [Custom properties](../domain-api/common-tasks/stored-attributes.md) and [calculated attributes](../domain-api/common-tasks/calculated-attributes.md)
- **Call an action or function.** Use `POST` for actions and `GET` for functions. Check `$metadata` for the signature and whether the operation is bound to an entity. [Domain API operations](../domain-api/operations/index.md)
- **Save several writes together.** Use a transaction and send its ID with each request. [Domain API transactions](../domain-api/data-manipulation/transactions.md)

For general setup, see [Building Apps](../building-apps/overview.md). For table data and reporting, see the [Table API](../table-api/index.md).

## Example prompts

Use these prompts with an agent that has access to the target ERP.net instance. Replace the placeholders and attach any file the prompt refers to.

### Find orders that need a warehouse assigned

```text
Start at https://docs.erp.net/dev/llms.txt. Use the configured Client
Credentials application to query sales orders in <instance>.

Find orders dated in <date range> that are FirmPlanned or Released and
have no store set. Follow pagination and return a table for the dispatch
team with the order number, date, and customer for each match.
```

### Check product records as an Agent

```text
Start at https://docs.erp.net/dev/llms.txt. In <instance>, use Token
Exchange with the configured confidential app to get a read access
token for the Agent CatalogScout.

Use that token to look up the part numbers in my attached list. Return
the product name and group name for each part number, and flag any
missing products so I can clean up the catalog.
```

### Apply product color corrections from a CSV

```text
Start at https://docs.erp.net/dev/llms.txt. Apply the color corrections
from my attached CSV in <instance>. The file has PartNumber and Color
columns.

Match products by PartNumber and look up the custom property with code
color in the instance model. Update that field for each unique match.
Read back the saved values and report the changes and any unresolved rows.
```

### Correct a draft sales order before release

```text
Start at https://docs.erp.net/dev/llms.txt. In <instance>, set the
document date on draft sales order <order ID> to <new date> and the
quantity on line <line ID> to <new quantity and unit>.

Apply both edits in one front-end transaction. Commit if both writes
succeed and roll back if either fails. Report the outcome and, after a
successful commit, verify the saved date and quantity.
```
