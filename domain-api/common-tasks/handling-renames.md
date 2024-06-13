# Handling entity type renames

Sometimes, we have to rename entity types. In one version, they are called X, and in the next version, they are called Y. Of course, we avoid doing this as much as we can. But sometimes it is unavoidable and has to be done.

For example, we have renamed "General.DocumentPrintImages" ("General_DocumentPrintImages" in the API) to "Systems.Internal.DocumentPrintImages".

This, naturally, is a problem for applications, which access these entities through the API.

To alleviate the problem, we have taken numerous measures:

 

## 1. Error message containing the new name

When you access an entity, which has been renamed, ERP.net Domain API returns an informative error message, which contains the new name of the entity type

For example:

```
The provided entity set name 'General_DocumentPrintImages' is no more supported.
The new name of this entity is 'Systems_Internal_DocumentPrintImages'.
List of all entity renames can be found at https://testdb.my.erp.net/api/domain/odata/GetRenamedEntityTypes.
```

You have to change your application to use the new name.

 

## 2. Support both old and new versions

If you want your app to support both the old and the new version of ERP.net Domain API, you can check the version with the following function:

/GetVersion

 The function returns a JSON object with "version" property.

Example:

https://testdb.my.erp.net/api/domain/odata/GetVersion

Example response:

```
{
  "@odata.context": "https://testdb.my.erp.net/api/domain/odata/$metadata#Erp.OpenObject",
  "version": "24.1.5.41"
}
```

## 3. Automate entity type renames

If you want to implement some form of automation for these pesky renames, we support end-point, which returns all renames, along with some related data:

/GetRenamedEntityTypes

The function returns a JSON array with entity type rename containig OldName, NewName and Version (the version when the new name replaces the old name).

Example:

https://testdb.my.erp.net/api/domain/odata/GetRenamedEntityTypes

Example response:

```

  "@odata.context": "https://testdb.my.erp.net/api/domain/odata/$metadata#Collection(Erp.OpenObject)",
  "value": [
    {
      "OldName": "Systems_Core_DataEntryDefaultValues",
      "NewName": "Systems_Internal_DataEntryDefaultValues",
      "Version": "24.1.5.35"
    },
    {
      "OldName": "Systems_Core_ExtensibleDataObjects",
      "NewName": "Systems_Internal_ExtensibleDataObjects",
      "Version": "24.1.5.35"
    },
    ...
```

### Finding the Actual Entity Type Name
You can use the result of this function to determine the actual entity type name. Here is an example of how this can be achieved:

```
// The variable $renames contains the array of all entity type renames returned by the ~/GetRenamedEntityTypes function.
// The variable $entitySet is the provided entity set name, which may have been renamed.

// The $renames collection may contain entries where an entity type name has been renamed multiple times.
// To find the valid name, we need to iterate through all renames and update the $entitySet accordingly.

foreach (var rename in $renames)
{
    if (rename.OldName == $entitySet) 
    {
        $entitySet = rename.NewName;
    }
}

// After the loop, $entitySet will contain the actual entity type name.

```

## 4. List of renames

To view the list of renamed entity types, go to the following address:

[Renamed entity types](https://docs.erp.net/model/entities/renames.html)
