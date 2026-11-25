# Stored attributes (custom properties)

Stored attributes, also called custom properties, are extra fields you can add to a type of record in your ERP.net instance, such as products or customers.

You define the field once for that record type. The definition gives the field a code and determines what values it accepts. Each record can then have its own saved value. For example, a **Color** field defined for products can hold `blue` on one product and `black` on another. Updating one product changes its saved color.

## Identify a custom property

### Property code, API field name, and value

| Term | Example | Meaning |
|------|---------|---------|
| Property code | `color` | Identifies the field in its definition. Use it when looking up allowed values. |
| API field name | `CustomProperty_color` | Identifies the field in a Domain API request or response. Use it in `$select`, `$filter`, and JSON bodies. |
| Saved value | `blue` | The color saved on this particular product. In JSON it appears inside the field's `Value` member. |

### Find the field name on your instance

Custom properties vary by instance. To find the fields configured on products, request the current model for the product entity set.

```http
GET /api/domain/currentmodel/General_Products_Products HTTP/1.1
Host: demodb.my.erp.net
Authorization: Bearer <access_token>
```

Look in the `Attributes` list for entries where `IsCustomProperty` is `true`. The `Name` is the API field name. `LimitToAllowedValues` tells you whether the field requires a choice from a list. Here is part of the entry for the free-text Color field.

```json
{
  "Name": "CustomProperty_color",
  "IsCustomProperty": true,
  "LimitToAllowedValues": false
}
```

The same field name appears in OData `$metadata`. Copy it from the model, especially if the property code contains spaces or punctuation. For example, the code `color.name` appears as `CustomProperty_color_u002Ename`.

If a newly configured property is missing, refresh the model with an authenticated `GET /api/domain/reset`, then request the current model again. See [Discover the domain model](discover-model.md).

## Read and filter values

The examples use the Color field on product `CHAIR-01`. Replace the example host, product ID, and access token with your own values.

### Read a value

Use `$select` to ask the API to return the custom field. A default product response omits custom fields.

```http
GET /api/domain/odata/General_Products_Products(59098bcf-f331-478f-91c2-f5520590f534)?$select=PartNumber,CustomProperty_color HTTP/1.1
Host: demodb.my.erp.net
Authorization: Bearer <access_token>
```

Find `CustomProperty_color` in the product response and read its `Value`. This product has `blue` saved as its color.

```json
{
  "PartNumber": "CHAIR-01",
  "CustomProperty_color": {
    "Value": "blue",
    "ValueId": null,
    "Description": null
  }
}
```

The API uses a JSON object so it can return more than the saved text. `ValueId` can identify the selected item when the field uses a list. `Description` can contain its display label in multiple languages. In this example, the field has just a text value, so both are `null`. See [Custom Property Value](../complex-types/custom-property-value.md).

### Filter by a property

The filter compares the text saved in `Value`. Write the API field name followed by `eq` and the value in single quotes. For example, `CustomProperty_color eq 'black'` finds products with `black` saved as their color.

```http
GET /api/domain/odata/General_Products_Products?$top=10&$select=PartNumber,CustomProperty_color&$filter=CustomProperty_color%20eq%20'black' HTTP/1.1
Host: demodb.my.erp.net
Authorization: Bearer <access_token>
```

Use `$select` to include the color in the response.

## Write or clear a value

Send a `PATCH` to the record you want to update. When creating a record, you can include the custom property in the `POST` body.

### Set a free-text value

If the field accepts free text, use the API field name in the request body and put the text you want to save in `Value`.

```http
PATCH /api/domain/odata/General_Products_Products(59098bcf-f331-478f-91c2-f5520590f534) HTTP/1.1
Host: demodb.my.erp.net
Authorization: Bearer <access_token>
Content-Type: application/json

{
  "CustomProperty_color": {
    "Value": "black"
  }
}
```

### Set a value from a list

When a field requires a choice from a list, the available choices can depend on the record you are editing. Call `GetAllowedCustomPropertyValues` on that record. Pass `color`, the property's configured code, as `customPropertyCode`.

```http
GET /api/domain/odata/General_Products_Products(59098bcf-f331-478f-91c2-f5520590f534)/GetAllowedCustomPropertyValues(customPropertyCode='color') HTTP/1.1
Host: demodb.my.erp.net
Authorization: Bearer <access_token>
```

The response returns a list of value objects. Choose one and copy it, including its `ValueId`, into `CustomProperty_color` in the product's `PATCH` body. This example selects the Black option.

```json
{
  "CustomProperty_color": {
    "Value": "black",
    "ValueId": "5263a2d3-88b0-41db-adae-31c76135719e",
    "Description": { "EN": "Black" }
  }
}
```

The function returns up to 10 choices by default. Add `search='bl'` to search by value or label. Use `top` and `skip` to page through the results. See the [function reference](https://docs.erp.net/model/entities/General.Products.Products.html#getallowedcustompropertyvalues) for all parameters.

### Clear an optional value

To remove the color saved on a product, send the API field name with `null` in the `PATCH` body.

```http
PATCH /api/domain/odata/General_Products_Products(59098bcf-f331-478f-91c2-f5520590f534) HTTP/1.1
Host: demodb.my.erp.net
Authorization: Bearer <access_token>
Content-Type: application/json

{
  "CustomProperty_color": null
}
```

A read that selects the field now returns `"CustomProperty_color": null`. To keep a saved value when updating other fields, omit the custom field from the `PATCH` body.

### Set a reference

For a property that links to another record, assign the related record through its `CustomReference_<Code>` field. See [User-defined references](custom-property-references.md).

## Manage property definitions

Use `Systems_Bpm_CustomProperties` to manage field definitions. For a product field, set the definition's `EntityName` to `Gen_Products`. Read and write the saved values on product records through `General_Products_Products`.

For an allowed-values list, maintain entries in `Systems_Bpm_CustomPropertyAllowedValues` or set `AllowedValuesEntityName` to source values from another entity. See the [Custom Properties model reference](https://docs.erp.net/model/entities/Systems.Bpm.CustomProperties.html).
