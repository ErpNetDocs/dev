## $options extension query option

> [!note]
> $options is Domain API specific option and is not part of the OData standard.

$options contains option flags, which are provided as url arguments.

Flag | Description
-----|------------
skipnulls | Indicates that properties with null value are not returned in the JSON result.

Example:
```odata
options=skipnulls
```
