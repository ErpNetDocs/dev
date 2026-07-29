# External identifiers

`ExternalId` and `ExternalSystem` are optional attributes on aggregate root entity sets. They associate an @@name record with the record that represents it in another system.

The attributes are independent of `AdditionalDataJson`. Use external identifiers to identify and locate a synchronized record; use [Additional Data JSON](additional-data-json.md) for supplementary integration data that does not need to be queried.

## Attributes

| Attribute | Purpose | Maximum length |
| --- | --- | --- |
| `ExternalId` | Identifier of the record in the external system. | 254 characters |
| `ExternalSystem` | Optional name or identifier of the external system that qualifies `ExternalId`. | 64 characters |

For example, a product imported from Contoso Commerce can be identified as:

```json
{
  "ExternalId": "P-1042",
  "ExternalSystem": "Contoso Commerce"
}
```

The attributes are exposed only on aggregate root entity sets.

## Microsoft Graph and Outlook calendar example

An @@name Activity can be linked to an Outlook calendar event synchronized through Microsoft Graph. In this case, use the Graph event's `iCalUId` as the external identifier, rather than the mailbox-specific event `id`.

`iCalUId` identifies the meeting across mailbox copies, so the same meeting can resolve to the same Activity when it is encountered through the organizer's or an attendee's calendar.

The built-in Microsoft 365 calendar synchronization uses the following identifier pair:

```json
{
  "ExternalId": "040000008200E00074C5B7101A82E00800000000A1B2C3D4E5F60708090000000000000001000000001234567890ABCDEF1234567890ABCDEF",
  "ExternalSystem": "Office365-iCalUId"
}
```

An integration can then locate the Activity for a Graph event by its `iCalUId`:

```http
GET /api/domain/odata/General_Activities_Activities?$select=Id,Subject&$filter=ExternalId eq '040000008200E00074C5B7101A82E00800000000A1B2C3D4E5F60708090000000000000001000000001234567890ABCDEF' and ExternalSystem eq 'Office365-iCalUId' HTTP/1.1
```

Use a distinct `ExternalSystem` value for each identifier scheme. This prevents an `iCalUId` from being confused with a Microsoft Graph event `id` or an identifier issued by another integration.

## Query by external identity

Both attributes support filtering. Use them to find the @@name record that corresponds to an external record:

```http
GET /api/domain/odata/General_Products_Products?$select=Id,PartNumber,ExternalId,ExternalSystem&$filter=ExternalId eq 'P-1042' and ExternalSystem eq 'Contoso Commerce' HTTP/1.1
```

`ExternalSystem` is optional. Omit it from the filter when the external ID is sufficient for the integration scenario.

## Create or update

Set the attributes in a create or `PATCH` request together with the entity's regular data:

```http
PATCH /api/domain/odata/General_Products_Products(00000000-0000-0000-0000-000000000000) HTTP/1.1
Content-Type: application/json

{
  "ExternalId": "P-1042",
  "ExternalSystem": "Contoso Commerce"
}
```

For import and merge operations, `ExternalId` with an optional `ExternalSystem` is the first lookup criterion. See [@erpnet.action](../data-manipulation/erpnet-action.md) for the complete lookup behavior.
