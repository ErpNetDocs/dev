# Querying Data

The **Querying Data** section explains how to retrieve information from ERP.net using the Domain API. All queries use standard OData conventions, with some ERP.net-specific extensions.

## OData Basics

ERP.net exposes entities (repositories) through an OData-compliant service. Each entity represents a type of data in the system, such as `Customer`, `Order`, or `Product`.

- **Service Root URL:** The base URL for all queries. Example: https://testdb.my.erp.net/api/domain/odata/  

- **Entity Types:** Each entity can be queried independently. Common entities include:
- [Crm_Sales_Customers](https://docs.erp.net/model/entities/Crm.Sales.Customers.html)
- [Crm_Sales_SalesOrders](https://docs.erp.net/model/entities/Crm.Sales.SalesOrders.html)
- [General_Products_Products](https://docs.erp.net/model/entities/General.Products.Products.html)

## In This Section

* [Query Options](query-options/index.md)  
  Explains the URL parameters used for querying data.

* [Paging results](./paging.md)  
  How to page large result sets using `$top` and `@odata.nextLink` (`$skiptoken` vs `$skip`).

* [Query Builder](./query-builder.md)  
  ERP.net includes a **Visual Query Designer** to simplify query creation

* [Query Tool](./query-tool.md)  
  The **ERP.net Query Tool** allows you to experiment with Domain API requests directly in the browser.

* [Filterable References](./filterable-references.md)  
  Filter by properties of a referenced object.

* [Examples](../complex-types/index.md)  
  Useful examples and common tasks.



