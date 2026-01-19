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

### 2) First synchronization run (full initial sync)

On the first run there is no watermark. If you want a complete local copy, the recommended approach is to do a full initial pull and only then start incremental pulls.

Recommended approach (full initial sync):

1. Perform a full pull (no filter on `AggregateLastUpdateTimeUtc`).
2. Always use paging (`$top`) and follow `@odata.nextLink` until completion.
3. Process each item idempotently (upsert).
4. While processing all pages, track `maxSeenUtc` = the maximum `AggregateLastUpdateTimeUtc` value you have successfully processed.
5. Persist the watermark only after the whole initial run finishes successfully:
   - `watermarkUtc = maxSeenUtc`

This ensures that the first incremental run starts from a safe point in time.

Alternative (start “from now”, no history):

If you do not need historical data, you can initialize:

- `watermarkUtc = nowUtc`

and start incremental pulls from `watermarkUtc - overlap`. This starts the sync from “current time”, but it does not load existing/older records.

### 3) Incremental pulls (watermark + overlap)

A robust incremental sync uses a small overlap window to avoid missing changes around the watermark boundary.

- Store `watermarkUtc` (the last successfully processed point in time).
- Choose an overlap duration.
- For the next run, request changes from `fromUtc = watermarkUtc - overlap`.

> NOTE
> Domain API incremental sync uses `$filter=... ge ...`, so using an overlap window and idempotent processing is required.
> Recommended overlap:
> - 5 minutes (default)
> - 30 minutes (heavy workloads / large transactions)
> - up to 1 hour (worst-case safety window)

> NOTE
> For synchronization scenarios, use `$top=1000` and do not use a value greater than 1000.
> If the payload per row is large (many selected fields and/or `$expand`), consider using a smaller `$top`.

Example:

- `watermarkUtc = 2023-06-09T10:00:00.000Z`
- `overlap = 5 minutes`
- `fromUtc = 2023-06-09T09:55:00.000Z`

```http
General_Products_Products?
  $top=1000&
  $select=Id,AggregateLastUpdateTimeUtc,PartNumber&
  $filter=AggregateLastUpdateTimeUtc ge 2023-06-09T09:55:00.000Z
```

Because the filter intentionally goes a bit back in time, the result may contain duplicates that you have already processed in previous runs (or earlier pages of the same run). Your sync logic must be idempotent (see “Handling duplicates” below).

### 4) Page through results using @odata.nextLink

When `$top` is provided, Domain API returns `@odata.nextLink`.

Recommended client behavior:
- Treat `@odata.nextLink` as an opaque URL.
- Keep requesting it until the server stops returning `@odata.nextLink`.

This paging approach is required for large result sets.

For more details, see [Paging results ($top and @odata.nextLink)](../querying-data/paging.md).

## Concepts

### AggregateLastUpdateTimeUtc

- `AggregateLastUpdateTimeUtc` is available for entities that are **aggregate roots**.
- The value is calculated from the Extensible Data Object (EDO) change tracking (i.e. it represents the aggregated “last update moment” for the whole aggregate).

This makes it suitable for incremental pull scenarios where any change in the aggregate tree should be considered an update.

### Using overlap

Overlap reduces the chance of missing changes around the watermark boundary (latency, clock skew, boundary equality, retries).

Typical approach:
- Query from `watermarkUtc - overlap` using `ge`
- Deduplicate and process idempotently
- Advance watermark only after a successful full run

### Handling duplicates (required)

Duplicates are expected when using overlap. The sync code should be idempotent.

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

## Troubleshooting

### I don’t see AggregateLastUpdateTimeUtc

The attribute is available only for entities that are aggregate roots. If the entity set you are querying is not an aggregate root, the attribute will not be exposed. Also make sure that the AggregateLastUpdateTimeUtc is explicitly included in `$select` clause.

### My incremental sync produces duplicates

This is expected when using overlap (`fromUtc = watermarkUtc - overlap`). Ensure your sync logic is idempotent and deduplicates by (`Id`, `AggregateLastUpdateTimeUtc`) as described above.

### I keep reprocessing the same rows on every run

Common causes:

- The overlap window is too large for the update rate of the dataset.
- The sync advances the watermark incorrectly (e.g. persisting `watermarkUtc` before the whole run has completed successfully).
- The watermark is not persisted, so every run starts from an old value.

Recommended approach:
- Persist `watermarkUtc` only after a successful full run.
- Consider reducing the overlap window if duplicates are too frequent.

### I suspect that some updates are missing

Common causes:

- Overlap is too small for your worst-case transaction duration / processing delays.
- The sync persists `watermarkUtc` even when the run fails halfway.

Recommended approach:
- Increase overlap (e.g. from 5 minutes to 30 minutes, or up to 1 hour for worst-case safety).
- Persist the watermark only after successful commit of the whole run.
