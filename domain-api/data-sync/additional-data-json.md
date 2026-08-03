# Additional Data JSON

Aggregate root entities have an associated **Extensible Data Object (EDO)**: a system record that holds common data about the aggregate in addition to the entity's primary attributes. `AdditionalDataJson` is exposed from this associated record as an attribute of the aggregate root in the Domain API.

`AdditionalDataJson` is an independent extension point for per-record integration data. Use it whenever an application needs to persist unmodeled state with an @@name record—whether the record originates in an external system, in @@name itself, or in an automation that has no external identifier.

The attribute is not exposed on aggregate child entities.

For the underlying concept, see [Extensible Data Objects](https://docs.erp.net/tech/advanced/data-objects/edo.html).

## Purpose and limitations

The field is similar to a small, per-record **NoSQL-like payload slot**. An integration can use it to store a JSON object with arbitrary keys and nested values without changing the @@name relational data model.

For example, an external product catalog can retain synchronization metadata that has no corresponding standard Product attribute:

```json
{
  "catalog": {
    "listingId": "P-1042",
    "status": "approved"
  },
  "synchronization": {
    "etag": "W/\"43\"",
    "lastSeenUtc": "2026-07-29T10:15:00Z"
  },
  "labels": ["priority", "seasonal"]
}
```

It can also be used without an external identity. For example, an internal automation can store the state of an enrichment workflow for a customer:

```json
{
  "enrichment": {
    "profile": "customer-risk-v2",
    "completedUtc": "2026-07-29T10:15:00Z",
    "score": 0.82
  }
}
```

@@name persists `AdditionalDataJson` as a plain string. It does not parse, validate, query, index, or partially update its contents. The field name does not guarantee that a value is valid JSON or even a JSON object. Clients must treat the received value as untrusted text and parse it defensively.

The system does not provide:

- a declared schema or data types for its JSON properties;
- JSON-path queries, filtering, sorting, grouping, or indexing;
- server-side validation of the JSON structure; or
- partial updates or merges of individual JSON properties.

Use `AdditionalDataJson` for data owned and interpreted by an integration. When the data needs to be modeled, typed, validated, visible to users, or used in filters and reports, define [stored attributes (custom properties)](../common-tasks/stored-attributes.md) instead.

## Choose the right mechanism

| Requirement | Use |
| --- | --- |
| Persist an integration-owned, per-record payload (often JSON) | `AdditionalDataJson` |
| Add a business field that users configure or edit | [Stored attributes (custom properties)](../common-tasks/stored-attributes.md) |
| Filter, sort, group, report on, validate, or relate the value | A modeled attribute, a custom property, or a dedicated entity |

For automated clients and coding agents: do not infer a schema from existing JSON values. Treat the JSON document as integration-owned, preserve unknown properties when updating it, and coordinate schema changes with the owner of that integration.

## Read the value

`AdditionalDataJson` is delay-loaded. Select it explicitly only when the integration needs the payload:

```http
GET /api/domain/odata/General_Products_Products?$top=10&$select=Id,PartNumber,AdditionalDataJson HTTP/1.1
```

The value is an OData `string`. When its stored content is JSON, it is returned as a JSON-escaped string:

```json
{
  "Id": "00000000-0000-0000-0000-000000000000",
  "PartNumber": "PRD-1042",
  "AdditionalDataJson": "{\"sourceStatus\":\"approved\",\"labels\":[\"priority\"]}"
}
```

If the integration expects JSON, parse the string with error handling and validate the expected structure before using it. Do not include `AdditionalDataJson` in broad list queries unless the consumer needs it; retrieving it performs a separate secured read for each requested entity.

## Update the value

Use `PATCH` to replace the complete value. When storing JSON, JSON-encode the object as the value of the OData string property:

```http
PATCH /api/domain/odata/General_Products_Products(00000000-0000-0000-0000-000000000000) HTTP/1.1
Content-Type: application/json

{
  "AdditionalDataJson": "{\"sourceStatus\":\"approved\",\"labels\":[\"priority\"]}"
}
```

To clear the data, send `null`:

```json
{
  "AdditionalDataJson": null
}
```

The server validates the value on client commit. Its length must not exceed **32,000 characters**; otherwise the request fails with [R101790](https://docs.erp.net/model/business-rules/R101790.html), *Additional Data JSON Maximum Length*.

## Design guidance

- `AdditionalDataJson` is optional. Omit it or send `null` when no integration data is needed.
- Treat the value as integration-owned. A PATCH replaces the whole value; it does not merge individual JSON properties when the value contains JSON.
- If the integration stores JSON, keep a stable schema and version it inside the JSON when it may evolve.
- Do not use this field for lookup, filtering, sorting, reporting, or relationship data. Use modeled attributes, [stored attributes (custom properties)](../common-tasks/stored-attributes.md), or dedicated entities for those purposes.
- Do not store secrets or credentials. The value is entity data and is available to callers with permission to read the entity.

For the EDO concept and functional guidance, see [Additional Data JSON](https://docs.erp.net/tech/advanced/data-objects/additional-data-json.html).
