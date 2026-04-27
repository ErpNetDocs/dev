# Discover the domain model

## Overview

ERP.net exposes JSON endpoints that allow applications and developers to discover the Domain API model without querying business data directly.

These endpoints are useful when you need to:

- inspect the available repositories
- compare the standard product model with the current instance model
- generate integrations, clients, or metadata-driven UI
- discover repository members such as attributes, references, child collections, and bound methods

This topic covers these endpoints:

- `/api/domain/standardmodel`
- `/api/domain/standardmodel/{entitySet}`
- `/api/domain/currentmodel`
- `/api/domain/currentmodel/{entitySet}`

The `standardmodel` endpoints expose the default ERP.net model without instance-specific extensions.

The `currentmodel` endpoints expose the live model of the current ERP.net instance, including instance-specific extensions such as custom and calculated attributes.

## Getting started

### Get the standard model

Use the standard model when you need the baseline ERP.net model, independent of a specific instance customization.

```http
GET /api/domain/standardmodel
```

This returns a JSON array of repositories in compact form.

The `standardmodel` endpoint allows anonymous access.

### Get the current instance model

Use the current model when your application must reflect the actual structure of a specific ERP.net instance.

```http
GET /api/domain/currentmodel
```

This returns a JSON array of repositories from the current instance model.

The `currentmodel` endpoint requires authentication.

### Get one repository

To retrieve one repository in detail, append the entity set name to the URL.

```http
GET /api/domain/currentmodel/Crm_Sales_SalesOrders
GET /api/domain/standardmodel/Crm_Sales_SalesOrders
```

The `{entitySet}` segment uses underscores (`_`) instead of dots (`.`).

For example:

- `Crm_Sales_SalesOrders` in the URL corresponds to repository `Crm.Sales.SalesOrders`

When a single repository is requested, the response is always detailed, even if the `details` query parameter is not provided.

### Get the full detailed model

Both collection endpoints support the optional query parameter `details=true`.

```http
GET /api/domain/standardmodel?details=true
GET /api/domain/currentmodel?details=true
```

When `details=true` is specified, the response for every repository includes:

- `Attributes`
- `References`
- `ChildCollections`
- `Methods`

This allows applications to retrieve the entire model structure in a single request.

However, `details=true` produces a significantly larger payload. Prefer the compact form for repository discovery, and use `details=true` only when your application really needs the full model structure at once.

## Concepts

### Standard model vs. current model

Use `/api/domain/standardmodel` when you need the default ERP.net product model.

Use `/api/domain/currentmodel` when you need the live model of a specific ERP.net instance.

The current model may contain instance-specific extensions that do not exist in the standard model, such as:

- custom properties
- calculated attributes

### Compact vs. detailed responses

The collection endpoints support two response modes:

| Mode | How to request it | Typical use |
|---|---|---|
| Compact | `/standardmodel` or `/currentmodel` | Discover repositories quickly |
| Detailed | `/standardmodel?details=true` or `/currentmodel?details=true` | Load the full repository structure for all repositories |

In compact mode, member-level collections such as `Attributes`, `References`, `ChildCollections`, and `Methods` are omitted.

In detailed mode, those collections are included for every repository.

For a single repository request such as `/currentmodel/{entitySet}`, the response is always detailed.

### URL entity set format

The single-repository endpoints use the entity set name in underscore form.

| URL segment | Repository name |
|---|---|
| `Crm_Sales_SalesOrders` | `Crm.Sales.SalesOrders` |
| `General_Documents_DocumentTypes` | `General.Documents.DocumentTypes` |

Internally, the endpoint converts underscores to dots before resolving the repository.

### Serialization behavior

The responses are serialized as JSON and enum values are returned as strings.

For collection responses:

- in compact mode, null-valued properties are omitted from the JSON
- in detailed mode, null-valued properties are preserved in the JSON

This matters if your client distinguishes between a missing property and a property explicitly returned as `null`.

### Top-level repository structure

Each repository object contains the following top-level fields.

