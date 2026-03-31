# Version 26.2

- [**Reference Access Tokens (PAT, SAT)**](../../auth/tokens/reference-access-tokens.md)  
  Now supported for long-lived, manually issued access - ideal for automation, scripts, and service integrations.
  
- [**ExecuteScript (Domain API)**](../../domain-api/execute-script.md)  
  Execute raw JavaScript server-side via an unbound OData action, with full access to the Domain Model for querying and modifying data.

- [**Paging via @odata.nextLink (Domain API)**](../../domain-api/querying-data/paging.md)  
  Queries that include `$top` now return `@odata.nextLink` for server-driven paging. Depending on the query shape, the continuation link may use `$skiptoken` (keyset paging) or `$skip` (offset paging) - clients should always follow `@odata.nextLink` as an opaque URL.

- [**Incremental pull synchronization**](../../domain-api/data-sync/incremental-pull.md)  
  Aggregate root entities now expose `AggregateLastUpdateTimeUtc`, enabling incremental pull scenarios by filtering entities updated after a given UTC timestamp.

- [**Import**](../../domain-api/data-manipulation/import.md)
  ERP.net Domain API defines an Import endpoint which can be used to import multiple entities at once.

- **WebClient**: Main menu extensions registry. See [Registering Extensions](../../web-client/registering-extensions.md).

- [**ERP.net Marketplace onboarding webhooks**](../../marketplace/index.md)
  Applications installed via the @@name Marketplace can now optionally receive a secure `HTTP POST` callback to automate onboarding. The lifecycle event is delivered in a cryptographically signed envelope.

- **Default track changes levels** - In **v26.2**, all entities have a minimum default tracking level of **Level 1**. Entities with higher predefined tracking levels keep their existing behavior.
  Related topics: [Track Changes](https://docs.erp.net/tech/advanced/data-objects/track-changes.html), [Default Tracking Levels](https://docs.erp.net/tech/advanced/data-objects/default-tracking-levels.html)

- **DocumentVersioningSystem configuration option** - In **v26.2**, the [DocumentVersioningSystem](https://docs.erp.net/tech/reference/config-options-reference.html#42-documentversioningsystem-obsolete-as-of-v262) configuration option becomes obsolete. The system now always uses **Track Changes**, and the legacy **VH** document versioning system is discontinued.

- **New Visual Import Tool** - The Domain API site  provides a **Visual Import Tool** at the `/api/domain/import` endpoint, for example: `https://testdb.my.erp.net/api/domain/import`. The tool allows you to work with import payloads interactively in the browser. You can type, paste, edit, and execute import JSON directly in the page.

- **Domain API**: `Quantity` and `Amount` can now be set without first providing the corresponding measurement unit or currency, as long as the dependent reference is stored in the same entity. For details, see [Property Dependencies and Update Order](../domain-api/data-manipulation/update-order.md).