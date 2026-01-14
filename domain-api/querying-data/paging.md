# Paging results ($top and @odata.nextLink)

## Overview

When retrieving large datasets through the Domain API, clients should page results instead of requesting “everything at once”.

Paging is enabled by providing `$top`. When `$top` is present, the response may include `@odata.nextLink`, which the client must follow to retrieve the next page.

> NOTE
> `@odata.nextLink` is an opaque (transparent) server-generated URL. Clients must not parse it, modify it, or attempt to “recreate” it. The only correct behavior is to request the URL exactly as returned by the server.

## Getting started

### 1) Pick a page size (recommended for synchronizations)

For synchronization / data replication scenarios, a good default is:

- `$top=1000` (recommended)
- do not use a value greater than `1000`

The goal is to keep each request predictable in duration and payload size, while still being efficient enough for large pulls. If the payload per row is large (many selected fields or expanded references), consider using a smaller `$top`.

> NOTE
> Recommended for synchronizations: use `$top=1000` and do not use a value greater than 1000.

Example:

```http
General_Products_Products?$top=1000&$select=Id,PartNumber
```

### 2) Request the first page and follow @odata.nextLink

Start with the initial query (including `$top`). If the server returns `@odata.nextLink`, request it to get the next page. Continue until `@odata.nextLink` is no longer returned.

Recommended client behavior:

- Treat `@odata.nextLink` as an opaque URL (do not parse or modify it).
- Keep requesting it until the server stops returning `@odata.nextLink`.

## Concepts

### Why $top is required

`$top` serves two purposes:

1) It limits the payload size of a single response.
2) It enables server-driven paging via `@odata.nextLink` so the client can safely fetch the whole result set page-by-page.

For synchronization workloads, server-driven paging is the recommended approach for pulling large datasets reliably.

### Why $top=1000 is a good default for synchronization

In sync scenarios, clients often need to read many rows (sometimes millions) and must balance:

- efficiency (fewer requests),
- stability (avoid very large responses),
- resilience (retries should be cheap),
- consistent throughput over time.

Using `$top=1000` is a practical default because it typically provides good throughput without making individual requests too heavy. It also makes retry logic simpler: if a request fails, you retry a relatively small batch.

If you select many fields, use `$expand`, or the entity payload is large, using a smaller `$top` may reduce memory usage and response size per request.

> NOTE
> Always page using `$top` and `@odata.nextLink` for synchronization. Avoid designs that depend on “single huge response”.

### @odata.nextLink is opaque (clients must not analyze it)

Clients must treat `@odata.nextLink` as a server-generated continuation link.

Whether the link contains `$skiptoken` (keyset paging) or `$skip` (offset paging) is an internal server decision and does not change the client algorithm: always follow `@odata.nextLink` exactly as returned.

> NOTE
> Do not branch sync logic based on whether the link contains `$skiptoken` or `$skip`.

### Server behavior: $skiptoken vs $skip (for reference)

Depending on the query shape, Domain API may generate `@odata.nextLink` using different paging strategies.

- If the query targets an entity set backed by a **table** (not a view) and **does not specify `$orderby`**, `@odata.nextLink` may contain:

  - `$skiptoken={NEXT_ID}`

  This is keyset paging based on the default ordering by `Id`.

  Example:

  ```json
  "@odata.nextLink": "General_Products_Products?$top=1000&$select=Id&$count=true&$skiptoken=3f253c9a-5936-e311-81cb-00155d001f00"
  ```

- If the query is against a **view** or if **`$orderby` is specified**, `@odata.nextLink` may be generated using `$skip` (offset paging), e.g.:

  - `...&$skip={prevSkip + top}`

This is an implementation detail. Clients must not rely on a specific strategy and must not branch logic based on whether the link contains `$skiptoken` or `$skip`.

### Implementation detail (for expectations)

For requests with `$top`, the server may internally fetch one extra row to determine whether there is a next page and to construct `@odata.nextLink`.

## Troubleshooting

### I don’t get @odata.nextLink

`@odata.nextLink` is returned only when `$top` is specified and there are more results after the current page.

### I don’t get $skiptoken (I get $skip instead)

`$skiptoken` may be returned only when:
- `$top` is specified,
- no `$orderby` is specified,
- the entity set is backed by a table (not a view).

If any of these conditions is not met, `@odata.nextLink` may use `$skip`.
