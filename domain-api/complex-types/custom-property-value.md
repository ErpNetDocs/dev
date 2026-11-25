# Custom Property Value

`CustomPropertyValue` is the Domain API complex type for the saved value of a stored attribute. An entity exposes it as `CustomProperty_<Code>`. The items returned by `GetAllowedCustomPropertyValues` use the same type. See [Stored attributes](../common-tasks/stored-attributes.md) for requests that read, write, and filter these values.

## Composition of the CustomPropertyValue type

| Field | Type | Meaning |
|-------|------|---------|
| `Value` | String or null | The short stored value, such as a code or free text. Use it when filtering by an ordinary custom property. |
| `ValueId` | GUID or null | The ID of the selected allowed value. For a reference property, it is the ID of the referenced object. |
| `Description` | MultilanguageString or null | A localized description or label for the value. |

Use a string for a non-null `Value`, including for number and date properties. The standard short value for a date uses `yyyyMMdd`, for example `"20260518"`. Use `ValueId` to identify a selected item even if its short value is missing.

### Example

This product's color was selected from a list.

```json
{
  "CustomProperty_color": {
    "Value": "blue",
    "ValueId": "5263a2d3-88b0-41db-adae-31c76135719e",
    "Description": { "EN": "Blue", "DE": "Blau" }
  }
}
```

## Write and filter values

### Write a value

For a free-text property, write `{ "Value": "blue" }`. For a property restricted to a list, write the selected value object returned by `GetAllowedCustomPropertyValues`, including its `ValueId`. An unset property is represented by `null` for the whole property.

### Filtering

For ordinary stored attributes, compare the property directly to the text in `Value` with `eq`.

```text
General_Products_Products?$top=10&$select=CustomProperty_color&$filter=CustomProperty_color eq 'blue'
```

Reference properties also have a `CustomReference_<Code>` navigation property. Expand it to read the related record. To match a specific record, use `CustomReference_<Code> eq 'EntitySet(Id)'` with the referenced object's `@odata.id`. See the [reference filter example](../common-tasks/custom-property-references.md#filtering).

## Property definitions and metadata

### Definition of a stored attribute (custom property)

A property definition sets where the field appears and which values it accepts. Definitions are managed in [Systems.Bpm.CustomProperties](https://docs.erp.net/model/entities/Systems.Bpm.CustomProperties.html).

| Setting | Meaning |
|---------|---------|
| `EntityName` | The entity on which the property is defined. |
| `Code` | The property code used to build its API field name. |
| `LimitToAllowedValues` | When enabled, the property value must come from its allowed-values list. |
| `AllowedValuesEntityName` | The entity that supplies the allowed values, if the list comes from another entity. |

For a list maintained on the property itself, add entries in [Systems.Bpm.CustomPropertyAllowedValues](https://docs.erp.net/model/entities/Systems.Bpm.CustomPropertyAllowedValues.html).

If the property needs a navigation field for the selected record, set `PropertyType` to `Reference`. An entity can also supply allowed values for an ordinary property. See [User-defined references](../common-tasks/custom-property-references.md).

### API property name (OData-compliant)

The Domain API adds the prefix `CustomProperty_` to the property's escaped code. Letters, digits, and underscores are kept as written. Other characters are escaped with `_u` followed by the uppercase hexadecimal representation of their UTF-8 bytes. The API adds `00` before the hexadecimal value for a single-byte escape.

| Configured code | API field name |
|-----------------|----------------|
| `color` | `CustomProperty_color` |
| `color.name` | `CustomProperty_color_u002Ename` |
| `size (cm)` | `CustomProperty_size_u0020_u0028cm_u0029` |

Each instance has its own stored attributes. Use the exact field name returned by OData `$metadata` or the [current model](../common-tasks/discover-model.md).

For new codes, prefer ASCII letters, digits, and underscores. If a character's escape contains more than four hex digits, the API can fail to resolve the generated name back to the property.

### Reset (refreshing the EDM model)

The Domain API caches its model. If a newly configured property is missing, send an authenticated request to refresh the cache, then fetch `$metadata` or the current model again.

```http
GET /api/domain/reset HTTP/1.1
Host: demodb.my.erp.net
Authorization: Bearer <access_token>
```
