## **options** query option

> [!note]
> **options** is Domain API specific option and is not part of the OData standard.
> Therefore, this option DOES NOT have "$" in front of its name.

**options** contains option flags, which are provided as url arguments.

Flag | Description
-----|------------
skipnulls | Indicates that properties with null value are not returned in the JSON result.

Example:
```odata
options=skipnulls
```