| Field | Description |
|---|---|
| `Name` | Repository name in underscore form, suitable for URL usage |
| `TableName` | Primary database table name, when applicable |
| `RepositoryName` | Repository name in dotted form |
| `LocalizedName` | Localized display name |
| `LocalizedDescription` | Localized description |
| `IsAggregateRoot` | Indicates whether the repository is an aggregate root |
| `AggregateParent` | Aggregate parent repository, in underscore form |
| `AggregateRoot` | Aggregate root repository, in underscore form |
| `ObjectType` | Full EDM object type name |
| `Category` | Repository category |
| `UIVisibility` | UI visibility classification |
| `Attributes` | Included only in detailed responses |
| `References` | Included only in detailed responses |
| `ChildCollections` | Included only in detailed responses |
| `Methods` | Included only in detailed responses |

### Attribute structure

When detailed information is included, each item in `Attributes` contains metadata for a repository attribute.

Internal attributes are excluded from the `Attributes` collection.

| Field | Description |
|---|---|
| `Name` | Attribute name as exposed in the API model |
| `ColumnName` | Physical column name, when applicable |
| `Type` | EDM type name |
| `LocalizedName` | Localized display name |
| `LocalizedDescription` | Localized description |
| `IsCustomProperty` | Indicates whether the attribute is a custom property |
| `IsCalculatedDataAttribute` | Indicates whether the attribute is calculated |
| `SupportsAllowedValues` | Indicates whether the attribute has an allowed values provider |
| `LimitToAllowedValues` | Indicates whether values are restricted to the allowed values set |
| `CategoryId` | Attribute category identifier |
| `CategoryLocalizedName` | Localized attribute category name |
| `IsBlob` | Indicates whether the attribute stores binary content |
| `SupportsSet` | Indicates whether the attribute can be set |
| `ReadOnly` | Indicates whether the attribute is read-only |
| `IsPersistent` | Indicates whether the attribute is persisted |
| `BaseInfo` | Low-level metadata object for the attribute |
| `enumValues` | Present for enum attributes; lists allowed enum values with localization |

### Enum attribute values

For enum attributes, the `enumValues` collection contains:

| Field | Description |
|---|---|
| `Value` | The raw enum value |
| `Name` | The enum member name |
| `LocalizedName` | Localized enum value name |
| `LocalizedDescription` | Localized enum value description |

### Reference structure

When detailed information is included, each item in `References` describes a repository reference.

| Field | Description |
|---|---|
| `Name` | Reference name as exposed in the API model |
| `ColumnName` | Backing column name, when applicable |
| `Type` | EDM type name |
| `LocalizedName` | Localized display name |
| `LocalizedDescription` | Localized description |
| `ReferencedEntitySet` | Target entity set name |
| `ReferencedType` | Full target EDM type name |
| `CategoryId` | Category identifier, when available |
| `CategoryLocalizedName` | Localized category name, when available |
| `ReadOnly` | Indicates whether the reference is read-only |
| `IsOwnership` | Indicates whether the reference is an ownership relation |
| `IsFilterable` | Indicates whether the reference is filterable |
| `IsAggregateParent` | Indicates whether the reference points to the aggregate parent |
| `BaseInfo` | Low-level metadata object, when the reference is backed by an attribute |

### Child collection structure

When detailed information is included, each item in `ChildCollections` describes a child collection of an enterprise repository.

| Field | Description |
|---|---|
| `Name` | Child collection name |
| `LocalizedName` | Localized display name |
| `LocalizedDescription` | Localized description |
| `ReadOnly` | Indicates whether the collection is read-only |
| `DefaultVisibleIndex` | Default visibility order index |
| `ChildObjectType` | Full EDM type name of the child object |
| `ChildEntitySet` | Child entity set name |

`ChildCollections` are available only for repositories that expose them.

### Method structure

When detailed information is included, each item in `Methods` describes a bound operation for the repository type.

