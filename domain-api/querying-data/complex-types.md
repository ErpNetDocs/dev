## Handling Complex Data Types

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

Example:

[General_Products_Products?$filter=contains(Name,'ap')&$top=10](https://testdb.my.erp.net/api/domain/query?General_Products_Products?$filter=contains(Name,'ap')&$top=10)


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


Example:

To filter by Quantity or Amount value you can use the special (internal) property named {Property}Value.
For example in Crm_Sales_SalesOrderLines we have Quantity property. We can filter by quantity value:

[Crm_Sales_SalesOrderLines?$top=10&$filter=QuantityValue ge 10](https://testdb.my.erp.net/api/domain/query?GET+Crm_Sales_SalesOrderLines?$top=10&$filter=QuantityValue ge 10)


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

Examples:

The enum values are represented as string. You can filter by enum value like this

[Crm_Sales_SalesOrders?$top=10&$filter=State eq 'Released'&$orderby=DocumentDate desc](https://testdb.my.erp.net/api/domain/query?GET+Crm_Sales_SalesOrders?$top=10&$filter=State+eq+%27Released%27&$orderby=DocumentDate+desc)

To filter by reference use it's odata id:

[Crm_Sales_SalesOrders?$top=10&$filter=DocumentType eq 'Systems_Documents_DocumentTypes(469b67b1-8b4b-4fb4-9d97-20c96105a85a)'](https://testdb.my.erp.net/api/domain/query?GET+Crm_Sales_SalesOrders?$top=10&$filter=DocumentType+eq+%27Systems_Documents_DocumentTypes(469b67b1-8b4b-4fb4-9d97-20c96105a85a)%27)


## For more information see [Complex Types Topic](../complex-types/index.md)