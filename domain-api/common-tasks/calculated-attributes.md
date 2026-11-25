# Calculated attributes

A calculated attribute is a read-only value that ERP.net computes when you request it. For example, an instance can add an `AccountStatus` attribute to customers that returns `Active` or `Inactive` based on the customer's `Active` field. Use a [stored attribute](stored-attributes.md) when the integration needs to save a value of its own.

## Find the attribute

Calculated attributes are configured per repository in an instance. For an instance-defined calculation, the Domain API name starts with `CalculatedAttribute_` followed by the configured `Name`, with escaping applied where needed.

```http
GET /api/domain/currentmodel/Crm_Sales_Customers HTTP/1.1
Host: demodb.my.erp.net
Authorization: Bearer <access_token>
```

Find the attribute in `Attributes` by its `IsCalculatedDataAttribute` flag. Use the exact `Name` and `Type` from the instance model or OData `$metadata`. If a newly configured attribute is missing, check that it is active and belongs to this repository. If necessary, refresh the model with an authenticated `GET /api/domain/reset`. See [Discover the domain model](discover-model.md).

## Request the value

This example assumes the instance has an active string calculation named `AccountStatus` on `Crm.Sales.Customers`. The [calculated attribute scripting walkthrough](../../scripting/getting-started/calculated-attributes.md) shows how to configure this calculation.

```http
GET /api/domain/odata/Crm_Sales_Customers?$top=5&$select=Id,Active,CalculatedAttribute_AccountStatus HTTP/1.1
Host: demodb.my.erp.net
Authorization: Bearer <access_token>
Accept: application/json
```

The calculated result is returned under the selected property's name. This example shows one item from the response.

```json
{
  "Id": "79f3f74e-098a-4d91-9714-c4f845c2dc62",
  "Active": true,
  "CalculatedAttribute_AccountStatus": "Active"
}
```

Add the attribute explicitly to [`$select`](../querying-data/query-options/select.md). Use `$select=default,CalculatedAttribute_AccountStatus` to request it along with the default fields.

## Query and write behavior

| Operation | Behavior |
|-----------|----------|
| `$select` | Request the calculated value by its API name. |
| `$filter`, `$orderby` | User-defined calculated attributes do not support server-side filtering or sorting. |
| `POST`, `PATCH` | Write the underlying editable fields. The calculated attribute is read-only. |

When a calculation derives from a filterable field, filter that field. For example, use `$filter=Active eq true` to find the customers whose example calculation would return `Active`. For other calculations, narrow the collection with supported server-side filters and select the calculated result for the records you need. Apply any remaining filtering or sorting in your application. If you need to sort the complete filtered set, follow [paging](../querying-data/paging.md) before applying the final sort.

See the [Technical Documentation](https://docs.erp.net/tech/advanced/calculated-attributes/index.html) for configuration and [calculated attribute examples](../../scripting/examples/calculated-attributes.md) for JavaScript formulas.