| Field | Description |
|---|---|
| `Name` | Operation name |
| `Namespace` | Operation namespace |
| `EntitySetPath` | Entity set path |
| `ReturnType` | Return type, or `void` when there is no return value |
| `IsAction` | Indicates whether the operation is an action |
| `Parameters` | Operation parameters |

Each method parameter contains:

| Field | Description |
|---|---|
| `Name` | Parameter name |
| `Type` | Full EDM type name |

### `BaseInfo` structure

In detailed responses, the `BaseInfo` property exposes low-level metadata for an attribute.

The JSON object returned in `BaseInfo` is based on the `BaseAttributeInfo` class.

`BaseInfo` is included:

- for items in `Attributes`
- for items in `References`, when the reference is backed by an attribute

Use `BaseInfo` when your application needs implementation-oriented metadata such as filter support, storage details, nullability, ordering, display hints, subtype classification, or Open Exchange behavior.

| Field | Type | Description |
|---|---|---|
| `RepositoryName` | `string` | Full repository name of the attribute, when available |
| `LogicalOrder` | `int` | Logical order used when displaying, creating, cloning, or importing objects |
| `Internal` | `bool` | Indicates whether the attribute is intended for internal use |
| `Nullable` | `bool?` | Indicates whether the attribute allows null values |
| `IsDelayLoading` | `bool` | Indicates whether the attribute uses delayed loading |
| `Orderable` | `bool?` | Indicates whether the attribute can participate in ordering |
| `Indexed` | `bool` | Indicates whether the attribute participates in a table index as first column |
| `MaxLength` | `int` | Maximum length for text values |
| `Precision` | `int` | Total number of decimal digits for numeric values |
| `Scale` | `int` | Number of digits after the decimal separator for decimal values |
| `DateTimeStorageType` | `string` | Storage mode for `DateTime` values |
| `DateTimeUtc` | `bool` | Indicates whether date-time values are stored in UTC |
| `SupportedFilters` | `string` | Supported filter capabilities for the attribute |
| `IsFilterableReference` | `bool` | Indicates whether the attribute represents a reference that can be filtered through fields of the referenced entity |
| `SecurityCrucial` | `bool` | Indicates whether referenced-object security should be applied to the referencing object |
| `AllowTrackChanges` | `bool` | Indicates whether changes to the attribute create tracking change sets |
| `DefaultValueType` | `string` | Default value behavior for the attribute |
| `BaseTableName` | `string` | Data-access base table from which the attribute is inherited |
| `BaseColumnName` | `string` | Data-access base column from which the attribute is inherited |
| `Description` | `string` | Description shown alongside the attribute |
| `DisplayType` | `string` | Preferred display type in the presentation layer |
| `SummaryType` | `string` | Default summary type in the presentation layer |
| `WidthType` | `string` | Preferred width in the presentation layer |
| `RequiresUserInput` | `bool` | Indicates that the attribute requires user input and does not support a null value without a late system default |
| `Obsolete` | `bool` | Indicates whether the attribute is obsolete |
| `DefaultFormat` | `string` | Default format string |
| `UIVisibility` | `string` | User interface visibility classification |
| `Options` | `string` | Bit-flag options describing specific presentation or behavior traits |
| `Subtype` | `string` | Specific subtype classification for the attribute |
| `UnitExpression` | `string` | Expression that returns the measurement unit or currency for quantity or amount attributes |
| `OpenExchangeAttributeName` | `string` | Attribute name to use in Open Exchange format, when different from the standard name |
| `ExportInOpenExchange` | `bool` | Indicates whether the attribute participates in Open Exchange export |

### `BaseInfo.SupportedFilters`

`SupportedFilters` is a flags enum.

| Value | Meaning |
|---|---|
| `NotFilterable` | No filtering is supported |
| `Equals` | Equality filtering is supported |
| `GreaterThanOrLessThan` | Range-style comparison filtering is supported |
| `Like` | Pattern matching is supported |
| `EqualsIn` | Set-based equality filtering is supported |

### `BaseInfo.DefaultValueType`

