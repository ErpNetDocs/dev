# `@erpnet.action` instance annotation

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
        "@erpnet.findBy": {"Name": "Pieces" }
    },
    "MeasurementUnit": {
        "Code": "pcs"
    },
    "Name": {"EN": "Domain API Test 002"},
    "ProductGroup": {
        "Code": "DATG01",
        "Name": {"EN": "Domain API Tests"}
	}
}
```
The purpose of this annotation is to provide a better way to update, create, merge or delete data.
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

> **Note:**  
> You can try all examples directly in the [ERP.net Query Tool](https://testdb.my.erp.net/api/domain/query?GET+General_Products_Products?$top=10&$expand=ProductGroup&$orderby=PartNumber).  
> In the tool, you can choose the HTTP method and execute not only GET requests, but also POST, PATCH, and DELETE.  
> This allows you to test queries, create, update, or delete data directly against the public test database.


### Product Import Examples

Below are several examples of importing products.

In this first example, we create a product while providing the measurement unit and category by their IDs.  
The **ProductGroup** uses **implicit merge**, meaning it searches by code.

**Note:** If a product group is found, its code and name will be updated with the provided values.  
If there are no changes, the object remains **Unchanged** and is not written to the database.  
However, if the group’s name was manually edited after the initial import (for example, a the group name is translated to some other language), this change will be lost due to the **MERGE**, which will set the entire multi-language value for Name attribute.
```json
POST General_Products_Products
{
  "PartNumber": "DAT001",
  "Name": {"EN": "Domain API Test 001"},
  "BaseMeasurementCategory@odata.bind": "General_Products_MeasurementCategories(045d1e60-2114-4ca7-b636-0666dd0d2ec8)",
  "MeasurementUnit@odata.bind": "General_Products_MeasurementUnits(7dbe6d6a-22ef-4c2f-a798-054bc2d13c8b)",
  "ProductGroup": {
    "Code": "DATG01",
    "Name": {"EN": "Domain API Tests"}
  }
}
```


In this example, we explicitly set `@erpnet.action: merge`.  
This means the system will first look for a product by its code before creating a new one.
```json
POST General_Products_Products
{
  "@erpnet.action": "merge",
  "PartNumber": "DAT002",
  "Name": {"EN": "Domain API Test 002"},
  "BaseMeasurementCategory@odata.bind": "General_Products_MeasurementCategories(045d1e60-2114-4ca7-b636-0666dd0d2ec8)",
  "MeasurementUnit@odata.bind": "General_Products_MeasurementUnits(7dbe6d6a-22ef-4c2f-a798-054bc2d13c8b)",
  "ProductGroup": {
    "Code": "DATG01",
    "Name": {"EN": "Domain API Tests"}
  }
}
```


Same as above, but without using IDs for the measurement unit and category — only the name and code are used.
The BaseMeasurementCategory is searched by Name - this is `General_MeasurementCategories?$filter=contains(Name,"Pieces")`.
```json
POST General_Products_Products
{
  "@erpnet.action": "merge",
  "PartNumber": "DAT003",
  "BaseMeasurementCategory": {
    "@erpnet.action": "find",
    "@erpnet.findBy": {"Name": "Pieces"}
  },
  "MeasurementUnit": {
    "@erpnet.action": "find",
    "Code": "pcs"
  },
  "Name": {"EN": "Domain API Test 003"},
  "ProductGroup": {
    "Code": "DATG01",
    "Name": {"EN": "Domain API Tests"}
  }
}
```


Same as the previous example, but the measurement unit omits the `@erpnet.action` annotation. The action again is **find** because we only provide the code attribute.
```json
POST General_Products_Products
{
  "@erpnet.action": "merge",
  "PartNumber": "DAT003",
  "BaseMeasurementCategory": {
    "@erpnet.action": "find",
    "@erpnet.findBy": {"Name": "Pieces"}
  },
  "MeasurementUnit": {
    "Code": "pcs"
  },
  "Name": {"EN": "Domain API Test 003"},
  "ProductGroup": {
    "Code": "DATG01",
    "Name": {"EN": "Domain API Tests"}
  }
}
```

This example searches by **ExternalId**.
```json
POST General_Products_Products
{
  "@erpnet.action": "merge",
  "ExternalId": "EXT004",
  "PartNumber": "DAT004",
  "BaseMeasurementCategory": {
    "@erpnet.action": "find",
    "@erpnet.findBy": {"Name": "Pieces"}
  },
  "MeasurementUnit": {
    "Code": "pcs"
  },
  "Name": {"EN": "Domain API Test 004"},
  "ProductGroup": {
    "Code": "DATG01",
    "Name": {"EN": "Domain API Tests"}
  }
}
```

---


In the following example we are editing a sales order and want to MERGE the customer.
The `merge` action first attempts to find an existing object by the provided attributes.
The provided customer data is interpretted as  `"@erpnet.action": "merge", "@erpnet.findBy": { "Code": "CS0099" }`
First, the system will search for a customer with Number = "CS0099". 
If none is found, it will create a new customer and fill it's Number and Party.
The Party will also attempt a merge. 
The provided data can't be transformed to `@erpnet.findBy` because we provide Company.Name, which is not a NameDataMember for the PartiesRepository, so no lookup by name will be performed. The NameDataMember for parties is "PartyName".
If there is no __findBy__ arguments a new Party will be created.
**Erp.General_Contacts_Party** is abstract class so we can't create a party directly. We should specify the exact inheritor type. This is done with
`"@odata.type": "Erp.General_Contacts_Company"`. Now the system knows what object to create.
As a result, a new company will be created.
It is safe to pass the attributes defined in "Erp.General_Contacts_Company" type, so the system can set their values in the created company.


```
PATCH Crm_Sales_SalesOrders(fd8e5bd8-5fa4-4eae-a763-aad226b9101d)
{
   "Customer": {
     "Number": "CS0099",
     "Party": {
       "@odata.type": "Erp.General_Contacts_Company",
       "Name": { "EN": "New Company"},
       "RegistrationType": {"EN": "Ltd" }
     }
   }
}
```

---

In the following example, we create a sales order **without using any IDs**.  
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
        "Code": "pcs"
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

---
