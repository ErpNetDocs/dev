# Common Tasks

This section includes examples of common tasks performed with the Domain API.

[!list folder="."]

## CONTAINS

Using _contains_ filter function for string attribute:

```
General_Contacts_Parties?$top=5&$filter=contains(PartyCode,'30')
``` 
[Try it Yourself](https://testdb.my.erp.net/api/domain/query?GET+General_Contacts_Parties?$top=5&$filter=contains(PartyCode,%2730%27))


Using _contains_ filter function for multi-language attribute:

```
General_Contacts_Parties?$top=5&$filter=contains(PartyName,'Ivan')
``` 
[Try it Yourself](https://testdb.my.erp.net/api/domain/query?GET+General_Contacts_Parties?$top=5&$filter=contains(PartyName,%27Ivan%27))

## IN OPERATOR

Not every attribute in @@name supports **in** filter operator. Generally all navigation properties support _in_, some _enum_ attributes and occasionaly some other attributes. This information you can find in the [Domain Model Reference](https://docs.erp.net/model/entities/).

Using _in_ for Id attribute:
```
General_Products_Products?$top=10&$filter=Id in (edf2bd2a-7e4d-e111-a06c-00155d00050a, cf728601-1fd5-4853-ab23-01deeee7d038)
```
[Try it Yourself](https://testdb.my.erp.net/api/domain/query?GET+General_Products_Products?$top=10&$filter=Id+in+(edf2bd2a-7e4d-e111-a06c-00155d00050a,+cf728601-1fd5-4853-ab23-01deeee7d038))

Using _in_ for Document State attribute:
```
Crm_Sales_SalesOrders?$top=10&$filter=State in ('FirmPlanned', 'Released')&$select=State
```
[Try it Yourself](https://testdb.my.erp.net/api/domain/query?GET+Crm_Sales_SalesOrders?$top=10&$filter=State+in+(%27FirmPlanned%27,+%27Released%27)&$select=State)


Using _in_ for DocumentType navigation property:
```
Crm_Sales_SalesOrders?$top=10&$select=DocumentType&$filter=DocumentType in ('General_DocumentTypes(de4913f3-962a-4289-a0f3-01bc2c1da21d)', 'General_DocumentTypes(a8b99412-3348-4c12-abdf-1a6a15ab5449)')
```
[Try it Yourself](https://testdb.my.erp.net/api/domain/query?GET+General_DocumentTypes?$filter=EntityName+eq+%27Crm_Sales_Orders%27)


Passing inherited entity uri-s (Document.ToParty is of type General_Contacts_Party. The types General_Contacts_Company and General_Contacts_Person are inherited by General_Contacts_Party):
```
Crm_Sales_SalesOrders?$top=10&$filter=ToParty in ('General_Contacts_Persons(2e97f255-f410-4925-8c51-211c8eaa18b8)', 'General_Contacts_Companies(bc60d0bc-2804-4e3c-b355-04184aef5505)')
```
Note: if you pass uri-s to General_Contacts_Party entity the query can be optimized and will be faster than specifying inherited entity uri-s.
[Try it Yourself](https://testdb.my.erp.net/api/domain/query?GET+Crm_Sales_SalesOrders?$top=10&$filter=ToParty+in+(%27General_Contacts_Persons(2e97f255-f410-4925-8c51-211c8eaa18b8)%27,+%27General_Contacts_Companies(bc60d0bc-2804-4e3c-b355-04184aef5505)%27))


## CAST

The following uri returns the parent document of a specified sales order cast as Crm_Presales_Offer.

Note: The type must be specified with the namespace which for all entities is Erp.

```
Crm_Sales_SalesOrders(11217345-3659-43be-a85d-005eaaa3aaac)/Parent/Erp.Crm_Presales_Offer
```


## SELECT DEFAULT

By default only system attributes are present in the JSON result. The Id attribute, custom properties and calculated attributes are not present. Use the keyword default in $select clause to include all default attributes. To include custom properties and calculated attributes they must be specified explicitly in $select clause.

```
General_Products_Products?$top=1&$select=default,CalculatedAttribute_name
```

## NESTED EXPAND
```
Crm_Sales_SalesOrders(59098bcf-f331-478f-91c2-f5520590f534)?$expand=Lines($expand=Product($select=Codes,Name,PartNumber;$expand=Codes($select=Code)))
```


## EXPAND $ref - returns the items as links

```
Crm_Sales_SalesOrders(59098bcf-f331-478f-91c2-f5520590f534)?&$expand=Lines/$ref
```


## FILTER BY DATE
```
Crm_Sales_SalesOrders?$top=2&$filter=DocumentDate eq 2012-01-01T00:00:00Z
```


## FILTER BY Custom Property
```
General_Products_Products?$top=10&$select=CustomProperty_color&$filter=CustomProperty_color eq 'blue'
```


## FILTER BY Quantity and Amount
```
Crm_Sales_SalesOrderLines?$top=10&$filter=QuantityValue ge 3 and QuantityValue le 5 and LineAmountValue ge 15.45&$select=Quantity
```


## Request COUNT applying filter 

```
Crm_Invoicing_Invoices/$count?$filter=DocumentDate eq 2020-03-23T00:00:00Z
```
This request returns the number of invoices for the specified date. See http://docs.oasis-open.org/odata/odata/v4.01/odata-v4.01-part2-url-conventions.html#_Toc31361043

```
Crm_Sales_SalesOrderLines/$count?$filter=SalesOrder/Void eq false and SalesOrder/State ge 'Released' and Product eq 'General_Products_Products(35d5bcb9-0881-4bc8-bfe4-84fb874d4626)'
```
This request returns the number of released sales order lines for a given product.



## Using $top, $skip and $count 

```
Crm_Sales_SalesOrderLines?$filter=SalesOrder/Void eq false and SalesOrder/State ge 'Released'&$top=10&$skip=120&$count=true
```

The $count=true query option specifies that the total number of rows for the specified filter will be included in the result along with the data.



## UPDATE PRODUCT
```
PATCH ~/General_Products_Products(59098bcf-f331-478f-91c2-f5520590f534)

{
"@odata.type": "#Erp.General_Products_Product",
"ABCClass": "A",
"StandardLotSizeBase": {
"Value": 3.45,
"Unit": "бр"
},
"MeasurementUnit@odata.bind": "https://mycompany.com/api/domain/odata/General_MeasurementUnits(5c5e77ce-60bb-4338-abd0-3a2acb27ff93)"
}
```


## FILTER BY DOCUMENT STATE
```
Crm_Sales_SalesOrders?$top=1&$filter=State ge Erp.General_DocumentState'Released'
```


## FILTER BY MULTIPLE DOCUMENT TYPES
```
Crm_Sales_SalesOrders?$top=2&$filter=DocumentType eq 'General_DocumentTypes(f8a93d3a-8cf3-4a09-9d45-667d664cb98d)' and DocumentType eq 'General_DocumentTypes(f8a93d3a-8cf3-4a09-9d45-667d664cb98e)' and DocumentType eq 'General_DocumentTypes(f8a93d3a-8cf3-4a09-9d45-667d664cb98f)'
```


## FILTER BY MasterDocument with Sales Order URI
```
Crm_Sales_SalesOrders?$top=2&$filter=MasterDocument eq 'Crm_Sales_SalesOrders(70ef9b04-d843-df11-a1e1-0018f3790817)'
```


## CHANGE DOCUMENT STATE
```
POST ~/Crm_Sales_SalesOrders(59098bcf-f331-478f-91c2-f5520590f534)/ChangeState
{
"newState" : "FirmPlanned",
"userStatus": {"@odata.id": "General_DocumentTypeUserStatuses(1ee1249e-4ef5-46b4-8409-26b2130d09c7)"}
}
```


## MAKE DOCUMENT VOID
```
POST ~/Crm_Sales_SalesOrders(11217345-3659-43be-a85d-005eaaa3aaac)/MakeVoid
{
"reason" : "test api method",
"voidType": "VoidDocument"
}
```
