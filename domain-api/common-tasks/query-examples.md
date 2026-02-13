# Query examples

This page contains practical examples of common OData query patterns used with ERP.net Domain API.

For query syntax reference and supported operators, see:
[Querying data](../querying-data/index.md).

## CONTAINS

Using `contains` for a string attribute:

```
General_Contacts_Parties?$top=5&$filter=contains(PartyCode,'30')
```

[Try it Yourself](https://testdb.my.erp.net/api/domain/query?GET+General_Contacts_Parties?$top=5&$filter=contains(PartyCode,%2730%27))

Using `contains` for a multi-language string attribute:

```
General_Contacts_Parties?$top=5&$filter=contains(PartyName,'Ivan')
```

[Try it Yourself](https://testdb.my.erp.net/api/domain/query?GET+General_Contacts_Parties?$top=5&$filter=contains(PartyName,%27Ivan%27))

## IN operator

Not every attribute supports the `in` operator. In general, navigation properties support `in`, some enum attributes support it, and some additional attributes may support it as well. Check the entity attribute metadata in the [Domain Model Reference](https://docs.erp.net/model/entities/).

Using `in` for `Id`:

```
General_Products_Products?$top=10&$filter=Id in (edf2bd2a-7e4d-e111-a06c-00155d00050a, cf728601-1fd5-4853-ab23-01deeee7d038)
```

[Try it Yourself](https://testdb.my.erp.net/api/domain/query?GET+General_Products_Products?$top=10&$filter=Id+in+(edf2bd2a-7e4d-e111-a06c-00155d00050a,+cf728601-1fd5-4853-ab23-01deeee7d038))

Using `in` for document state:

```
Crm_Sales_SalesOrders?$top=10&$filter=State in ('FirmPlanned', 'Released')&$select=State
```

[Try it Yourself](https://testdb.my.erp.net/api/domain/query?GET+Crm_Sales_SalesOrders?$top=10&$filter=State+in+(%27FirmPlanned%27,+%27Released%27)&$select=State)

Using `in` for a navigation property:

```
Crm_Sales_SalesOrders?$top=10&$select=DocumentType&$filter=DocumentType in ('General_DocumentTypes(de4913f3-962a-4289-a0f3-01bc2c1da21d)', 'General_DocumentTypes(a8b99412-3348-4c12-abdf-1a6a15ab5449)')
```

Passing inherited entity URI-s (e.g. `ToParty` is of base type `General_Contacts_Party` and may refer to `General_Contacts_Companies` or `General_Contacts_Persons`):

```
Crm_Sales_SalesOrders?$top=10&$filter=ToParty in ('General_Contacts_Persons(2e97f255-f410-4925-8c51-211c8eaa18b8)', 'General_Contacts_Companies(bc60d0bc-2804-4e3c-b355-04184aef5505)')
```

> [!note]
> If you pass URI-s to the base `General_Contacts_Parties` entity (when applicable), the query can often be optimized and be faster than specifying inherited entity URI-s.

## CAST

The following URI returns the parent document of a specified sales order cast as `Crm_Presales_Offer`.

> [!note]
> The type must be specified with the namespace which for all entities is `Erp`.

```
Crm_Sales_SalesOrders(11217345-3659-43be-a85d-005eaaa3aaac)/Parent/Erp.Crm_Presales_Offer
```

## $select=default

By default, only system attributes are present in the JSON result. The `Id` attribute, custom properties and calculated attributes are not present. Use the keyword `default` in the `$select` clause to include all default attributes. Custom properties and calculated attributes must be specified explicitly.

```
General_Products_Products?$top=1&$select=default,CalculatedAttribute_name
```

## Nested $expand

```
Crm_Sales_SalesOrders(59098bcf-f331-478f-91c2-f5520590f534)?$expand=Lines($expand=Product($select=Codes,Name,PartNumber;$expand=Codes($select=Code)))
```

## $expand $ref (returns items as links)

```
Crm_Sales_SalesOrders(59098bcf-f331-478f-91c2-f5520590f534)?$expand=Lines/$ref
```

## Filter by date

```
Crm_Sales_SalesOrders?$top=2&$filter=DocumentDate eq 2012-01-01T00:00:00Z
```

## Filter by stored attribute (custom property)

```
General_Products_Products?$top=10&$select=CustomProperty_color&$filter=CustomProperty_color eq 'blue'
```

> [!note]
> For stored attributes, filtering is supported by the short value (`Value`) and only `eq` is supported.

## Filter by quantity and amount

```
Crm_Sales_SalesOrderLines?$top=10&$filter=QuantityValue ge 3 and QuantityValue le 5 and LineAmountValue ge 15.45&$select=Quantity
```

## $count with filter

```
Crm_Invoicing_Invoices/$count?$filter=DocumentDate eq 2020-03-23T00:00:00Z
```

```
Crm_Sales_SalesOrderLines/$count?$filter=SalesOrder/Void eq false and SalesOrder/State ge 'Released' and Product eq 'General_Products_Products(35d5bcb9-0881-4bc8-bfe4-84fb874d4626)'
```

See OData URL conventions:
http://docs.oasis-open.org/odata/odata/v4.01/odata-v4.01-part2-url-conventions.html

## $top, $skip and $count=true

```
Crm_Sales_SalesOrderLines?$filter=SalesOrder/Void eq false and SalesOrder/State ge 'Released'&$top=10&$skip=120&$count=true
```

The `$count=true` query option specifies that the total number of rows for the specified filter will be included in the result along with the data.

## Filter by document state

```
Crm_Sales_SalesOrders?$top=1&$filter=State ge Erp.General_DocumentState'Released'
```

## Filter by MasterDocument (using Sales Order URI)

```
Crm_Sales_SalesOrders?$top=2&$filter=MasterDocument eq 'Crm_Sales_SalesOrders(70ef9b04-d843-df11-a1e1-0018f3790817)'
```


