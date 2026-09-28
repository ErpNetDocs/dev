# Custom entities

Custom entities let an ERP.net instance add entity types that are specific to that instance. Each active and valid type is exposed as a repository in the Domain Model and can be read or changed through the Domain API.

A custom type definition is stored in `Systems.Bpm.CustomEntityTypes`. Its `RepositoryName` and the corresponding Domain API entity set are derived from `RepositoryNamespace`, `Code`, and whether the type has a parent. They are not entered independently.

## Define a custom entity type

Create a definition in the `Systems_Bpm_CustomEntityTypes` entity set. For example, this request defines a root type for flexible workspaces in the `Crm.Sales` repository namespace:

```http
POST https://<instance>.my.erp.net/api/domain/odata/Systems_Bpm_CustomEntityTypes
Content-Type: application/json
```

```json
{
  "Code": "Workspaces",
  "Name": { "EN": "Workspace" },
  "PluralName": { "EN": "Workspaces" },
  "Description": { "EN": "Flexible workspaces employees can reserve." },
  "RepositoryNamespace": "Crm.Sales",
  "Active": true
}
```

The `EN` values are examples; provide the localized values required by the instance.

### Definition fields

| Field | Meaning and constraints |
| --- | --- |
| `Id` | System-generated identifier of the definition. |
| `Code` | Required, stable technical identifier used to derive names and identify the type in metadata and configuration. Use the plural form of the type, such as `Workspaces`. It must start with an ASCII letter and contain only ASCII letters and digits (`A-Z`, `a-z`, `0-9`); it cannot contain spaces, punctuation, dots, or underscores. Maximum length: 64 characters. The `(Code, RepositoryNamespace)` pair must be unique. |
| `Name` | Required, localized singular display name, such as `Workspace`. Maximum length: 254 characters. It does not affect technical names. |
| `PluralName` | Required, localized plural display or collection name, such as `Workspaces`. Maximum length: 254 characters. It does not affect technical names. |
| `Description` | Required, localized explanation of the type's purpose. Maximum length: 254 characters. |
| `RepositoryNamespace` | Required namespace for the generated repository, such as `Crm.Sales`. It must exactly match a namespace registered in the current Domain Model/RepositorySource; arbitrary namespaces are rejected. Maximum length: 128 characters. |
| `ParentEntityType` | Optional reference to another custom type. Leave it empty for a root type. Set it to a root type to define a sub-entity collection. A sub-entity cannot itself be a parent. |
| `Active` | Defaults to `true`. Only active, valid definitions (and sub-entity definitions with active, valid parents) are registered in the Domain Model. Deactivating a type prevents new records of that type from being created; existing records are not deleted. |

`Code` is deliberately separate from the localized display names: changing a display name does not rename the repository or its entity set. Choose the code carefully because it participates in technical identifiers.

## Repository names and Domain API entity sets

`RepositoryName` is the logical name used by the Domain Model. The prefixes distinguish root entities from sub-entities:

| Type | `RepositoryName` |
| --- | --- |
| Root entity | `{RepositoryNamespace}.CustomEntity{Code}` |
| Sub-entity | `{RepositoryNamespace}.CustomSubEntity{Code}` |

The Domain API entity set uses the same name with the namespace's dots replaced by underscores:

| Definition | `RepositoryName` | Domain API entity set |
| --- | --- | --- |
| `RepositoryNamespace = Crm.Sales`, `Code = Workspaces`, no parent | `Crm.Sales.CustomEntityWorkspaces` | `Crm_Sales_CustomEntityWorkspaces` |
| `RepositoryNamespace = Crm.Sales`, `Code = Reservations`, parent = `Workspaces` | `Crm.Sales.CustomSubEntityReservations` | `Crm_Sales_CustomSubEntityReservations` |

For this example, each root record represents one physical workspace. Its `Reservations` sub-entity records represent bookings for that workspace, with details such as the reservation date and employee stored on each reservation. This arrangement works when each reservation is for exactly one workspace; the same employee can reserve a different workspace on another day. Use the exact entity set returned by the instance's live model; do not build it from the localized `Name` or `PluralName`.

The related internal `EntityName` follows the same dot-to-underscore conversion. It identifies the custom entity in table metadata and configuration; clients normally use the entity set in Domain API requests.

## Discover and use the repositories

Custom entities are instance-specific, so discover them from that instance's live Domain Model. The standard model does not list them.

- Use `/api/domain/currentmodel` to find the available repositories.
- Use `/api/domain/currentmodel/{entitySet}` to inspect an entity set's attributes, references, and child collections.

For example, after defining the root type above:

```http
GET https://<instance>.my.erp.net/api/domain/odata/Crm_Sales_CustomEntityWorkspaces
```

Use the same entity set for standard `POST`, `PATCH`, and `DELETE` operations. For a sub-entity, use its own entity set and the parent reference exposed by the live model. The platform assigns the definition's `EntityTypeId` internally when records are created through the generated type. This internal field is not exposed in Domain API data, so clients do not send or read it.

