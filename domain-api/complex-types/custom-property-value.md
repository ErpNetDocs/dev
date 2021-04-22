# Custom Property Value

Custom Properties (also called Custom Attributes in the domain terminology) are user-defined attributes, which can supplement the predefined system attributes.

## Definition of а Custom Property 

For reference information about the definition of the custom properties, see @General.CustomProperties.

Here are some highlights for the definition record:

- *EntityName* contains the name of the entity, for which the property is defined.
You can find the entity name for each entity in the model documentation.
For example, the entity name for @Crm.Customers is "Crm_Customers" (check it out in the link - see the tag line under the entity name).

- *LimitToAllowedValues* - this defines whether the property is free text or is limited to a list of allowed values.

- *AllowedValuesEntityName* -  specifies that the allowed values are retrieved from the specified entity.
When this is NULL, the allowed values are retrieved from @General.CustomPropertyAllowedValues .

## Data type and values

In the Domain API, the custom properties are properties of type General_CustomPropertyValue.
The API name of the custom property starts with 'CustomProperty_' followed by the user defined property code.

> [!note]
> Properties with Code, which does not conform to the specification for identifier name, might not be accessible through the API.
> See [Identifier Name Specification](https://docs.microsoft.com/en-us/dotnet/csharp/programming-guide/inside-a-program/identifier-names).

Each database contains different custom properties and that is why each database have different EDM model ($metadata).

> [!note]
> If a user creates new custom property in the database this custom property is not automatically shown in Domain API.
> This is because the Domain API caches all repositories and their attributes.
> To refresh the cached attributes you must call the ~/domain/reset endpoint.  
>
> Example:
> <https://demodb.my.erp.net/api/domain/reset>

## Composition of the CustomPropertyValue type

| Name | Type | Description |
| ---- | ---- | --- |
| Value	| String | The short value. This is the actual value of the custom property. | 
| Description	| MultilanguageString	| This is long, descriptive, multi-language value of the custom property. Can be null. | 
| ValueId	| Guid	| The Id of the entry represented by the property value. It's the id of the allowed value. Can be null. | 

## Example

```
"CustomProperty_color": {
    "Value": "аpple",
    "ValueId": "5263a2d3-88b0-41db-adae-31c76135719e",
    "Description": {
        "EN": "The Apple.",
        "DE": "Die Apfel."
    }
}
```
> [!note]  
> To filter by Custom Property you must use only the short value (only eq is supported):  
> General_Products_Products?$top=10&$select=CustomProperty_color&$filter=CustomProperty_color eq 'apple'
