# Additional Data JSON

Aggregate root entities have an associated **Extensible Data Object (EDO)**: a system record that holds common data about the aggregate in addition to the entity's primary attributes. `AdditionalDataJson` is exposed from this associated record as an attribute of the aggregate root in the Domain API.

`AdditionalDataJson` is an independent extension point for per-record integration data. Use it whenever an application needs to persist unmodeled state with an @@name record—whether the record originates in an external system, in @@name itself, or in an automation that has no external identifier.

The attribute is not exposed on aggregate child entities.

For the underlying concept, see [Extensible Data Objects](https://docs.erp.net/tech/advanced/data-objects/edo.html).

## Purpose and limitations

The field is similar to a small, per-record **NoSQL-like JSON document slot**. An integration can use it to store a JSON object with arbitrary keys and nested values without changing the @@name relational data model.

For example, an external product catalog can persist synchronization metadata that has no corresponding standard Product attribute:

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

The system does not provide JSON-path queries, filtering, sorting, grouping, indexing, or partial updates and merges of individual JSON properties.

Use `AdditionalDataJson` for data owned and interpreted by an integration. When the data needs to be modeled, visible to users, or used in filters and reports, define [stored attributes (custom properties)](../common-tasks/stored-attributes.md) instead.

## Integration ownership convention

`AdditionalDataJson` is shared by all applications and services that work with the same record. Store a JSON root object and give each application or service its own top-level object within it. Name the object after the external application or service, or after the functional domain that owns the data. Use a stable logical identifier that clearly identifies its owner.

```json
{
  "erpnet-retail-pos": {
    "listingId": "P-1042",
    "status": "approved"
  },
  "calendarSync": {
    "lastSynchronizedUtc": "2026-08-19T10:15:00Z"
  }
}
```

In this example, ERP.net Retail POS owns only the `erpnet-retail-pos` object and the calendar synchronization service owns only the `calendarSync` object. An application must not use or overwrite another application's object unless the integrations explicitly agree to share it.

Because `PATCH` replaces the complete `AdditionalDataJson` value, update it using read-modify-write:

1. Read the current value.
2. Parse the JSON root object.
3. Add, replace, or remove only the application-owned top-level object.
4. Serialize and PATCH the complete value, preserving the objects owned by other applications.

For automated clients and coding agents: do not infer ownership from the contents of an object. Use its top-level name, and preserve unknown top-level objects exactly as received.

## Choose the right mechanism

| Requirement | Use |
| --- | --- |
| Persist an integration-owned, per-record JSON document | `AdditionalDataJson` |
| Add a business field that users configure or edit | [Stored attributes (custom properties)](../common-tasks/stored-attributes.md) |
| Filter, sort, group, report on, or relate the value | A modeled attribute, a custom property, or a dedicated entity |

For automated clients and coding agents: do not infer a schema from existing JSON values. Treat each top-level object as owned by its named integration, preserve unknown objects when updating the document, and coordinate schema changes with that integration's owner.

## Read the value

`AdditionalDataJson` is delay-loaded. Select it explicitly only when the integration needs the payload:

```http
GET /api/domain/odata/General_Products_Products?$top=10&$select=Id,PartNumber,AdditionalDataJson HTTP/1.1
```

The value is an OData `string`, so the JSON document is returned as a JSON-escaped string:

```json
{
  "Id": "00000000-0000-0000-0000-000000000000",
  "PartNumber": "PRD-1042",
  "AdditionalDataJson": "{\"sourceStatus\":\"approved\",\"labels\":[\"priority\"]}"
}
```

Parse the string as JSON after receiving it. Do not include `AdditionalDataJson` in broad list queries unless the consumer needs it; retrieving it performs a separate secured read for each requested entity.

## Update the value

Use `PATCH` to replace the complete value. JSON-encode the object as the value of the OData string property:

```http
PATCH /api/domain/odata/General_Products_Products(00000000-0000-0000-0000-000000000000) HTTP/1.1
Content-Type: application/json

{
  "AdditionalDataJson": "{\"sourceStatus\":\"approved\",\"labels\":[\"priority\"]}"
}
```

To clear the data, send an empty value:

```json
{
  "AdditionalDataJson": ""
}
```

Sending `null` does not clear the value. `AdditionalDataJson` is delay-loaded, so `null` means that no value has been supplied by the request and the stored value is kept.

The server validates the value on client commit. Its length must not exceed **32,000 characters**; otherwise the request fails with [R101790](https://docs.erp.net/model/business-rules/R101790.html), *Additional Data JSON Maximum Length*. The value must also be a valid JSON object; otherwise the request fails with [R101839-1](https://docs.erp.net/model/business-rules/R101839-1.html), *Additional Data JSON Validation*.

## Design guidance

- `AdditionalDataJson` is optional. Omit it when no integration data is needed; sending `null` leaves the stored value unchanged.
- Treat the JSON document as integration-owned. A PATCH replaces the whole value; it does not merge individual JSON properties.
- Keep a stable schema and version it inside the JSON when it may evolve.
- Do not use this field for lookup, filtering, sorting, reporting, or relationship data. Use modeled attributes, [stored attributes (custom properties)](../common-tasks/stored-attributes.md), or dedicated entities for those purposes.
- Do not store secrets or credentials. The value is entity data and is available to callers with permission to read the entity.

For the EDO concept and functional guidance, see [Additional Data JSON](https://docs.erp.net/tech/advanced/data-objects/additional-data-json.html).
