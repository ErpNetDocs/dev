# **$filter** query option

Use `$filter` to return only the records in a Domain API collection that match a condition. For example, this request selects released sales orders.

```text
Crm_Sales_SalesOrders?$filter=State eq 'Released'
```

The examples use paths relative to the Domain API service root. The Domain API supports a subset of OData operators and adds functions such as `equalnull` and `like`. See the [OData $filter tutorial](https://www.odata.org/getting-started/basic-tutorial/#filter) for the general syntax.

## Supported operators

| Operator | Meaning |
|----------|---------|
| `eq` | Matches a field equal to the given value. |
| `ge` | Matches a field at or above the given value. |
| `le` | Matches a field at or below the given value. |
| `and` | Requires both conditions to match. |
| `in` | Matches a field equal to any value in a list. |

Filter support depends on the field. Check the entity's [Domain Model documentation](https://docs.erp.net/model/entities/) for its supported filters. For example, `[Filter(multi eq;like)]` indicates that the field supports equality against multiple values and pattern matching.

Use `ge` and `le` for range filters. The Domain API can also include values at the boundary when you use `gt` or `lt`.

The Domain API does not support `ne` (not equal) or `not` (negation).

### Combining filter conditions

The Domain API does not support the `or` operator in `$filter`.

#### Match both conditions with `and`

Put both conditions in one `$filter` and join them with `and`. To find sales orders dated in January 2020, require `DocumentDate` to be on or after January 1 and on or before January 31.

```text
Crm_Sales_SalesOrders?$filter=DocumentDate ge 2020-01-01T00:00:00Z and DocumentDate le 2020-01-31T00:00:00Z
```

#### Match one of several values with `in`

Use `in (...)` when the choices are values of one field that supports multiple values. This request returns sales orders in either the `FirmPlanned` or `Released` state.

```text
Crm_Sales_SalesOrders?$filter=State in ('FirmPlanned', 'Released')
```

See [Operator 'in'](#operator-in) for examples using IDs and references.

#### Match either condition on different fields

To find orders that are released or have no store set, query each condition separately.

```text
Crm_Sales_SalesOrders?$filter=State eq 'Released'
Crm_Sales_SalesOrders?$filter=Store eq null
```

Combine the results in your application. If an order appears in both, keep one copy using its `Id`.

### Operator 'eq'

Use `Field eq Value` to match a single value. How you write the value depends on the field type.

Write GUIDs and numbers without quotes.

```text
General_Products_Products?$filter=Id eq 0e8fb111-5b04-4eab-a890-47cfb9cfa4c4
Crm_Sales_SalesOrderLines?$filter=LineNo eq 10
```

Use single quotes around strings and enum member names.

```text
General_Products_Products?$filter=PartNumber eq '1234'
Crm_Sales_SalesOrders?$filter=State eq 'Released'
```

For a navigation property, write the reference as a quoted `EntitySet(Id)` value.

```text
General_Products_Products?$filter=ProductGroup eq 'General_Products_ProductGroups(0bf6b45c-7ec1-484d-9f84-00072b77fabb)'
```

#### Match an unset field

In a filter, `null` represents a field with no value. Use `Field eq null` to match records where that field is unset. For example, this request returns sales orders that have no store set.

```text
Crm_Sales_SalesOrders?$filter=Store eq null
```

Write the literal `null` without quotes. In a filter on a text field, `'null'` represents the text `null`, and `''` represents an empty string.

To include orders for a specific store as well as orders with no store, see [`equalnull(field, value)`](#equalnullfield-value).

### Operator 'in'

Use `in` to match any of several values on a field that supports multiple values. Put the values in parentheses, separated by commas. Write each value the same way you would with `eq`.

Use `in` to express alternatives explicitly. Depending on the field, repeated `eq` conditions on the same field can be treated as alternatives or rejected.

Match products with either ID.

```text
General_Products_Products?$filter=Id in (0e8fb111-5b04-4eab-a890-47cfb9cfa4c4, 14389ba0-ee5c-459e-afd0-d74c17240f28)
```

Match sales orders in either state.

```text
Crm_Sales_SalesOrders?$top=10&$filter=State in ('FirmPlanned', 'Released')&$select=State
```

Match sales orders with either document type.

```text
Crm_Sales_SalesOrders?$top=10&$select=DocumentType&$filter=DocumentType in ('Systems_Documents_DocumentTypes(f8a93d3a-8cf3-4a09-9d45-667d664cb98d)', 'Systems_Documents_DocumentTypes(469b67b1-8b4b-4fb4-9d97-20c96105a85a)')
```

Match sales orders addressed to a particular person or company. `ToParty` can reference either entity type.

```text
Crm_Sales_SalesOrders?$top=10&$filter=ToParty in ('General_Contacts_Persons(adb66f3f-e173-4a37-878c-000920f44ff0)', 'General_Contacts_Companies(39148781-d316-4d4d-a392-0002f73710f2)')
```

## Filter by date and date-time attributes

Use an OData date-time literal such as `2020-01-01T00:00:00Z`. Filters on date-only fields such as `DocumentDate` compare calendar dates. Filters on date-time fields can also compare the time component.

## Filter by complex attributes

The Domain API represents quantities and amounts as complex values that include a measurement unit or currency. Append `Value` to the field name to filter by its numeric value.

```text
Logistics_Inventory_StoreTransactionLines?$filter=QuantityValue ge 5.555
Crm_Sales_SalesOrderLines?$filter=LineAmountValue ge 5.555
```

Custom property filters use `eq` with the property's short value. See [Stored attributes](../../common-tasks/stored-attributes.md#filter-by-a-property) for how to read the value and filter by it.

```text
General_Products_Products?$top=10&$select=CustomProperty_color&$filter=CustomProperty_color eq 'blue'
```

Use `contains` or `like` to search a filterable `MultilanguageString` field. These fields do not support equality against a string.

```text
General_Products_Products?$filter=contains(Name,'ppl')
```

## Supported standard functions

Use these functions on text fields that support text filtering.

| Function | Matches when |
|----------|--------------|
| `contains(field, text)` | The text occurs anywhere in the field. |
| `startswith(field, text)` | The field begins with the text. |
| `endswith(field, text)` | The field ends with the text. |

```text
General_Products_Products?$filter=startswith(PartNumber,'ABC')
```

## Supported non-standard $filter functions

### Edm.Boolean contains(Erp.MultilanguageString, Edm.String)

Returns true if the second string is contained in any language of the first multi-language string.

```text
Crm_Sales_Customers?$filter=contains(Party/PartyName,'Peter')
```

### Edm.Boolean contains(NavigationProperty, Edm.String)

Searches the entities referenced by the navigation property using the referenced entity's search behavior.

```text
Crm_Sales_Customers?$filter=contains(SalesPerson,'Peter')
```

#### Context values in `contains`

ERP.net recognizes these special search strings for supported fields.

| Search string | Matches |
|---------------|---------|
| `'(empty)'` | A field with no value set. |
| `'(me)'` | The current user in a user reference. |
| `'(current)'` | The current enterprise company, company location, or user role, depending on the reference type. |
| `'(me) or (empty)'` | The current user or a null user reference. |
| `'(current) or (empty)'` | The current value or a null reference for the supported context types. |

These are quoted search strings recognized by `contains`. This example matches orders assigned to the current user or unassigned orders.

```text
Crm_Sales_SalesOrders?$filter=contains(AssignedToUser,'(me) or (empty)')
```

### `equalnull(field, value)`

Use `equalnull(field, value)` to return records where the field equals the given value or is unset. The field must support equality filtering and allow nulls.

This request returns orders for the specified store as well as orders with no store set. `Store` is a navigation property, so the second argument is a quoted `EntitySet(Id)` reference.

```text
Crm_Sales_SalesOrders?$filter=equalnull(Store,'Logistics_Inventory_Stores(8d7dd360-17cc-47f4-a878-1ee0f06445ad)')
```

To return only orders with no store set, use [`Store eq null`](#match-an-unset-field).

### `lessequalnull(field, value)`

Use `lessequalnull(field, value)` to return records where the field is less than or equal to the given value or is unset. Use a field that supports range comparisons, such as a date. This example finds salespersons whose contract start date is on or before February 1, 2019, or is unset.

```text
Crm_SalesForce_SalesPersons?$filter=lessequalnull(ContractStartDate,2019-02-01T00:00:00Z)
```

### `greaterequalnull(field, value)`

Use `greaterequalnull(field, value)` to return records where the field is greater than or equal to the given value or is unset. This example finds salespersons whose contract end date is on or after February 1, 2019, or is unset.

```text
Crm_SalesForce_SalesPersons?$filter=greaterequalnull(ContractEndDate,2019-02-01T00:00:00Z)
```

Join the two functions with `and` to apply both bounds. Each function includes nulls for its own field.

```text
Crm_SalesForce_SalesPersons?$filter=lessequalnull(ContractStartDate,2019-02-01T00:00:00Z) and greaterequalnull(ContractEndDate,2019-02-01T00:00:00Z)
```

### `like(field, pattern)`

Use `like` for pattern matching on text fields that support it. In the pattern, `%` matches zero or more characters and `_` matches one character. In a request URL, encode a literal `%` in the pattern as `%25`. This example matches part numbers that start with `ABC`.

```text
General_Products_Products?$filter=like(PartNumber,'ABC%25')
```
