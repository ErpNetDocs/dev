Sometimes, we have to rename entity types. In one version, they are called X, and in the next version, they are called Y. Of course, we avoid doing this as much as we can. But sometimes it is unavoidable and has to be done.

For example, we have renamed "General.DocumentPrintImages" ("General_DocumentPrintImages" in the API) to "Systems.Internal.DocumentPrintImages".

This, naturally, is a problem for applications, which access these entities through the API.

To alleviate the problem, we have taken numerous measures:

 

## 1. Error message containing the new name

When you access an entity, which has been renamed, ERP.net (@name) returns an informative error message, which contains the new name of the entity type

For example:

```

The provided entity set name 'General_DocumentPrintImages' is no more supported.
The new name of this entity is 'Systems_Internal_DocumentPrintImages'.
List of all entity renames can be found at https://testdb.my.erp.net/api/domain/odata/GetRenamedEntityTypes.

```

You have to change your application to use the new name.

 

## 2. Support both old and new versions

If you want your app to support both the old and the new version of ERP.net (@name), you can check the version with the following function:

/GetVersion(...)

 

## 3. Automate entity type renames

If you want to implement some form of automation for these pesky renames, we support end-point, which returns all renames, along with some related data:

/GetRenames(...)

 

## 4. List of renames

To view the list of renamed entity types, go to the following address:

....(renames)
