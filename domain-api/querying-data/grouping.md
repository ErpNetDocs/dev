# Grouping and aggregation

## Overview

OData grouping and aggregation allows Domain API clients to request summary data directly from the server. It uses the OData `$apply` query option with the `groupby` and `aggregate` transformations.

Use it when the client needs business summaries such as sales amount by customer, sold quantity by product, or document totals from document lines. The service evaluates the matching records, groups them, and returns one result object per group.

ERP.net supports the standard OData aggregation syntax where possible and adds several convenience extensions for Domain API usage:

- grouping by reference-valued properties, such as `Product` or `SalesOrder/Customer`;
- `expandgroup`, which expands grouped reference keys in the returned open-object result;
- `yearmonth(...)`, which groups date values by calendar year and month.

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
- reference properties through supported references, such as `SalesOrder/Customer`;
- computed interval keys declared with `compute(... as Alias)`, such as `yearmonth(DocumentDate) as YearMonth`.

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

Each source property can appear only once in `groupby(...)` and only once in `aggregate(...)`.

### Direct aggregation

Use `aggregate(...)` without `groupby(...)` when the client needs totals over the filtered source rows and no explicit group keys.

```http
GET /domain/odata/Crm_Sales_SalesOrderLines?$apply=aggregate(LineAmount with sum as TotalAmount)
```

Example result:

```json
{
  "value": [
    {
      "TotalAmount": {
        "Value": 98770.82,
        "Currency": "EUR"
      }
    },
    {
      "TotalAmount": {
        "Value": 3904225639.96,
        "Currency": "BGN"
      }
    }
  ]
}
```

The result is still returned as a collection of open objects. For plain scalar aggregates, it normally contains one object. For Amount and Quantity aggregates, the service still separates results by measure. This means a direct amount aggregate returns one row per currency, and a direct quantity aggregate returns one row per measurement unit.

Normal `$filter` is evaluated before the aggregate.

```http
GET /domain/odata/Crm_Sales_SalesOrderLines?$filter=RequiredDeliveryDate ge 2026-01-01&$apply=aggregate(LineAmount with sum as TotalAmount,$count as Count)
```

`expandgroup` is not used with direct aggregation because there are no grouped reference keys to expand.

### Group intervals

Use `compute` before `groupby` when the grouped key should be a date or string interval instead of the exact source value.

```http
$apply=compute(IntervalExpression as Alias)/groupby((Alias),aggregate(Expression with Function as AggregateAlias))
```

Example result:

```json
{
  "value": [
    {
      "Alias": "2026-01",
      "AggregateAlias": 42
    }
  ]
}
```

Supported interval expressions:

| Expression | Source value | Grouped value |
| --- | --- | --- |
| `year(DateProperty)` | date or date-time | calendar year, such as `2026` |
| `yearmonth(DateProperty)` | date or date-time | calendar year and month, such as `2026-01` |
| `date(DateProperty)` | date or date-time | calendar date, such as `2026-01-31` |
| `substring(StringProperty,0,1)` | string | first character of the string |

`yearmonth` is an ERP.net extension. It should be used when the client needs monthly grouping by year and month. The standard `month(...)` function is not supported for grouped interval keys because it does not include the year.

The interval alias becomes the grouped key in the result. Interval keys are returned as strings.

Example: sales amount by document month.

```http
GET /domain/odata/Crm_Sales_SalesAnalytics?$apply=compute(yearmonth(DocumentDate) as YearMonth)/groupby((YearMonth),aggregate(LineAmount with sum as TotalAmount))
```

Example result:

```json
{
  "value": [
    {
      "YearMonth": "2026-01",
      "TotalAmount": {
        "Value": 12948.77,
        "Currency": "BGN"
      }
    }
  ]
}
```

Example: line count by delivery date.

```http
GET /domain/odata/Crm_Sales_SalesOrderLines?$apply=compute(date(RequiredDeliveryDate) as DeliveryDate)/groupby((DeliveryDate),aggregate($count as Count))
```

Example result:

```json
{
  "value": [
    {
      "DeliveryDate": "2026-01-31",
      "Count": 18
    }
  ]
}
```

Example: customers by first character of their number.

```http
GET /domain/odata/Crm_Sales_Customers?$apply=compute(substring(Number,0,1) as NumberInitial)/groupby((NumberInitial),aggregate($count as Count))
```

Example result:

```json
{
  "value": [
    {
      "NumberInitial": "A",
      "Count": 128
    }
  ]
}
```

