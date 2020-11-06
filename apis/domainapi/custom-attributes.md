# Custom Attributes

User-defined attributes, which can supplement the system properties of almost all entities in the system.

Custom properties are defined in General_CustomProperties entity.

EntityName specifies the entity for which the property is applicable. In model documentation there is a value for EntityName (specified as Entity: {EntityName})

Custom properties can be free text property or restricted to a list of allowed values. This is specified by LimitToAllowedValues property.

AllowedValuesEntityName specifies that the allowed values are retrieved from the specified entity. If null the allowed values are retrieved from General_CustomPropertyAllowedValues.

In Domain API custom properties are properties of type General_CustomPropertyValue. The API name of the custom property starts with 'CustomProperty_' followed by the user defined property code.

Each database contains different custom properties and that is why each database have different EDM model ($metadata).

If a user creates new custom property in the database this custom property is not automatically shown in Domain API. That is because Domain API caches all repositories and their attributes. To refresh the cached attributes you must call the ~/domain/reset endpoint.  

Example:
https://demodb.my.erp.net/api/domain/reset


Properties of CustomPropertyValue complex type.

| Name | Type | Description |
| ---- | ---- | --- |
| Value	| String | The short value.| 
| Description	| MultilanguageString	| The description of the property value.Can be null. | 
| ValueId	| Guid	| The Id of the entry represented by the property value. It's the id of the allowed value. Can be null. | 

Example:
```
"CustomProperty_color": {
    "Value": "Син",
    "ValueId": "5263a2d3-88b0-41db-adae-31c76135719e",
    "Description": {
        "BG": "Морско"
    }
}
```
>Note!  
>To filter by Custom Property you must use only the short value (only eq is supported):  
>General_Products_Products?$top=10&$select=CustomProperty_color&$filter=CustomProperty_color eq 'син'
