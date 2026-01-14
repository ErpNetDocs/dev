# Incremental pull by last update time

## Overview

Some integrations need to periodically pull only the entities that have changed since the last successful synchronization run (incremental sync).

Starting from v26.2, aggregate root entities expose a calculated attribute `AggregateLastUpdateTimeUtc`, which can be used to implement incremental pulls via Domain API.

`AggregateLastUpdateTimeUtc` is a UTC timestamp.

## Getting started

### 1) Select the last update timestamp

To retrieve the value, explicitly select `AggregateLastUpdateTimeUtc`:

```http
General_Products_Products?$top=10&$select=Id,AggregateLastUpdateTimeUtc,PartNumber
```

### 2) Pull updates since a watermark (with overlap)

A robust incremental sync typically uses a small overlap window to avoid missing changes around the watermark boundary.

- Store `watermarkUtc` (the last successfully processed point in time).
- Choose an overlap duration (e.g. 2 minutes).
- For the next run, request changes from `fromUtc = watermarkUtc - overlap`.

Example:

- `watermarkUtc = 2023-06-09T10:00:00.000Z`
- `overlap = 2 minutes`
- `fromUtc = 2023-06-09T09:58:00.000Z`

```http
General_Products_Products?
  $count=true&
  $top=2000&
  $select=Id,AggregateLastUpdateTimeUtc,PartNumber&
  $filter=AggregateLastUpdateTimeUtc ge 2023-06-09T09:58:00.000Z
```

Because the filter intentionally goes a bit back in time, the result may contain duplicates that you have already processed in previous runs (or earlier pages of the same run). Your sync logic must be idempotent (see “Handling duplicates” below).

### 3) Page through results using @odata.nextLink

When `$top` is provided, Domain API returns `@odata.nextLink`.

Recommended client behavior:
- Treat `@odata.nextLink` as an opaque URL.
- Keep requesting it until the server stops returning `@odata.nextLink`.

This paging approach is required for large result sets.

## Concepts

### AggregateLastUpdateTimeUtc

- `AggregateLastUpdateTimeUtc` is available for entities that are **aggregate roots**.
- The value is calculated from the Extensible Data Object (EDO) change tracking (i.e. it represents the aggregated “last update moment” for the whole aggregate).

This makes it suitable for incremental pull scenarios where any change in the aggregate tree should be considered an update.

### Using overlap (recommended)

Overlap reduces the chance of missing changes around the watermark boundary (latency, clock skew, boundary equality, retries).

Typical approach:
- Query from `watermarkUtc - overlap` using `ge`
- Deduplicate and process idempotently
- Advance watermark only after a successful full run

### Handling duplicates (required when using overlap)

When you use overlap (and often even without overlap, depending on the chosen boundary operator), duplicates are expected. The sync code should be idempotent.

A practical approach is to store a per-entity checkpoint keyed by `Id`:
- `lastAppliedUtcById[Id] = last processed AggregateLastUpdateTimeUtc for this entity`

Then, for every received item:
- If `item.AggregateLastUpdateTimeUtc <= lastAppliedUtcById[item.Id]` → skip (duplicate/older)
- Else → upsert + update the per-entity checkpoint

Pseudocode:

```text
maxSeenUtc = watermarkUtc

for each page (following @odata.nextLink):
  for each item:
    lastApplied = lastAppliedUtcById[item.Id] (or null)

    if lastApplied != null and item.AggregateLastUpdateTimeUtc <= lastApplied:
        continue

    upsert(item)
    lastAppliedUtcById[item.Id] = item.AggregateLastUpdateTimeUtc
    maxSeenUtc = max(maxSeenUtc, item.AggregateLastUpdateTimeUtc)

watermarkUtc = maxSeenUtc  // advance watermark only after successful commit of the whole run
```

### Paging: $skiptoken vs $skip

When a query contains `$top`, Domain API returns `@odata.nextLink`:

- If the query targets an entity set backed by a **table** (not a view) and **no `$orderby` is specified**, `@odata.nextLink` uses:

`$skiptoken={NEXT_ID}`

This is keyset paging (based on `Id`) and avoids the instability of offset paging when inserts/deletes happen between page requests.

Example:

```json
"@odata.nextLink": "General_Products_Products?$top=10&$select=Id&$count=true&$skiptoken=3f253c9a-5936-e311-81cb-00155d001f00"
```

- If the query is against a **view** or if **`$orderby` is specified**, `@odata.nextLink` is generated using `$skip` (offset paging).

## Troubleshooting

### I don’t see AggregateLastUpdateTimeUtc

The attribute is available only for entities that are aggregate roots. If the entity set you are querying is not an aggregate root, the attribute will not be exposed.

### I don’t get $skiptoken in @odata.nextLink

`$skiptoken` is returned only when:
- `$top` is specified,
- no `$orderby` is specified,
- the entity set is backed by a table (not a view).

If any of these conditions is not met, `@odata.nextLink` uses `$skip`.

### My incremental sync produces duplicates

This is expected when using overlap (`fromUtc = watermarkUtc - overlap`). Ensure your sync logic is idempotent and deduplicates by (`Id`, `AggregateLastUpdateTimeUtc`) as described above.