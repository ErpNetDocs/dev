# Custom Property Value

Stored attributes (also known as **custom properties** / **custom attributes**) are user-defined attributes that extend the ERP.net data model per instance.

In the Domain API, stored attributes are exposed as properties named `CustomProperty_<code>`, whose value is represented by the `CustomPropertyValue` complex type.

See also: [Stored attributes (custom properties)](../common-tasks/stored-attributes.md)

## Definition of a stored attribute (custom property)

Stored attribute definitions are maintained in:

- Model reference: [Systems.Bpm.CustomProperties](https://docs.erp.net/model/entities/Systems.Bpm.CustomProperties.html)

Highlights from the definition record:

- `EntityName` contains the name of the entity for which the property is defined.
- `LimitToAllowedValues` defines whether the property is free text or limited to allowed values.
- `AllowedValuesEntityName` specifies that the allowed values are retrieved from another entity.
  If this is null, allowed values can be maintained in:
  [Systems.Bpm.CustomPropertyAllowedValues](https://docs.erp.net/model/entities/Systems.Bpm.CustomPropertyAllowedValues.html)

## API property name (OData-compliant)

The API name of the stored attribute starts with `CustomProperty_` followed by the (possibly escaped) user-defined property code.

> [!note]
> **How `CustomProperty.Code` is converted to an OData-compliant property name**
>
> Domain API uses an escaping algorithm equivalent to:
> `CustomProperty_ + EscapePropertyCode(CustomProperty.Code)`.
>
> - Letters (`A-Z`, `a-z`), digits (`0-9`), and `_` are kept as-is.
> - Any other character is replaced with `_u` followed by the UTF-8 bytes of the character encoded as **uppercase hex**.
>   - If the character encodes to a **single UTF‑8 byte**, the hex is left-padded with `00` so the escape is 4 hex digits (e.g. `.` → `_u002E`).
>
> Examples:
>
> - `Code = color` → `CustomProperty_color`
> - `Code = color.name` → `CustomProperty_color_u002Ename`
> - `Code = size (cm)` → `CustomProperty_size_u0020_u0028cm_u0029`

Each ERP.net instance can have different stored attributes, therefore each instance has its own EDM model (`$metadata`).

## Reset (refreshing the EDM model)

If a user creates a new stored attribute in the database, it may not be exposed in the Domain API in real time.
Domain API caches repositories and their attributes until the next restart.

To refresh the cached attributes, call the `/api/domain/reset` endpoint (authenticated).

Example:
<https://demodb.my.erp.net/api/domain/reset>

## Composition of the CustomPropertyValue type

| Name | Type | Description |
|---|---|---|
| `Value` | String | The short value (the actual value). |
| `Description` | MultilanguageString | A long descriptive multi-language value. Can be null. |
| `ValueId` | Guid | The Id of the allowed value entry. Can be null. |

## Example

```json
"CustomProperty_color": {
  "Value": "apple",
  "ValueId": "5263a2d3-88b0-41db-adae-31c76135719e",
  "Description": {
    "EN": "The Apple.",
    "DE": "Der Apfel."
  }
}
```

> [!note]
> To filter by a stored attribute you must use only the short value (`Value`) and only `eq` is supported:
> `General_Products_Products?$top=10&$select=CustomProperty_color&$filter=CustomProperty_color eq 'apple'`
