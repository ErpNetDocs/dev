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
| **find**             | Searches for a matching object and uses the first found. If no matching object is found, an error is thrown. <br> **If the JSON object contains data properties, they are ignored — the found object remains unchanged.**                       |
| **findOrNull**       | Searches for a matching object and uses the first found. If none is found, returns `null`. <br> **If the JSON object contains data properties, they are ignored — the found object remains unchanged.**                                         |
| **findOrCreate**     | Searches for a matching object and uses the first found. If none is found, a new object is created and populated with the provided data properties.                                                                                             |
| **findSingle**       | Searches for a matching object and uses the first found. If no matching object is found **or more than one matching object is found**, an error is thrown.                                                                                      |
| **findSingleOrNull** | Searches for a matching object and uses the first found. If no matching object is found **or more than one matching object is found**, returns `null`.                                                                                          |
| **merge**            | Searches for a matching object and uses the first found. If none is found, a new object is created and populated with the provided data properties. <br> **If an existing object is found, it is updated with the provided data properties.**   |


## Default values
If the @erpnet.action annotation is not present in the object, the following defaults are applied:

**For top-level objects**:

POST → @erpnet.action: create

PATCH → @erpnet.action: update

**For nested objects**:

If only properties defining the search criteria are provided (either @erpnet.findBy or data properties usable in a find action),
→ @erpnet.action: find

Otherwise
→ @erpnet.action: merge


# @erpnet.findBy annotation

The @erpnet.findBy annotation explicitly defines the search criteria when an existing object should be found.
It is applicable to the following @erpnet.action values:
- find
- findOrNull
- findOrCreate
- findSingle
- findSingleOrNull
- merge

The value of this annotation is an object with one of the following (string) properties:
```
{
	ExternalId,
	ExternalSystem,
	Id,
	Code,
	Name,
	DisplayText
}
```

- **ExternalId** is provided when we want to find existing object with particular ExternalId.
- **ExternalSystem** may be provided alongside ExternalId.
- **Id** is provided if we want to find object by it's Id. This is a Guid.
- **Code** is provided if we want to find object by it's Code. This is applicable only for entities that provde CodeDataMember. The CodeDataMember of the entity can be found in the entity [documentation](https://docs.erp.net/model/entities/General.Products.Products.html#default-visualization).
- **Name** is provided if we want to find object by it's Name. This is applicable only for entities that provde NameDataMember. The NameDataMember of the entity can be found in the entity [documentation](https://docs.erp.net/model/entities/General.Products.Products.html#default-visualization). Search by Name is performed with `contains` operation.
- **DisplayText** - performs search by entity's display text - also `contains`. This search is equivalent to the $search odata url parameter. 

