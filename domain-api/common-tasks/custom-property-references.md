# User-defined references

A user-defined reference is a stored attribute whose value points to an object from another entity. It is configured as a custom property with `PropertyType = Reference`.

## Configuration

User-defined references are defined in [Systems.Bpm.CustomProperties](https://docs.erp.net/model/entities/Systems.Bpm.CustomProperties.html).

Set the following fields:

- `EntityName` — the entity containing the property.
- `Code` — the property code.
- `PropertyType` — `Reference`.
- `AllowedValuesEntityName` — the referenced aggregate-root entity.

`LimitToAllowedValues` is enforced automatically for reference properties. The value of `AllowedValuesEntityName` must identify an existing aggregate-root entity.

> [!note]
> `AllowedValuesEntityName` is also used for ordinary stored attributes whose allowed values come from another entity. The `PropertyType = Reference` setting is what makes the property available as a navigation reference.

## Domain API names

The property is exposed in two forms. Replace `<Code>` with the escaped property code when necessary:

- `CustomProperty_<Code>` — the stored attribute value, represented by [CustomPropertyValue](../complex-types/custom-property-value.md).
- `CustomReference_<Code>` — the navigation property to the referenced object.

The two names are intentionally different. The custom property contains the stored value and snapshot data; the custom reference provides normal Domain API reference behavior.

## Reading and expanding

Select the custom property and expand the reference when the referenced object data is needed:

```http
GET /api/domain/odata/Crm_Sales_Customers(79f3f74e-098a-4d91-9714-c4f845c2dc62)?$select=Id,CustomProperty_Project&$expand=CustomReference_Project($select=Id,Code,Name) HTTP/1.1
Host: demodb.my.erp.net
Authorization: Bearer <access_token>
```

The `CustomProperty_Project` value contains `Value`, `ValueId`, and `Description`. The expanded `CustomReference_Project` contains the referenced entity according to the selected fields.

## Creating or updating a reference

A reference can be assigned using its navigation property and the `@odata.id` of the referenced object. This request assigns a classic project to an existing customer. It assumes that the customer entity has a reference property named `Project`.

```http
PATCH /api/domain/odata/Crm_Sales_Customers(79f3f74e-098a-4d91-9714-c4f845c2dc62) HTTP/1.1
Host: demodb.my.erp.net
Authorization: Bearer <access_token>
Content-Type: application/json

{
  "CustomReference_Project": {
    "@odata.id": "Projects_Classic_Projects(5263a2d3-88b0-41db-adae-31c76135719e)"
  }
}
```

Use the entity-set name from the instance's `$metadata` and the ID of the project record. To clear an optional reference, send `"CustomReference_Project": null` in the `PATCH` body.

When the reference is changed, `ValueId` is stored as the identifier of the referenced object. `Value` and `Description` are populated from the referenced object. For repositories with a `Code` data member, `Value` is the code and `Description` is the name; otherwise the object's display text is used for `Value`.

> [!NOTE]
> `Value` and `Description` are snapshot fields. They are validated and synchronized from the referenced object when the reference is saved. If the referenced object is modified later, these snapshots are not updated automatically and may become stale.

## Filtering

Filter directly by the navigation property. Use the referenced object's `@odata.id` as the comparison value.

```http
GET /api/domain/odata/Crm_Sales_Customers?$filter=CustomReference_Project%20eq%20'Projects_Classic_Projects(5263a2d3-88b0-41db-adae-31c76135719e)' HTTP/1.1
Host: demodb.my.erp.net
Authorization: Bearer <access_token>
```

The filter matches by the referenced object's ID, so it still finds the reference if the object's name changes.

## Validation and restrictions

- A reference property always uses allowed values.
- The referenced entity must be an existing aggregate root.
- The reference definition cannot be changed after values have been stored.
- A referenced object cannot be deleted while it is used by a reference property.
- When a reference value is saved, its `Value` and `Description` snapshots are validated and synchronized from `ValueId`.

If a new property is not visible in the instance Domain API, refresh the cached model with the authenticated `/api/domain/reset` endpoint.

## See also

- [Stored attributes (custom properties)](stored-attributes.md)
- [Custom Property Value](../complex-types/custom-property-value.md)
- [Create](../data-manipulation/create.md)
- [Update](../data-manipulation/update.md)