Root and sub-entity records include common fields such as `Id`, `Code`, `Name`, and `Active`. A sub-entity also belongs to its root and has a line number within that parent. A record's `Code` is optional; when supplied, it must be unique within its parent and sub-entity type.

Custom repositories participate in the common Domain Model mechanisms, including stored custom properties, calculated attributes, user business rules, validations, filtering, and localized display metadata, when configured for the instance.


## Records and their storage

Custom entity types describe the shape of a repository. The records created through that repository are stored in shared tables; the platform does not create a physical SQL table for every custom type.

### `Systems.Bpm.CustomEntities` — `Sys_Custom_Entities`

This shared storage entity holds records created through generated root custom entity sets, such as `Crm_Sales_CustomEntityWorkspaces`.

| Field | Availability in Domain API | Meaning |
| --- | --- | --- |
| `Id` | Exposed as the entity key; generated by the platform. | Unique identifier of the workspace or other root custom entity record. |
| `Code` | Exposed; optional; maximum 64 characters. | Business identifier. When supplied, it must be unique within the custom entity type. |
| `Name` | Exposed; optional; localized; maximum 254 characters. | Human-readable name of the record. |
| `Active` | Exposed; defaults to `true`. | Indicates whether the record is active. |
| `EntityTypeId` | Internal storage field; not exposed in Domain API data. | Identifies the custom type definition. The generated custom entity repository determines and fills it automatically. Clients do not include it in requests. |

### `Systems.Bpm.CustomSubEntities` — `Sys_Custom_Sub_Entities`

This shared storage entity holds records created through generated sub-entity sets, such as `Crm_Sales_CustomSubEntityReservations`.

| Field | Availability in Domain API | Meaning |
| --- | --- | --- |
| `Id` | Exposed as the entity key; generated by the platform. | Unique identifier of the reservation or other child record. |
| `Code` | Exposed; optional; maximum 64 characters. | Business identifier. When supplied, it must be unique within its owning root record and sub-entity type. |
| `Name` | Exposed; optional; localized; maximum 254 characters. | Human-readable name of the child record. |
| `Active` | Exposed; defaults to `true`. | Indicates whether the child record is active. |
| `LineNo` | Exposed; defaults to the next number within the root record. | Orders child records belonging to the same root. Usually let the platform assign it. |
| `RootEntityId` | Internal storage field; not exposed in Domain API data. | Identifies the owning root record. In the generated sub-entity API, use the `RootEntity` reference to associate the reservation with a workspace. |
| `EntityTypeId` | Internal storage field; not exposed in Domain API data. | Identifies the sub-entity type definition. The generated custom sub-entity repository determines and fills it automatically. Clients do not include it in requests. |

The generated repositories expose records through entity sets for their custom types. For example, `Crm_Sales_CustomEntityWorkspaces` is backed by `Systems.Bpm.CustomEntities`, and `Crm_Sales_CustomSubEntityReservations` is backed by `Systems.Bpm.CustomSubEntities`. For child records, use the parent reference exposed by the live model.

In a generated root type such as `Workspaces`, the Domain API exposes `Id`, `Code`, `Name`, and `Active`, together with any configured custom properties. A generated sub-type such as `Reservations` exposes those fields plus `LineNo` and the `RootEntity` reference to its parent; internal `EntityTypeId` and `RootEntityId` storage fields are omitted.

### Define custom properties before creating records

A custom type becomes useful for a particular business process when you attach stored custom properties to its generated entity name. Define these properties in `Systems_Bpm_CustomProperties`. Set `EntityName` to the custom entity's internal entity name (namespace dots changed to underscores), not its repository name or localized display name. In this example, reservations can record the managed asset booked with a workspace:

```http
POST https://<instance>.my.erp.net/api/domain/odata/Systems_Bpm_CustomProperties
Content-Type: application/json
```

```json
{
  "Code": "ReservedManagedAsset",
  "Name": { "EN": "Reserved managed asset" },
  "EntityName": "Crm_Sales_CustomSubEntityReservations",
  "PropertyType": "Reference",
  "AllowedValuesEntityName": "Applications_AssetManagement_ManagedAssets",
  "IsActive": true
}
```

For the employee reference, the direction is from the reservation to the employee: `ReservationEmployee` is defined on `Reservations` and points to `General.Contacts.Persons`. No property or reference is added to the employee record. Define the property as follows:

```json
{
  "Code": "ReservationEmployee",
  "Name": { "EN": "Reservation employee" },
  "EntityName": "Crm_Sales_CustomSubEntityReservations",
  "PropertyType": "Reference",
  "AllowedValuesEntityName": "General_Contacts_Persons",
  "IsActive": true
}
```

