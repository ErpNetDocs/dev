# $select query option

## Description

$select is an OData system query option.

The $select system query option allows clients to request a specific set of properties for each entity or complex type.

For a great introduction to $select, read the [OData $filter tutorial](https://www.odata.org/getting-started/basic-tutorial/#select).

## $expand

The $select query option is often used in conjunction with the $expand system query option, to define the extent of the resource graph to return ($expand) and then specify a subset of properties for each resource in the graph ($select).
Expanded navigation properties MUST be returned, even if they are not specified as a selectItem.

## Default attributes

When there is no $select clause or '$select=*', only the default attributes are returned.

The attributes, which are returned by default are:

* System attributes like **Name**, **Description**, **PartNumber**, etc.
* References like **ProductType**, **ProductGroup**, etc (in OData terminology - Navigation properties)

The following attributes are not returned by default:

* The 'Id' attribute
* Custom (user-defined) attributes
* Child lists (OData terminology: Collection navigation properties)
* Calculated attributes

Example:

```odata
GET ~/General_Products_ProductTypes?$top=2
```

The result is:

```json
{
    "@odata.context": "https://example-server.com/example-db/api/domain/odata/$metadata#General_Products_ProductTypes",
    "value": [
        {
            "@odata.id": "General_Products_ProductTypes(c696c660-9aa4-4fe5-a396-126af4101792)",
            "IsDefault": false,
            "IsFixedAsset": false,
            "IsServiceActivityService": false,
            "IsServiced": true,
            "IsShipped": true,
            "IsStocked": true,
            "LotAutoCreation": true,
            "Code": "001",
            "Name": {
                "BG": "sdds"
            }
        },
        {
            "@odata.id": "General_Products_ProductTypes(880b0c31-a9ef-4a3c-a0e7-13d39aa57464)",
            "IsDefault": false,
            "IsFixedAsset": false,
            "IsServiceActivityService": false,
            "IsServiced": false,
            "IsShipped": false,
            "IsStocked": false,
            "LotAutoCreation": false,
            "Code": "test sch",
            "Name": {
                "BG": "test sch"
            }
        }
    ]
}
```

> [!note]
> Non default properties must be explicitly specified in the $select clause.

Example:

```odata
GET ~/General_Products_Products?$top=2&$select=CustomProperty_WebName,CalculatedAttribute_ExampleAttr
```

The result will only contain the selected properties.

## The **default** $select keyword

You can explicitly use the keyword **default** in the $select clause to include all default properties.

```odata
GET ~/General_Products_Products?$top=2&$select=default,CustomProperty_WebName,CalculatedAttribute_ExampleAttr
```

The result will contain all default properties plus the selected CustomProperty_WebName and CalculatedAttribute_ExampleAttr.