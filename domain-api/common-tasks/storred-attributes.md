
# Stored attributes (custom properties)

Stored attributes (also known as **custom properties** or **custom attributes**) are user-defined fields that extend the ERP.net data model on a per-instance basis.

In the Domain API, stored attributes appear as additional properties in the OData EDM model (`$metadata`) of the instance.

---

## Where stored attributes are defined

Stored attribute definitions are maintained in the `Systems.Bpm.CustomProperties` repository:

- Model reference: [Systems.Bpm.CustomProperties](https://docs.erp.net/model/entities/Systems.Bpm.CustomProperties.html)

Key definition settings (high level):

- `EntityName`: which entity type the property applies to (e.g. `Crm_Customers`, `General_Products_Products`, etc.)
- `Code`: the property code (used in API property naming)
- `PropertyType`: Text / Number / Date / Picture
- `LimitToAllowedValues`: whether the value is free text or constrained
- `AllowedValuesEntityName` / `AllowedValuesFilterXML`: allowed values coming from another entity
- `AllowedValuesProperty` / `AllowedValuesParent`: inheritance / filtering of allowed values

---

## How stored attributes appear in the Domain API

### Property name

In OData responses and requests, a stored attribute is represented as a property named:

`CustomProperty_<Code>`

Example:

- Code = `color`
- API property = `CustomProperty_color`

> [!note]
> **How `CustomProperty.Code` becomes an OData property name**
>
> In Domain API, a stored attribute is exposed as:
>
> `CustomProperty_<escapedCode>`
>
> where `<escapedCode>` is produced by the same logic as `EscapePropertyCode(code)`:
>
> - Characters that are **letters**, **digits**, or the underscore (`_`) are kept **as-is**.
> - Any other character (space, dot, dash, parentheses, etc.) is replaced with:
>   - the prefix **`_u`**, followed by
>   - the **UTF-8 bytes** of the character written as **uppercase hex**
>   - **If the character encodes to a single UTF‑8 byte**, the hex is left‑padded with `00` so the escape becomes 4 hex digits (e.g. `.` → `_u002E`).
>
> **Examples**
>
> - `Code = color` → `CustomProperty_color`
> - `Code = color.name` → `CustomProperty_color_u002Ename`  (dot `.` → `_u002E`)
> - `Code = size (cm)` → `CustomProperty_size_u0020_u0028cm_u0029`  
>   (space → `_u0020`, `(` → `_u0028`, `)` → `_u0029`)
>
> **Important limitation (as implemented)**
>
> The provided `UnescapePropertyCode()` recognizes only patterns of the form `_uXXXX` (exactly **4** hex digits). Escapes that produce **more than 4 hex digits** (i.e., characters whose UTF‑8 encoding is 3+ bytes) will **not** be unescaped by that method.

### Property type (CustomPropertyValue)

The value is represented using the `CustomPropertyValue` complex type:

- [Custom Property Value](../complex-types/custom-property-value.md)

Typical JSON shape:

```json
{
  "CustomProperty_color": {
    "Value": "apple",
    "ValueId": "5263a2d3-88b0-41db-adae-31c76135719e",
    "Description": { "EN": "The Apple." }
  }
}
```

---

## Refreshing the EDM model (when new properties do not appear)

When a new stored attribute is created in an ERP.net instance, it may not be visible immediately in Domain API because repositories and attributes are cached.
To refresh the cache, call the `/api/domain/reset` endpoint (authenticated).

See: [Custom Property Value](../complex-types/custom-property-value.md)

---

## Getting allowed values (GetAllowedCustomPropertyValues)

When a stored attribute is configured to have allowed values, you can retrieve the allowed values for a specific entity instance via the bound function:

`GetAllowedCustomPropertyValues` (Domain API request: **GET**)

This method is declared on the base `EntityObject` type and is documented in the model API methods (for example):
[General.Products.Products → API Methods](https://docs.erp.net/model/entities/General.Products.Products.html#getallowedcustompropertyvalues)

The metnod returns the allowed values for the specified custom property for this entity object. If supported the result is ordered by property value. Some property value sources do not support ordering - in that case the result is not ordered.
Return Type: Collection Of [CustomPropertyValue](../complex-types/custom-property-value.md)

### Parameters (as documented in the model)

- `customPropertyCode` (required): the stored attribute code
- `search` (optional): search text by value/description, supports `%`
- `exactMatch` (optional, default `false`)
- `orderByDescription` (optional, default `false`)
- `top` (optional, default `10`)
- `skip` (optional, default `0`)

### Example (typical OData function invocation)

```http
GET /api/domain/odata/Crm_Sales_Customers(79f3f74e-098a-4d91-9714-c4f845c2dc62)/GetAllowedCustomPropertyValues(customPropertyCode='color',search='app',top=10)
Accept: application/json
```

The result is a collection of `CustomPropertyValue` items.

> [!note]
> The exact URL template depends on the EDM model; always treat `$metadata` as the source of truth for the function signature and invocation syntax.

## See also

- [Custom Property Value](../complex-types/custom-property-value.md)
- [Operations (actions and functions)](../data-manipulation/operations.md)
