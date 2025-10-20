# @erpnet.action instance annotation

In OData instance annotations can be used to define additional information associated with a particular result, entity, property, or error.
The `@erpnet.action` annotation can be provided in the body of update request (POST, PATCH) or in the [Import action](import.md).

Example usage

```
POST General_Products_Products
{
   "@erpnet.action": "merge",
    "PartNumber": "DAT003",
    "BaseMeasurementCategory": {
        "@erpnet.action": "find",
        "@erpnet.findBy": {"Name": "Unit" }
    },
    "MeasurementUnit": {
        "Code": "PCE"
    },
    "Name": {"EN": "Domain API Test 002"},
    "ProductGroup": {
        "Code": "DATG01",
        "Name": {"EN": "Domain API Tests"}
	}
}
```
The purpose of this annotation is to provide a better way to import, update, or create data.
The value of the @erpnet.action annotation determines the type of operation that will be performed using the provided JSON data.

## Allowed values

| Action               | Description                                                                                                                                                                                                                                     |
| -------------------- | ----------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------------- |
| **create**           | Always creates a new object.                                                                                                                                                                                                                    |
| **update**           | Updates an existing object. This is the default for the top-level JSON object in a `PATCH` request. If `@erpnet.action: update` is explicitly specified for a nested (referenced) object, the properties of the referenced object are modified. |
| **delete**           | Deletes an existing object. Can be used in [Import](import.md).           
| **find**             | Searches for a matching object and uses the first found. If no matching object is found, an error is thrown. <br> **If the JSON object contains data properties, they are ignored — the found object remains unchanged.**                       |
| **findOrNull**       | Searches for a matching object and uses the first found. If none is found, returns `null`. <br> **If the JSON object contains data properties, they are ignored — the found object remains unchanged.**                                         |
| **findOrCreate**     | Searches for a matching object and uses the first found. If none is found, a new object is created and populated with the provided data properties.                                                                                             |
| **findSingle**       | Searches for a matching object and uses the first found. If no matching object is found **or more than one matching object is found**, an error is thrown.                                                                                      |
| **findSingleOrNull** | Searches for a matching object and uses the first found. If no matching object is found **or more than one matching object is found**, returns `null`.                                                                                          |
| **merge**            | Searches for a matching object and uses the first found. If none is found, a new object is created and populated with the provided data properties. <br> **If an existing object is found, it is updated with the provided data properties.**   |


## Default values
If the @erpnet.action annotation is not present in the object, the following defaults are applied:

**For top-level objects**:

POST → @erpnet.action: **create**

PATCH → @erpnet.action: **update**

**For nested objects**:

If only properties defining the search criteria are provided (either @erpnet.findBy or data properties usable in a find action),
→ @erpnet.action: **find**

Otherwise
→ @erpnet.action: **merge**


# `@erpnet.findBy` Annotation

The `@erpnet.findBy` annotation explicitly defines the search criteria used when an existing object should be located.  
It is applicable to the following `@erpnet.action` values:

- `find`
- `findOrNull`
- `findOrCreate`
- `findSingle`
- `findSingleOrNull`
- `merge`

---

## Structure

The value of the annotation is an object with one or more of the following string properties:

```json
{
  "ExternalId": "...",
  "ExternalSystem": "...",
  "Id": "...",
  "Code": "...",
  "Name": "...",
  "DisplayText": "..."
}
```

## Property Details

**ExternalId** — Used to find an existing object by its specific external identifier.

**ExternalSystem** — Optional. Can be provided alongside ExternalId to further qualify the search.

**Id** — Used to find an object by its unique identifier (Guid).

**Code** — Used to find an object by its code.
This applies only to entities that provide a **CodeDataMember**.
The CodeDataMember for a specific entity can be found in its documentation.

**Name** — Used to find an object by its name.
This applies only to entities that provide a **NameDataMember**.
The NameDataMember for a specific entity can be found in its documentation.
The search operation performs a case-insensitive contains match.

**DisplayText** — Searches by the entity’s display text using a contains operation.
This is equivalent to using the $search parameter in OData queries.


## Find Criteria Evaluation

The find operation uses only the first available criterion from the @erpnet.findBy annotation in the following priority order:

1. `ExternalId` (+ optional `ExternalSystem`)
2. `Id`
3. `Code`
4. `Name`
5. `DisplayText`

If multiple properties are specified, only the first one (in the order they appear) is used.
For example:

```
"@erpnet.findBy": {
  "ExternalId": "123",
  "ExternalSystem": "SomeSystem",
  "Code": "345"
}
```
In this case, the search will be performed only by ExternalId and ExternalSystem, while Code will be ignored.


## Default behavior

If the @erpnet.findBy annotation is omitted, the search criteria are automatically derived from the provided object’s properties.

For example 
```
{
"Customer": {
    "Number": "Г89163"
  }
}
```
is equivalent to 
```
{
"Customer": {
    "@erpnet.action": "find",
    "@erpnet.findBy": {"Code": "Г89163"}
  }
}
```
because "Number" is the __CodeDataMember__ for customers we perform search by Code.
This is a nested object and only CodeDataMember property is provided so the `@erpnet.action` is determined as `find`. 


# Examples

In this example, we create a sales order **without using any IDs**.  
The system automatically determines the `@erpnet.action` and `@erpnet.findBy` criteria based on the provided properties.

```http
POST Crm_Sales_SalesOrders
{
  "DocumentType": { // defaults to: @erpnet.action = find, @erpnet.findBy = Code
    "Code": "CRM_SALES_ORDER"
  },
  "EnterpriseCompany": { // defaults to: @erpnet.action = find
    "@erpnet.findBy": { "Code": "546346373" }
  },
  "EnterpriseCompanyLocation": { // defaults to: @erpnet.action = find, @erpnet.findBy = Code
    "PartyCode": "00193"
  },
  "Customer": { // defaults to: @erpnet.action = find, @erpnet.findBy = Code
    "Number": "Г89163"
  },
  "DocumentCurrency": { // defaults to: @erpnet.action = find, @erpnet.findBy = Code
    "CurrencySign": "BGN"
  },
  "Lines": [
    {
      "Product": { // defaults to: @erpnet.action = find, @erpnet.findBy = Code
        "PartNumber": "DAT001"
      },
      "QuantityUnit": { // defaults to: @erpnet.action = find, @erpnet.findBy = Code
        "Code": "PCE"
      },
      "Quantity": {
        "Value": 1,
        "Unit": "PCE"
      },
      "UnitPrice": {
        "Value": 20,
        "Currency": "BGN"
      }
    }
  ]
}
```
