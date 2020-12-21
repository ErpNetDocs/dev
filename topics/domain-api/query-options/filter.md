# $filter query option

$filter is a standard OData query option, implemented with some limitations in @@name.
Not every operation and function provided by the OData standard are implemented.

For a great introduction to $filter, read the [OData $filter tutorial](https://www.odata.org/getting-started/basic-tutorial/#filter).

This article mostly emphasizes on the implementation details of **$filter** in @@name.

## Supported operators

Operator | Description
---------|------------
**eq** | Equal
**ge** | Greater than or equal
**le** | Less than or equal
**and** | Returns true if both the left and right operands evaluate to true.
**in** | The in operator returns true if the left operand is a member of the right operand. The right operand MUST be a comma-separated list of primitive value.

### Operator 'or' is not supported

Operator 'or' is not supported.

However all navigation properties and some properties of enumerable type (e.g. General_Document.State) support the 'in' operator.

### Operator 'in'

The 'in' operator can be used for minimizing the query round trips.

Examples:

* List of Id values:

```odata
General_Products_Products?$filter=Id in (0e8fb111-5b04-4eab-a890-47cfb9cfa4c4, 14389ba0-ee5c-459e-afd0-d74c17240f28)
```

* List of enum values:

```odata
Crm_Sales_SalesOrders?$top=10&$filter=State in ('FirmPlanned', 'Released')&$select=State
```

* List of reference values:

```odata
Crm_Sales_SalesOrders?$top=10&$select=DocumentType&$filter=DocumentType in ('General_DocumentTypes(f8a93d3a-8cf3-4a09-9d45-667d664cb98d)', 'General_DocumentTypes(469b67b1-8b4b-4fb4-9d97-20c96105a85a)')
```

* List of reference values with different object types (the reference is of the base object type):

```odata
Crm_Sales_SalesOrders?$top=10&$filter=ToParty in ('General_Contacts_Persons(adb66f3f-e173-4a37-878c-000920f44ff0)', 'General_Contacts_Companies(39148781-d316-4d4d-a392-0002f73710f2)')
```

## Supported standard functions

* Edm.Boolean contains(Edm.String, Edm.String)
* Edm.Boolean endswith(Edm.String, Edm.String)
* Edm.Boolean startswith(Edm.String, Edm.String)

## Supported non-standard $filter functions

### Edm.Boolean contains(Erp.MultilanguageString, Edm.String) 

Returns true if the second string is contained in any language of the first multi-language string.

Example: 
```odata
~/Crm_Customers?$filter=contains(Party/PartyName,'Peter')
```

### Edm.Boolean equalnull(any-type,any-type) 

Returns true if the first argument is equal to the second argument or the first argument is null.

Example: 
```odata
~/Crm_Sales_SalesOrders?&$filter=equalnull(Store,'Logistics_Inventory_Stores(8d7dd360-17cc-47f4-a878-1ee0f06445ad)')
```

### Edm.Boolean lessequalnull(any-type,any-type) 

Returns true if the first argument is less than or equal to the second argument or the first argument is null.

Example: 

```odata
~/Crm_SalesPersons?$top=10&$filter=lessequalnull(ContractEndDate,2019-02-01T00:00:00.000Z)
```

### Edm.Boolean greaterequalnull(any-type,any-type)

Returns true if the first argument is greater than or equal to the second argument or the first argument is null.

Example: 

```odata
~/Crm_SalesPersons?$top=10&$filter=greaterequalnull(ContractStartDate,2019-02-01T00:00:00.000Z)
```
