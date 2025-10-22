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

[Query Options](query-options/index.md)  
Explains the URL parameters used for querying data.

[Query Builder](./query-builder.md)  
ERP.net includes a **Visual Query Designer** to simplify query creation

[Query Tool](./query-tool.md)  
The **ERP.net Query Tool** allows you to experiment with Domain API requests directly in the browser.

[Filterable References](./filterable-references.md)
Filter by properties of a referenced object.


**Example Queries:**

https://testdb.my.erp.net/api/domain/odata/Crm_Sales_Customers?$top=5&$select=Party,Number,CreditLimit&$expand=Party

https://testdb.my.erp.net/api/domain/odata/General_Products_Products?$top=10&$expand=ProductGroup&$orderby=PartNumber

More examples you can find in the [Common Tasks](../common-tasks/index.md) topic.

### Batch Requests

ERP.net supports <a href="https://www.odata.org/getting-started/advanced-tutorial/#batch" target="_blank">batch requests</a>, allowing multiple queries in a single HTTP request:

- Combine multiple read or write requests into one batch.  
- Reduce network overhead and improve performance.  
- Example use cases: retrieving multiple entity types or related data simultaneously.

**Best Practices:**

- Keep batches reasonably sized to avoid timeouts. 