`PropertyType` is an enum in the Domain API. Use its member names: `Reference`, `Text`, `Number`, `Date`, or `Picture`. The model maps these to storage codes (`R`, `T`, `N`, `D`, and `P` respectively); send the enum names in API requests, not the storage codes. A reference property is automatically limited to records from `AllowedValuesEntityName`. The selected entity must be an aggregate root with a `Code` attribute. `Applications.AssetManagement.ManagedAssets` satisfies these requirements; its Domain API entity set and `EntityName` are both `Applications_AssetManagement_ManagedAssets`. A managed asset is an individually tracked item, such as a monitor reserved with a workspace; `Finance.Assets.Assets` represents fixed assets for financial accounting. `General.Contacts.Persons` is also a valid reference target for the employee property because it is an aggregate root; its entity set and `EntityName` are `General_Contacts_Persons`.

The properties appear on reservations as `CustomProperty_ReservedManagedAsset` and `CustomProperty_ReservationEmployee`, each using the `CustomPropertyValue` complex type. Set `Value` to the selected record's code and `ValueId` to its `Id`; the create example below shows both values.

You can add ordinary custom properties in the same way. For example, a `ReservationDate` property with `PropertyType: "Date"` can store the date of the booking. Use distinct property codes for each definition on an entity, and keep codes stable after clients start using the API because they form part of the public property name. The API escapes punctuation in property codes; simple letter-and-digit codes avoid that extra mapping.

#### Reference from a system entity to a custom entity

References can point in the other direction too. For a managed asset that has a usual workspace, define a reference property on the managed asset entity whose allowed values are workspace records:

```json
{
  "Code": "PrimaryWorkspace",
  "Name": { "EN": "Primary workspace" },
  "EntityName": "Applications_AssetManagement_ManagedAssets",
  "PropertyType": "Reference",
  "AllowedValuesEntityName": "Crm_Sales_CustomEntityWorkspaces",
  "IsActive": true
}
```

This exposes `CustomProperty_PrimaryWorkspace` on `Applications_AssetManagement_ManagedAssets`. Populate `Code` on each workspace record so it can be selected as an allowed value. For instance, a room-installed display can point to its usual workspace. This custom property is a simple current assignment; managed assets also have a standard `Locations` child collection for company location assignments, including the assignment start date. Use that collection when you need to track standard company locations. The reservation's `ReservedManagedAsset` property represents the asset chosen for a booking. The `CompanyEmployees` repository represents employee contracts but is a child entity, so it cannot be used directly as `AllowedValuesEntityName` for a reference property. `Person.NationalNumber` is optional; use `ValueId` as the person identifier, and treat `Value` as a display/search code that may be empty.

These are stored custom-property references represented by `CustomPropertyValue`; they do not add a typed navigation reference or change the target entity's schema. Discover the exposed properties and exact OData shape from `/api/domain/currentmodel/{entitySet}` and `$metadata`. For the value object fields, see [Custom Property Value](complex-types/custom-property-value.md); for allowed-value behavior, see [Stored attributes](common-tasks/stored-attributes.md).

### Create a workspace and reservation

After defining the `Workspaces` root type and its `Reservations` child type, create a workspace through the generated root entity set:

```http
POST https://<instance>.my.erp.net/api/domain/odata/Crm_Sales_CustomEntityWorkspaces
Content-Type: application/json
```

```json
{
  "Code": "WS-014",
  "Name": { "EN": "East wing 014" },
  "Active": true
}
```

Then create a reservation through the generated child entity set and bind it to the workspace. The example also sets the custom properties defined above. Replace the placeholders with the IDs and codes of an existing managed asset and person, and the `Id` returned for the workspace:

```http
POST https://<instance>.my.erp.net/api/domain/odata/Crm_Sales_CustomSubEntityReservations
Content-Type: application/json
```

```json
{
  "Code": "RES-20261012-014",
  "Name": { "EN": "Reservation for 12 October 2026" },
  "RootEntity@odata.bind": "Crm_Sales_CustomEntityWorkspaces(<workspace-id>)",
  "Active": true,
  "CustomProperty_ReservedManagedAsset": {
    "Value": "MA-014",
    "ValueId": "<managed-asset-id>"
  },
  "CustomProperty_ReservationEmployee": {
    "Value": "<person-national-number-if-available>",
    "ValueId": "<person-id>"
  }
}
```

The platform derives the type from the generated entity set and fills the internal `EntityTypeId` automatically; this field is not returned in Domain API data and must not be included in the request. The platform also assigns the next `LineNo` within the workspace. The reservation belongs to its workspace through `RootEntity`.

The same records can also be queried through the shared `Systems_Bpm_CustomEntities` and `Systems_Bpm_CustomSubEntities` entity sets. Each shared set contains records for multiple custom types, and `EntityTypeId` is not available in Domain API data to filter or identify those records. Use the type-specific `Crm_Sales_CustomEntityWorkspaces` and `Crm_Sales_CustomSubEntityReservations` entity sets for application queries and writes. The shared sets are intended for generic operations that can handle mixed custom types.

## Related topics

- [Discover the Domain Model](common-tasks/discover-model.md)
- [Domain API](index.md)