# Master/Detail Attributes

## Description

There are many entity types in @@name which are in master/detail relationship.

For example, the [Sales Orders](xref:Crm.Sales.SalesOrders) entity type is master to the [Sales Order Lines](xref:Crm.Sales.SalesOrderLines) entity type.

Following the relationship between their respective entity types, some attributes (or references) might be in master/detail relationship.
For example, the [SalesOrder.Store](xref:Crm.Sales.SalesOrders#store) and [SalesOrderLine.LineStore](xref:Crm.Sales.SalesOrderLines#linestore) are in master/detail relationship.

For in-depth information about master/detail attribute relationship, refer to the [technical documentation](xref:master-detail).

Very simplified version of the story from dev perspective:

* The **detail attribute** is the important one.
It is considered by the business logic.
The master attribute is used mainly for at-a-glance user information.
* The **master attribute** contains value only when the value (of the detail attribute) for all lines is the same. Otherwise, it is NULL.
* The detail attribute generally should be a required (not-NULL) attribute. However, there are exceptions, so check the model docs to be sure.

## Best practices

### When creating data

* Set the value of the detail attribute for each line, according to your business requirements.
* For the master attribute:
  * If ALL detail lines have the same value for the detail attribute, set that value.
  * Otherwise (if there are different values) - set NULL.

### When consuming data

* Use the detail attribute value to properly implement your business logic.
* If you are referring to the value of the master attribute, be sure to properly handle the possible NULL values.

## Examples

### All lines have the same value

In this example, the master attribute has a value, because it is the same for all lines.

Entity | Attribute | Value
-------|-----------|------
Sales Order 00596 | Store | Main
Sales Order 00596 - Line 01 | Line Store | Main
Sales Order 00596 - Line 02 | Line Store | Main
Sales Order 00596 - Line 03 | Line Store | Main

### Different values on the lines

In this example, the master attribute is NULL, because there are lines with different values.

Entity | Attribute | Value
-------|-----------|------
Sales Order 00597 | Store | **NULL**
Sales Order 00597 - Line 01 | Line Store | Main
Sales Order 00597 - Line 02 | Line Store | Remote 01
Sales Order 00597 - Line 03 | Line Store | Main