Interval expressions are supported only as computed keys for `groupby`. They cannot be used as aggregate expressions or as general-purpose computed output values in grouped results.

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

#### One use per source property

A source property can be used only once in `groupby(...)` and only once in `aggregate(...)`.

This means that the same property cannot be grouped more than once with different intervals, and the same property cannot be aggregated more than once with different functions or aliases.

Interval group keys follow the same rule. For example, do not group by both `year(DocumentDate)` and `yearmonth(DocumentDate)` in the same grouped query.

Supported:

```http
$apply=groupby((Product),aggregate(LineAmount with sum as TotalAmount))
```

Not supported:

```http
$apply=groupby((DocumentDate,DocumentDate),aggregate($count as Count))
```

Not supported:

```http
$apply=compute(year(DocumentDate) as Year,yearmonth(DocumentDate) as YearMonth)/groupby((Year,YearMonth),aggregate($count as Count))
```

Not supported:

```http
$apply=groupby((Product),aggregate(LineAmount with sum as TotalAmount,LineAmount with max as MaxAmount))
```

If several aggregate values are needed from the same source value, execute separate grouped queries.

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

- `$apply=aggregate(...)`;
- `$apply=groupby(...)`;
- `aggregate(... with sum as Alias)`;
- `aggregate($count as Alias)`;
- `compute(date(Property) as Alias)/groupby((Alias),...)`;
- `compute(year(Property) as Alias)/groupby((Alias),...)`;
- `compute(substring(Property,0,1) as Alias)/groupby((Alias),...)`;
- path group keys such as `SalesOrder/Customer`.

ERP.net-specific behavior:

- grouping by reference-valued properties, such as `Product`;
- expanding grouped reference keys with `expandgroup`;
- returning measured aggregate values as complex Amount or Quantity objects;
- returning separate grouped rows for different currencies or measurement units;
- allowing only reference paths supported by the Domain API model;
- `yearmonth(Property)` for calendar month grouping by year and month.

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

### Month grouping does not work as expected

Symptom: the client needs monthly grouping by document date, but `month(DocumentDate)` is rejected or would not distinguish the same month in different years.

Cause: grouped interval keys support the ERP.net `yearmonth(...)` function for monthly grouping. It returns both year and month.

Fix: use `yearmonth(DateProperty)` in a `compute` transformation before `groupby`.

```http
GET /domain/odata/Crm_Sales_SalesAnalytics?$apply=compute(yearmonth(DocumentDate) as YearMonth)/groupby((YearMonth),aggregate(LineAmount with sum as TotalAmount))
```

Example result:

```json
{
  "value": [
    {
      "YearMonth": "2026-01",
      "TotalAmount": {
        "Value": 12948.77,
        "Currency": "BGN"
      }
    }
  ]
}
```

Prevention: use `yearmonth(...)` for month summaries and `year(...)` only for year summaries.

### Amount or quantity results contain more rows than expected

Symptom: the query groups by one key, such as `Product`, but the result contains more than one row for the same key.

Cause: Amount and Quantity aggregates are automatically separated by their measure. Amounts with different currencies and quantities with different measurement units are returned as separate grouped rows, even when currency or unit is not listed in `groupby(...)`.

Verification: compare the `Currency` value of the amount aggregate or the `Unit` value of the quantity aggregate in the repeated rows.

```http
GET /domain/odata/Crm_Sales_SalesOrderLines?$apply=groupby((Product),aggregate(LineAmount with sum as TotalAmount))
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
    },
    {
      "Product": {
        "Id": "4452392f-9ec6-4724-92e4-00004f6ef7ec"
      },
      "TotalAmount": {
        "Value": 420.00,
        "Currency": "EUR"
      }
    }
  ]
}
```

Fix: treat the measure as part of the grouped result. If a single value is required, filter the source records to one currency or unit, or convert the values to a common measure before aggregating them.

Prevention: when aggregating Amount or Quantity values, design the client result grid or chart as if currency or unit is part of the grouping key.

### `$top` does not return the largest groups

Symptom: `$top=10` returns ten grouped rows, but they are not the ten largest totals.

Cause: grouped queries do not currently support semantic server-side ordering by aggregate aliases.

Verification: remove `$top` and compare the result ordered locally by the desired aggregate.

Fix: materialize the grouped result and order it on the client.

Prevention: do not use `$top` as a "top N by amount" query until grouped `$orderby` support is explicitly added.





