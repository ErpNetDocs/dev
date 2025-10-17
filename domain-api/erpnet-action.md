# @erpnet.action instance annotation

In OData instance annotations can be used to define additional information associated with a particular result, entity, property, or error.
The `@erpnet.action` annotation can be provided in the body of update request (POST, PATCH) or in the [Import action](import.md) 

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

The goal of this annotation is to provide better way to import (update or create) data.
The value of the `@erpnet.action` annotation determines the type of operation that is to be performed with the provided JSON data.

## Allowed values
* **create** - Always creates new object.

* **update** - Updates existing object. This is the default for top-level JSON object in PATCH request. If `"@odata.action": "update"` is explicitly specified in nested/referenced object, the properties of the refered objects are modified.

* **find** - Searches matching object and uses the first found. If no matching object is found an error is thrown. **If the JSON object contains data properties they are ignored - the found object remains unchanged**.

* **findOrNull** - Searches matching object and uses the first found. If no matching object is found returns `null`. **If the JSON object contains data properties they are ignored - the found object remains unchanged**.

* **findOrCreate** - Searches matching object and uses the first found. If no matching object is found a new object is created and populated with the provided data properties.

* **findSingle** - Searches matching object and uses the first found. If no matching object is found **or there is more than one matching object an error is thrown**.

* **findSingleOrNull** - Searches matching object and uses the first found. If no matching object is found **or there is more than one matching object returns null**.

* **merge** - Searches matching object and uses the first found. If no matching object is found a new object is created and populated with the provided data properties. **If an existing object is found it is updated with the provided data properties**.


## Default values
If `@erpnet.action` annotation is not present in the object the following default value is applied:

**For Top-Level object:**
- POST = `@erpnet.action: create`
- PATCH = `@erpnet.action: update`

**For nested object:**
- if the provided properties (@erpnet.findBy or data properties that are usable in find action) are only properties that define the find criteria 'find'
 `@erpnet.action: find`
- otherwise
  `@erpnet.action: merge`
