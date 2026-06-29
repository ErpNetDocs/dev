# Grouping and aggregation

## Overview

OData grouping and aggregation allows Domain API clients to request summary data directly from the server. It uses the OData `$apply` query option with the `groupby` and `aggregate` transformations.

Use it when the client needs business summaries such as sales amount by customer, sold quantity by product, or document totals from document lines. The service evaluates the matching records, groups them, and returns one result object per group.

ERP.net supports the standard OData aggregation syntax where possible and adds two convenience extensions for Domain API usage:

- grouping by reference-valued properties, such as `Product` or `SalesOrder/Customer`;
- `expandgroup`, which expands grouped reference keys in the returned open-object result.

## Getting Started

> [!NOTE]
> The Crm_Sales_SalesOrderLines examples in this article are for demonstrating grouping syntax and result shapes. For real sales aggregation scenarios, prefer the [Crm_Sales_Analytics](https://docs.erp.net/model/entities/Crm.Sales.SalesAnalytics.html) view, which is designed for grouped sales data.

The shortest useful grouped query is a count per reference or scalar field.

```http
GET /domain/odata/Crm_Sales_SalesOrderLines?$apply=groupby((Product),aggregate($count as Count))
```

Example result:

```json
{
  "value": [
    {
      "Product": {
        "Id": "4452392f-9ec6-4724-92e4-00004f6ef7ec"
      },
      "Count": 12
    }
  ]
}
```

A more useful query is sales amount by customer:

```http
GET /domain/odata/Crm_Sales_SalesOrderLines?$apply=groupby((SalesOrder/Customer),aggregate($count as LinesCount,LineAmount with sum as TotalAmount))
```

Example result:

```json
{
  "value": [
    {
      "SalesOrder": {
        "Customer": {
          "Id": "52ffd30a-f863-e311-8784-00155d001f00"
        }
      },
      "LinesCount": 4,
      "TotalAmount": {
        "Value": 12948.77,
        "Currency": "BGN"
      }
    }
  ]
}
```

This groups sales order lines by the customer of their sales order and returns the number of lines and the total line amount for each customer.

To include display data for the grouped customer, use the ERP.net `expandgroup` extension:

```http
GET /domain/odata/Crm_Sales_SalesOrderLines?$apply=groupby((SalesOrder/Customer),aggregate($count as LinesCount,LineAmount with sum as TotalAmount))&expandgroup=SalesOrder/Customer($select=Number,DisplayText)
```

Example result:

```json
{
  "value": [
    {
      "SalesOrder": {
        "Customer": {
          "@odata.id": "Crm_Sales_Customers(52ffd30a-f863-e311-8784-00155d001f00)",
          "Number": "A10823",
          "DisplayText": "APPLE DISTRIBUTION INTERNATIONAL"
        }
      },
      "LinesCount": 4,
      "TotalAmount": {
        "Value": 12948.77,
        "Currency": "BGN"
      }
    }
  ]
}
```

A practical product-and-customer summary can group by more than one key:

```http
GET /domain/odata/Crm_Sales_SalesOrderLines?$top=10&$apply=groupby((Product,SalesOrder/Customer),aggregate($count as Count,LineAmount with sum as TotalAmount))&expandgroup=Product($select=Id,DisplayText),SalesOrder/Customer($select=Number,DisplayText)
```

Example result:

```json
{
  "value": [
    {
      "Count": 1,
      "Product": {
        "@odata.id": "General_Products_Products(4452392f-9ec6-4724-92e4-00004f6ef7ec)",
        "Id": "4452392f-9ec6-4724-92e4-00004f6ef7ec",
        "DisplayText": "CHS/Women/Caps/Gorros(Caps)/KG47301X"
      },
      "SalesOrder": {
        "Customer": {
          "@odata.id": "Crm_Sales_Customers(52ffd30a-f863-e311-8784-00155d001f00)",
          "Number": "A10823",
          "DisplayText": "APPLE DISTRIBUTION INTERNATIONAL"
        }
      },
      "TotalAmount": {
        "Value": 0,
        "Currency": "BGN"
      }
    }
  ]
}
```

Expected outcome:

- each returned row represents one product/customer group;
- `Count` contains the number of source lines in the group;
- `TotalAmount` contains the sum of `LineAmount` for the group;
- `Product` and `SalesOrder.Customer` are expanded according to `expandgroup`.

`$top` limits the number of grouped rows returned. It does not mean "top by amount" because grouped results currently do not support semantic server-side ordering.

Type annotations such as `@odata.type` may appear depending on the requested OData metadata level. They are omitted from most examples for readability.

## Configuration

### `$apply`

Use `$apply` to define the grouped result.

```http
$apply=groupby((GroupKey1,GroupKey2),aggregate(Expression with Function as Alias))
```

Example result:

```json
{
  "value": [
    {
      "GroupKey1": "Example group value",
      "GroupKey2": "Another group value",
      "Alias": 42
    }
  ]
}
```

Supported grouping inputs:

- scalar dimension properties of the source entity, such as `RequiredDeliveryDate`;
- reference properties of the source entity, such as `Product`;
- reference id paths, such as `Customer/Id`;
- dimension properties through supported references, such as `SalesOrder/DocumentNo`;
- reference properties through supported references, such as `SalesOrder/Customer`.

A supported reference path can follow only ownership references and filterable references. This rule applies to every reference segment, including the first segment from the source entity. Ordinary references that are not marked as ownership or filterable cannot be used in grouped keys.

The entity documentation page shows which references are available for filtering. Those are the references that can also be used as grouped path segments, together with ownership references. The grouped query does not join arbitrary navigation paths or child collections.

Reference grouping is an ERP.net extension over standard OData aggregation. Grouping by a reference path groups by the reference id in the database and returns the reference as a grouped key. The key can then be expanded with `expandgroup`.

Examples:

| Group key | Meaning |
| --- | --- |
| `Product` | groups sales lines by product |
| `Product/Id` | groups sales lines by product id |
| `SalesOrder/DocumentNo` | groups sales lines by their sales order document number |
| `SalesOrder/Customer` | groups sales lines by the customer of their sales order |

Supported aggregate functions:

- `$count as Alias`;
- `Property with sum as Alias`;
- `Property with min as Alias` (measure properties only);
- `Property with max as Alias` (measure properties only);
- `Property with countdistinct as Alias`.

### `expandgroup`

`expandgroup` is an ERP.net extension query option. It is used only for grouped results.

```http
expandgroup=GroupedReferencePath($select=Property1,Property2)
```

Example result:

```json
{
  "value": [
    {
      "GroupedReferencePath": {
        "Property1": "Value 1",
        "Property2": "Value 2"
      },
      "Count": 1
    }
  ]
}
```

It accepts the same item shape as OData `$expand`, including deep paths and nested expand options.

```http
expandgroup=SalesOrder/Customer($select=Number,DisplayText;$expand=Party($select=PartyName))
```

Example result:

```json
{
  "value": [
    {
      "SalesOrder": {
        "Customer": {
          "Number": "A10823",
          "DisplayText": "APPLE DISTRIBUTION INTERNATIONAL",
          "Party": {
            "PartyName": "APPLE DISTRIBUTION INTERNATIONAL"
          }
        }
      },
      "Count": 1
    }
  ]
}
```

Use `expandgroup` instead of `$expand` for grouped keys. Standard `$expand` is parsed against the source entity type, while grouped results are returned as open objects.

### Paging

Grouped queries can use `$top` and `$skip` only as technical row-window controls. They are useful for chunked retrieval when the client wants to stream or process a large grouped result in smaller portions.

```http
GET /domain/odata/Crm_Sales_SalesOrderLines?$skip=100&$top=100&$apply=groupby((Product),aggregate(LineAmount with sum as TotalAmount))
```

Example result:

```json
{
  "value": [
    {
      "Product": {
        "Id": "4452392f-9ec6-4724-92e4-00004f6ef7ec"
      },
      "TotalAmount": {
        "Value": 12948.77,
        "Currency": "BGN"
      }
    }
  ]
}
```

Grouped results have no guaranteed business order. Because `$orderby` is not supported together with `$apply=groupby(...)`, `$top` and `$skip` must not be used as semantic paging over a user-visible ordered result.

Do not interpret `$top=10` as "top 10 by amount", "top 10 by count", or any other ranked result. Use it only to retrieve the grouped result in chunks. If a specific user-visible order is required, materialize the grouped result and order it on the client.

## Concepts

### Result shape

Grouped OData results are returned as `Erp.OpenObject` values. Aggregate aliases become open properties on each result object.

```json
{
  "TotalAmount": {
    "Value": 12948.77,
    "Currency": "BGN"
  },
  "SalesOrder": {
    "Customer": {
      "Id": "52ffd30a-f863-e311-8784-00155d001f00"
    }
  }
}
```

When the grouped key is a path, the result keeps the same path shape as nested open objects. For example, grouping by `SalesOrder/Customer` returns a `SalesOrder` object that contains a `Customer` object.

When a grouped reference is expanded with `expandgroup`, the reference value can be returned as an entity object with the requested selected and expanded properties.

### Reference grouping

Standard OData grouping is based on property paths. ERP.net additionally supports grouping by reference-valued properties as a Domain API convenience.

```http
GET /domain/odata/Crm_Sales_SalesOrderLines?$apply=groupby((Product),aggregate($count as Count))&expandgroup=Product($select=Id,DisplayText)
```

Example result:

```json
{
  "value": [
    {
      "Product": {
        "@odata.id": "General_Products_Products(4452392f-9ec6-4724-92e4-00004f6ef7ec)",
        "Id": "4452392f-9ec6-4724-92e4-00004f6ef7ec",
        "DisplayText": "CHS/Women/Caps/Gorros(Caps)/KG47301X"
      },
      "Count": 12
    }
  ]
}
```

ERP.net groups by the identity of the reference but returns the key as the reference value. This allows clients to work with domain concepts without resolving reference ids in a separate request.

Nested reference grouping is also supported when the path is supported by the model:

```http
GET /domain/odata/Crm_Sales_SalesOrderLines?$apply=groupby((SalesOrder/Customer),aggregate(LineAmount with sum as TotalAmount))&expandgroup=SalesOrder/Customer($select=Number,DisplayText)
```

Example result:

```json
{
  "value": [
    {
      "SalesOrder": {
        "Customer": {
          "@odata.id": "Crm_Sales_Customers(52ffd30a-f863-e311-8784-00155d001f00)",
          "Number": "A10823",
          "DisplayText": "APPLE DISTRIBUTION INTERNATIONAL"
        }
      },
      "TotalAmount": {
        "Value": 12948.77,
        "Currency": "BGN"
      }
    }
  ]
}
```

Grouping by a reference id path is supported when only the id is needed:

```http
GET /domain/odata/Crm_Sales_SalesAnalytics?$apply=groupby((Customer/Id),aggregate(LineAmount with sum as Total))
```

Example result:

```json
{
  "value": [
    {
      "Customer": {
        "Id": "beb5455b-4436-e311-81cb-00155d001f00"
      },
      "Total": {
        "Value": 12948.77,
        "Currency": "BGN"
      }
    }
  ]
}
```

For repositories where the id attribute has a domain-specific name, use that public id property name. For example, if the referenced entity exposes `PartyId` as its id attribute, use `Party/PartyId`, not `Party/Id`.

### Paths through references

Grouped keys can use paths through references only when every reference segment in the path is supported for grouped queries.

A reference segment is supported when it is one of these:

- an ownership reference;
- a filterable reference, as shown in the entity documentation.

Ordinary references that are not ownership references and are not listed as filterable cannot be used in grouped key paths. Child collections are not supported in grouped key paths.

Examples:

| Group key | Supported when | Meaning |
| --- | --- | --- |
| `SalesOrder/DocumentNo` | `SalesOrder` is an ownership or filterable reference | groups sales order lines by sales order document number |
| `SalesOrder/Customer` | `SalesOrder` is supported and `Customer` is an ownership or filterable reference from sales orders | groups sales order lines by sales order customer |
| `Product` | `Product` is an ownership or filterable reference from sales order lines | groups sales order lines by product |

Unsupported examples:

| Group key | Why it is rejected |
| --- | --- |
| `SomeReference/Name` | `SomeReference` is not ownership or filterable |
| `SalesOrder/SomeReference/Name` | `SomeReference` is not ownership or filterable from sales orders |
| `Lines/Product` | child collections are not supported in grouped key paths |

```http
GET /domain/odata/Crm_Sales_SalesOrderLines?$apply=groupby((SalesOrder/DocumentNo),aggregate($count as Count))
```

Example result:

```json
{
  "value": [
    {
      "SalesOrder": {
        "DocumentNo": "SO000001"
      },
      "Count": 3
    }
  ]
}
```

### Filters before grouping

Normal `$filter` is evaluated before grouping. Records are filtered first, then grouped and aggregated.

```http
GET /domain/odata/Crm_Sales_SalesOrderLines?$filter=RequiredDeliveryDate ge 2026-01-01&$apply=groupby((Product),aggregate(LineAmount with sum as TotalAmount))
```

Example result:

```json
{
  "value": [
    {
      "Product": {
        "Id": "4452392f-9ec6-4724-92e4-00004f6ef7ec"
      },
      "TotalAmount": {
        "Value": 1250.00,
        "Currency": "BGN"
      }
    }
  ]
}
```

Use this for questions such as "sales amount by product for deliveries after a date".

### Groupable and aggregatable properties

Not every property can be used in `groupby` or `aggregate`. The Domain API model defines which properties are suitable for grouping and which are suitable for aggregation.

Properties used in `groupby` must be dimensions. Dimensions are values that can safely identify groups, such as references, dates, booleans, enum-like values, short strings, and other model-supported grouping values.

Properties used with numeric aggregate functions must be measures. Measures are values that can be summarized, such as amounts, quantities, and other numeric business values.

Common supported combinations:

| Query use | Supported property kind |
| --- | --- |
| `groupby(...)` | dimensions |
| `sum` | numeric measures, Amount, Quantity |
| `min`, `max` | measures only |
| `countdistinct` | supported dimensions and measures |

Example grouping by dimensions and aggregating a measure:

```http
GET /domain/odata/Crm_Sales_SalesOrderLines?$apply=groupby((Product,SalesOrder/Customer),aggregate(LineAmount with sum as TotalAmount))
```

Example result:

```json
{
  "value": [
    {
      "Product": {
        "Id": "4452392f-9ec6-4724-92e4-00004f6ef7ec"
      },
      "SalesOrder": {
        "Customer": {
          "Id": "52ffd30a-f863-e311-8784-00155d001f00"
        }
      },
      "TotalAmount": {
        "Value": 12948.77,
        "Currency": "BGN"
      }
    }
  ]
}
```

Example distinct count over a dimension:

```http
GET /domain/odata/Crm_Sales_SalesOrderLines?$apply=groupby((SalesOrder/Customer),aggregate(Product with countdistinct as DistinctProducts))
```

Example result:

```json
{
  "value": [
    {
      "SalesOrder": {
        "Customer": {
          "Id": "52ffd30a-f863-e311-8784-00155d001f00"
        }
      },
      "DistinctProducts": 17
    }
  ]
}
```

If a property is not supported for the requested grouping or aggregate function, the service rejects the grouped query.


### Measured aggregate values

Amount and Quantity properties are value-with-measure domain values. When they are aggregated, the result keeps the measure information and is returned as a complex object, not as a plain decimal.

An amount aggregate contains the numeric value and the currency:

```http
GET /domain/odata/Crm_Sales_SalesOrderLines?$apply=groupby((SalesOrder/Customer),aggregate(LineAmount with sum as TotalAmount))
```

Example result:

```json
{
  "value": [
    {
      "SalesOrder": {
        "Customer": {
          "Id": "52ffd30a-f863-e311-8784-00155d001f00"
        }
      },
      "TotalAmount": {
        "Value": 12948.77,
        "Currency": "BGN"
      }
    }
  ]
}
```

A quantity aggregate contains the numeric value and the measurement unit:

```http
GET /domain/odata/Crm_Sales_SalesOrderLines?$apply=groupby((Product),aggregate(Quantity with sum as TotalQuantity))&expandgroup=Product($select=Id,DisplayText)
```

Example result:

```json
{
  "value": [
    {
      "Product": {
        "Id": "4452392f-9ec6-4724-92e4-00004f6ef7ec",
        "DisplayText": "CHS/Women/Caps/Gorros(Caps)/KG47301X"
      },
      "TotalQuantity": {
        "Value": 24,
        "Unit": "PCS"
      }
    }
  ]
}
```

> [!NOTE]
> Aggregating an amount or quantity always groups by its measure. Amounts with different currencies and quantities with different measurement units are returned as separate grouped rows, even when the explicit group keys are the same.

### Ordering restrictions

Grouped results do not currently support OData `$orderby`. A grouped query with `$orderby` is rejected by the service.

```http
GET /domain/odata/Crm_Sales_SalesOrderLines?$apply=groupby((Product),aggregate(LineAmount with sum as TotalAmount))&$orderby=TotalAmount desc
```

Example error:

```json
{
  "message": "OData $orderby is not supported together with $apply groupby.",
  "code": 0,
  "type": "NotSupportedException"
}
```

`$top` and `$skip` do not provide stable business paging for grouped queries. Use them only for chunked retrieval, then apply any required ordering on the client after all chunks are materialized.


### Standard OData and ERP.net extensions

Standard OData syntax used by this feature:

- `$apply=groupby(...)`;
- `aggregate(... with sum as Alias)`;
- `aggregate($count as Alias)`;
- path group keys such as `SalesOrder/Customer`.

ERP.net-specific behavior:

- grouping by reference-valued properties, such as `Product`;
- expanding grouped reference keys with `expandgroup`;
- returning measured aggregate values as complex Amount or Quantity objects;
- returning separate grouped rows for different currencies or measurement units;
- allowing only reference paths supported by the Domain API model.

## Troubleshooting

### `$expand` cannot find the grouped property

Symptom:

```text
Could not find a property named 'Customer' on type 'Erp.Crm_Sales_SalesOrderLine'.
```

Cause: `$expand` is parsed against the source entity type. After `$apply=groupby(...)`, the returned shape is an open object, not the original entity type.

Verification: the query groups by a path such as `SalesOrder/Customer`, but uses `$expand=Customer` or `$expand=SalesOrder/Customer`.

Fix: use `expandgroup` with the grouped key path.

```http
GET /domain/odata/Crm_Sales_SalesOrderLines?$apply=groupby((SalesOrder/Customer),aggregate($count as Count))&expandgroup=SalesOrder/Customer($select=Number,DisplayText)
```

Example result:

```json
{
  "value": [
    {
      "SalesOrder": {
        "Customer": {
          "Number": "A10823",
          "DisplayText": "APPLE DISTRIBUTION INTERNATIONAL"
        }
      },
      "Count": 1
    }
  ]
}
```

Prevention: use `$expand` for normal entity queries and `expandgroup` for grouped reference keys.

### Grouping by a reference fails to parse

Symptom:

```text
$apply/groupby grouping expression 'Customer' must evaluate to a property access value.
```

Cause: the grouped key is not a supported grouped reference path or cannot be interpreted as a property path by the OData parser.

Verification: check whether the grouped key is a valid source reference, a reference id path, or a supported path through a reference.

Fix: use a supported reference path, or use the public id property path when only the identity is needed.

```http
GET /domain/odata/Crm_Sales_SalesAnalytics?$apply=groupby((Customer/Id),aggregate(LineAmount with sum as Total))
```

Example result:

```json
{
  "value": [
    {
      "Customer": {
        "Id": "beb5455b-4436-e311-81cb-00155d001f00"
      },
      "Total": {
        "Value": 12948.77,
        "Currency": "BGN"
      }
    }
  ]
}
```

Prevention: group by the reference itself when the entity key is needed, and group by the public id property when only the identity value is needed.

### `$top` does not return the largest groups

Symptom: `$top=10` returns ten grouped rows, but they are not the ten largest totals.

Cause: grouped queries do not currently support semantic server-side ordering by aggregate aliases.

Verification: remove `$top` and compare the result ordered locally by the desired aggregate.

Fix: materialize the grouped result and order it on the client.

Prevention: do not use `$top` as a "top N by amount" query until grouped `$orderby` support is explicitly added.