| Value | Meaning |
|---|---|
| `None` | No default value |
| `Constant` | Constant default value |
| `NewGuid` | New GUID is generated by default |
| `CurrentDate` | Current date is used by default |
| `CurrentDateTime` | Current date and time are used by default |
| `CurrentDateTimeUtc` | Current UTC date and time are used by default |

### `BaseInfo.DisplayType`

| Value | Meaning |
|---|---|
| `Inline` | Single-line inline display |
| `InlineBlock` | Multiline inline-block display |
| `InlineDropDown` | Single-line display with a dropdown multiline editor |
| `ExternalPanel` | External panel with multiline editor |

### `BaseInfo.WidthType`

| Value | Meaning |
|---|---|
| `Default` | Default width |
| `Short` | Short width |
| `Long` | Long width |

### `BaseInfo.SummaryType`

| Value | Meaning |
|---|---|
| `None` | No summary |
| `Count` | Count summary |
| `Sum` | Sum summary |
| `Average` | Average summary |
| `Min` | Minimum summary |
| `Max` | Maximum summary |
| `StandardDeviation` | Standard deviation summary |
| `StandardDeviationPercent` | Standard deviation percentage summary |

### `BaseInfo.Options`

`Options` is a flags enum.

| Value | Meaning |
|---|---|
| `None` | No special option is set |
| `InterpolatedString` | The attribute may contain string interpolation expressions |
| `SecuritySensitiveString` | The attribute contains sensitive string data such as passwords, secrets, or API keys |

### `BaseInfo.Subtype`

| Value | Meaning |
|---|---|
| `NonSpecific` | No specific subtype |
| `Binary_Picture` | Binary picture content |
| `DateTime_Utc` | UTC date-time value |
| `Decimal_Amount` | Amount value |
| `Decimal_Percent` | Percent value |
| `Decimal_Quantity` | Quantity value |
| `Int_AutoNumber` | Auto-number integer value |
| `String_Code` | Code string |
| `String_DataFilter` | Data filter XML |
| `String_EntityName` | Entity or table name |
| `String_Html` | HTML content |
| `String_Identifier` | Identifier string |
| `String_MultilanguageString` | Encoded multilanguage string |
| `String_Rtf` | RTF content |
| `String_UserLogin` | User login value |
| `String_StrictCode` | Strict code string |
| `String_Password` | Secret password string |
| `String_Interpolated` | Interpolated string content |
| `String_Repository` | Repository name |
| `String_Markdown` | Markdown content |
| `String_FileName` | File name |

### Example

```json
{
  "Name": "DocumentNo",
  "Type": "Edm.String",
  "LocalizedName": "Document No",
  "ReadOnly": false,
  "BaseInfo": {
    "RepositoryName": "General.Documents.DocumentNo",
    "LogicalOrder": 120,
    "Internal": false,
    "Nullable": false,
    "IsDelayLoading": false,
    "Orderable": true,
    "Indexed": true,
    "MaxLength": 20,
    "Precision": 0,
    "Scale": 0,
    "DateTimeStorageType": "None",
    "DateTimeUtc": false,
    "SupportedFilters": "Equals",
    "IsFilterableReference": false,
    "SecurityCrucial": true,
    "AllowTrackChanges": true,
    "DefaultValueType": "None",
    "BaseTableName": "Gen_Documents",
    "BaseColumnName": "Document_No",
    "Description": "User-visible document number.",
    "DisplayType": "Inline",
    "SummaryType": "None",
    "WidthType": "Default",
    "RequiresUserInput": false,
    "Obsolete": false,
    "DefaultFormat": null,
    "UIVisibility": "ShownByDefault",
    "Options": "None",
    "Subtype": "String_Code",
    "UnitExpression": null,
    "OpenExchangeAttributeName": null,
    "ExportInOpenExchange": true
  }
}
```

## See also

- [Domain API](../index.md)
- [Querying data](../querying-data/index.md)
- [API Reference](https://docs.erp.net/model/entities/)
