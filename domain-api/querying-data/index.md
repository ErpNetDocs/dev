## 2. Querying Data

The **Querying Data** section explains how to retrieve information from ERP.net using the Domain API. All queries use standard OData conventions, with some ERP.net-specific extensions.

### 2.1 OData Basics

ERP.net exposes entities (repositories) through an OData-compliant service. Each entity represents a type of data in the system, such as `Customer`, `Order`, or `Product`.

- **Service Root URL:** The base URL for all queries. Example:  

- **Entity Types:** Each entity can be queried independently. Common entities include:
- [Crm_Sales_Customers](https://docs.erp.net/model/entities/Crm.Sales.Customers.html)
- [Crm_Sales_SalesOrders](https://docs.erp.net/model/entities/Crm.Sales.SalesOrders.html)
- [General_Products_Products](https://docs.erp.net/model/entities/General.Products.Products.html)

### 2.2 Query Options

ERP.net supports standard OData query options:

- **$filter:** Filter data using conditions.  
Example: Retrieve orders where `Status eq 'Open'`.

- **$select:** Retrieve only specific fields to reduce payload.  
Example: `?$select=Id,Name,Status`

- **$expand:** Include related entities in the response.  
Example: `?$expand=Customer,Lines`

- **$orderby:** Sort the results by a specific field.  
Example: `?$orderby=DocumentDate desc`

- **$top / $skip:** Control pagination.  
Example: `?$top=50&$skip=100`

- **$count:** Get the total number of records matching the query.  
Example: `?$count=true`

- **$search:** Search by DisplayText of the specific entity.  
Example: `?$search="Widget"`

Here is an extended topic for [Query Options](query-options/index.md)



### 2.3 Query Builder

ERP.net includes a **Visual Query Designer** to simplify query creation. For more examples and instructions, see [Query Builder](query-builder.md).

1. Select the entity type to query.  
2. Choose fields and relationships.  
3. Add filters, sorting, and pagination.  
4. Preview and execute the query.

### 2.4 Query Tool

ERP.net provides a **Query Tool** to quickly test and explore queries in your browser. You can run OData queries directly against your service without writing code.  

**Example:** Retrieve the top 10 products and include their product groups, sorted by part number:  

https://testdb.my.erp.net/api/domain/query?GET+General_Products_Products?$top=10&$expand=ProductGroup&$orderby=PartNumber

Use this tool to experiment with `$filter`, `$select`, `$expand`, `$orderby`, and other query options interactively.


**Example Queries:**

https://testdb.my.erp.net/api/domain/odata/Crm_Sales_Customers?$top=5&$select=Party,Number,CreditLimit&$expand=Party

https://testdb.my.erp.net/api/domain/odata/General_Products_Products?$top=10&$expand=ProductGroup&$orderby=PartNumber

### 2.5 Handling Complex Data Types

Some entities include complex or multi-part data types. For detailed explanations and examples, see [Complex Data Types](../complex-types/index.md).

- **Multi-language Strings:** Some fields may have different values for each language. These fields are represented as a complex object with one property per language. For example:  

```
{
    "Name": {
        "EN": "Laptop",
        "DE": "Laptop",
        "FR": "Ordinateur portable"
    }
}
```

Use the property corresponding to the desired language when displaying or processing the value.

> **NOTE:** Multi-language fields support only filtering with the `contains(Field, 'value')` function. Other filter operators are not supported.


- **Quantities and Amounts:** Fields with units (e.g., `Quantity`, `Amount`) require attention to precision and currency. Example:

```
"Quantity": {
    "Value": 12.5,
    "Unit": "kg",
}
"Amount": {
    "Value": 250.00,
    "Currency": "USD"
}
```

- **Enums and References:** Some fields reference other entities or use enumerated values. Use `$expand` or lookups to resolve them. Example:

```
{
    "State": "New",
    "Customer": {
        "Id": 123,
        "Number": "009987"
    }
}
```
The enum values are represented as string. You can filter by enum value like this

[Crm_Sales_SalesOrders?$top=10&$filter=State eq 'Released'&$orderby=DocumentDate desc](https://testdb.my.erp.net/api/domain/query?GET+Crm_Sales_SalesOrders?$top=10&$filter=State+eq+%27Released%27&$orderby=DocumentDate+desc)

To filter by reference use it's odata id:

[Crm_Sales_SalesOrders?$top=10&$filter=DocumentType eq 'Systems_Documents_DocumentTypes(469b67b1-8b4b-4fb4-9d97-20c96105a85a)'](https://testdb.my.erp.net/api/domain/query?GET+Crm_Sales_SalesOrders?$top=10&$filter=DocumentType+eq+%27Systems_Documents_DocumentTypes(469b67b1-8b4b-4fb4-9d97-20c96105a85a)%27)


### 2.6 Filterable References

Some ERP.net entities include **reference fields**, which point to other entities. In OData queries, you can filter data based on properties of the referenced entity.  

**Example:** Retrieve sales order lines where the parent order's document date is on or after October 23, 2025:

```
Crm_Sales_SalesOrderLines?$filter=SalesOrder/DocumentDate ge 2025-10-23T00:00:00
```

> **Concept:** The filter navigates through the reference (`SalesOrder`) to access a property (`DocumentDate`) of the related entity.

**Important:** Not all reference fields support filtering. Only certain reference fields are filterable. For more information see [Filterable References](filterable-references.md).



### 2.7 Batch Requests

ERP.net supports <a href="https://www.odata.org/getting-started/advanced-tutorial/#batch" target="_blank">batch requests</a>, allowing multiple queries in a single HTTP request:

- Combine multiple read or write requests into one batch.  
- Reduce network overhead and improve performance.  
- Example use cases: retrieving multiple entity types or related data simultaneously.

**Best Practices:**

- Keep batches reasonably sized to avoid timeouts. 
