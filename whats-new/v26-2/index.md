# Version 26.2

- [**Reference Access Tokens (PAT, SAT)**](../../auth/tokens/reference-access-tokens.md)  
  Now supported for long-lived, manually issued access - ideal for automation, scripts, and service integrations.
  
- [**ExecuteScript (Domain API)**](../../domain-api/execute-script.md)  
  Execute raw JavaScript server-side via an unbound OData action, with full access to the Domain Model for querying and modifying data.

- [**Paging via @odata.nextLink (Domain API)**](../../domain-api/querying-data/paging.md)  
  Queries that include `$top` now return `@odata.nextLink` for server-driven paging. Depending on the query shape, the continuation link may use `$skiptoken` (keyset paging) or `$skip` (offset paging) - clients should always follow `@odata.nextLink` as an opaque URL.

- [**AggregateLastUpdateTimeUtc (Domain API)**](../../domain-api/data-sync/incremental-pull.md)  
  Aggregate root entities now expose `AggregateLastUpdateTimeUtc`, enabling incremental pull scenarios by filtering entities updated after a given UTC timestamp.

