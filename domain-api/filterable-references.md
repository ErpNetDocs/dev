# Filterable References

## Overview

Query filters allow each attribute from the entity to be filtered.
References also can be filtered, by equalling them to single or multiple instances of the referenced entity.

Sometimes however, we don't know the exact referened entity, but want to filter by the attributes of the referenced entity.

> [!note]
> In SQL terms, this is very similar to JOIN-ing the referenced table and then filtering in the WHERE by some of the columns of the referenced table.

For example, in the [Customers Entity](xref:Crm.Customers), we might want to filter by the attributes of the related [Parties Entity](xref:General.Contacts.Parties).

## Finding out if a reference is filterable

Because of the static way the SQL data access layer is built (using only Stored Procedures), filtering by the attributes of a referenced entity is not always possible.
To be able to filter by a reference, the auto-generated SQL procedures should already JOIN the referenced table.
For this reason, only a handful of the referenes support filtering.

> [!note]
> Ownership references are ALWAYS filterable.
> For example, starting from [SalesOrderLines Entity](xref:Crm.Sales.SalesOrderLines), you can filter by the attributes of the [SalesOrder](xref:Crm.Sales.SalesOrders).

To find out if an attribute supports filtering, look for the **FilterableReference** tag in the attribute details.
For example, see how StoreOrderLine reference is filterable:

<https://docs.erp.net/model/entities/Crm.Customers.html#party>

## Usage

To filter the customers by the attributes of the related party, you can use the following query:

