# Working with documents

See [Documents](xref:Documents) on technical documentation.

Retrieving and updating documents is the same as any other entity in the domain.

However there are some specific rules that apply only to documents. For example documents on state Released or above can not be modified directly. They must be modified with Adjustment Documents. [Adjustment Documents](xref:Adjustment-Documents)

Another important attribute of the documents that can not be modified with simple PATCH request is [State](xref:Document-States)

The examples below show some tasks related to documents.

## Create Document

Document can be created only by specifying the required properties. Other properties will be filled by it's constant default value or it's LateDefault expression.

If Front-End model is used in [API Transaction](transactions.md) dependent property values are recalculated upon property change.  For example in SalesOrderLine line.ProductDescription is set to line.Product.Name when line.Product changes.

In the example bellow a new SalesOrder is created with one SalesOrderLine.

Note that measurement units and currencies are specified before passing [Quantity](quantity.md) or [Amount](amount.md) values. This is required because the quantity or amount contains the code of the measurement unit or currency.

POST ~/Crm_Sales_SalesOrders

```json
{
"DocumentType": {
"@odata.id": "General_DocumentTypes(469b67b1-8b4b-4fb4-9d97-20c96105a85a)"
},
"EnterpriseCompany": {
"@odata.id": "General_EnterpriseCompanies(b0e80577-fbbe-4c9b-811e-20b6c6dd465f)"
},
"Customer": {
"@odata.id": "Crm_Customers(15f2640f-f374-4017-ae2d-d2a41535f054)"
},
"DocumentCurrency": {
"@odata.id": "General_Currencies(3187833a-d3c1-4804-bfc0-e17e6aee3069)"
},
"Lines": [
{
"Product": {
"@odata.id": "General_Products_Products(81d38b50-fd06-e611-8292-b31071e2ee7f)"
},
"QuantityUnit": {
"@odata.id": "General_MeasurementUnits(7dbe6d6a-22ef-4c2f-a798-054bc2d13c8b)"
},
"Quantity": {
"Value": 1,
"Unit": "pcs"
},
"UnitPrice": {
"Value": 20,
"Currency": "BGN"
}
}
]
}
```

## Change Document State

