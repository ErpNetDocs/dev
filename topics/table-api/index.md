# Table API

The primary purpose of the Table API is to allow external BI (Business Intelligence) tools to quickly pull raw data for further analysis.

## Authentication

The Table API supports the following types of authentication:
* OAuth 2.0
* Basic Authentication

> [!CAUTION]
> Although Basic Authentication is supported, its use is strongly **NOT** recommended, because of its significantly lower level of security.

For more information, see [Authentication](../authentication/index.md).

## Tables

For information about the tables, which can be queried, see the [Table Model](https://docs.erp.net/model/tables/).

## Best practices for refreshing data when building external BI systems based on a TableAPI site

### Introduction

The first step in building a BI is loading (Extract) the data from the source database. This is done using a TableAPI site that allows an authorized user to access the raw data at the table level. 

It is essential to achieve optimum transfer speed by using the filtering capabilities provided on the primary information to achieve the minimum possible data refresh time. 

For this reason, here we describe best practices when using TableAPI (OData) data source to power PowerBI and BI data analysis platforms in general. The information is graded with increasing complexity and presented through examples for ease of understanding and allows for step-by-step code copying and testing.

It is necessary to follow these guidelines to avoid possible errors and delays when loading the data. 

There are two main ways in which the data can be downloaded:

1. Using OData.Feed to read the data from the source;
2. Using Web.Contents to read the data from the source;
   
In both ways, it is necessary to manually set parameters and filters with which the queries will be invoked to achieve optimal results. This includes setting the fields returned by the query (listed in the select clause of the query) and setting the filter that will be applied to determine the data. 

The filters can be either on the fields of the table to be loaded, or on related tables at a higher level in the hierarchical model. This means that a table containing document rows can be filtered by fields that are in the document head, i.e. filtering by Document_Date can be implemented, which in turn will allow incremental refreshing of the data.

Both options use authentication of user rights via Basic identification (username and password). These access authorization parameters will be entered in both the PowerBI Desktop and the published online PowerBI model.

For a small base, it is possible not to make the presets described below. However, the growth of databases, the extension of the BI model, and the likelihood of changes in the configuration of services make it imperative to define some initial parameters to be used throughout the BI project.

### Pre-defining and setting of important common parameters

The main parameters that are required for operation are:

- **RangeStart** - system mandatory parameter when using incremental refresh, setting the start time for a subperiod
- **RangeEnd** - a system required parameter when using incremental refresh, setting the end time for a subperiod
- **TopCount** - a user parameter to facilitate the editing of the project, containing the number of records to download
- **baseURL** - user parameter specifying the site(TableAPI) from which the data is downloaded

The RangeStart and RangeEnd parameters are of type Date/Time and must be set to values in order to load preview data when working with the model in PowerBI Desktop.

The TopCount parameter is of type Decimal number and sets the number of records to be fetched with a single query. In the PowerBI Desktop development environment, there must be a relatively small value, such as 500, to load preview data quickly.

The baseURL parameter is of type **Text** contains the address of the TableAPI site, and it is a good idea to have it defined this way because it is very easy to change it from one location in the entire project should any change occur. 

For example, it could have the following value: 

**https://test-tableAPI.erp.net**

### Loading model information

Select "OData feed" from the menu using the "**New source**" button. 

Fill in the data as shown in the picture (assuming we have defined the baseURL parameter as shown above).

The data for the available objects is then loaded and you can see what each one looks like. This is necessary to determine the field names we will need to filter by or use when filtering by a reference field. 

The following image shows what the **Crm_Sales_Orders** object looks like and in particular the field that is used to reference the document head we need to filter on (**Document_Reference** field).

We can choose to load data directly this way, as it supports additional filtering by fields in the table and selecting which fields to load. This method **does NOT SUPPORT** filtering by reference fields and is therefore not applicable if incremental refresh is to be used!

### Load data via OData.Feed read from source

This method is only suitable for testing in PowerBI Desktop because it is **NOT SUPPORTED** by online PowerBI. It is described because it is basic information needed to understand and work with TableAPI queries.

Some of the steps described below are also applicable to other data sources. 

General guidelines for operation are given through the following points:

1. Set the data source for each Query using TableAPI in the first step (Source) as follows:

= OData.Feed(baseURL & "/tableapi/odata/{Table_Name}{Parameters}", null, [Implementation="2.0"]) 
      
{Table_Name} - replaced with the name of the table to be loaded, e.g. Gen_Documents;
{Parameters} - replaced with OPTIONAL parameters to the query.

If the access authorization is not set yet, it is necessary to select one with the Basic type and specify the values of the "User name" and "Password" parameters for the user who has access to the TableAPI application.

The parameter section (if present) must begin with a "?" character. Parameters are separated from each other by the "&" symbol.

The possible parameters are:

- "**$filter=**" - specifies the conditions that the returned data will meet;
- "**$select=**" - sets the fields to be returned by the query;
- "**$top=**" - sets the maximum number of records to return from the query.

2. Use permanent filtration.

Add the appropriate filter in the URL using the options supported by TableAPI. 

Example of filtering table Gen_Document by active (Void=false) and released (State>=30) documents:

let
Source = OData.Feed(baseURL & "/tableapi/odata/Gen_Documents?$filter=Void eq false and State ge 30", null, [Implementation="2.0"])
in
Source

To get this step code, you can add the following in the OData source URL (in addition to the one shown in Figure 2):

/tableapi/odata/**Gen_Documents?$filter=Void eq false and State ge 30**

3. Add filtering by fields with listed values.

To the above example, we also add filtering by Entity_Name for values 'Inv_Transactions' and 'Crm_Sales_Orders'.

=OData.Feed(baseURL & "/tableapi/odata/Gen_Documents?$filter=Void eq false and State ge 30 and Entity_Name in ('Inv_Transactions','Crm_Sales_Orders')", null, [Implementation="2.0"])

It should be noted here that support for filtering by enumerated values has been added to TableAPI, allowing it to be used directly from the PowerBI Desktop interface. 

Here is what the query looks like that is generated by PowerBI Desktop before adding the filtering:

HTTP GET /tableapi/odata/Gen_Documents?$filter=Void eq false and State ge 30&$top=1000

This happens if the Void and State filterings for Gen_Documents, which is again selected by specifying it from the interface, are selected entirely through the interface. 

The steps that are generated in this process can be viewed in the Advanced editor:

let
    Source = OData.Feed(baseURL & "/tableapi/odata/", null, [Implementation="2.0"]),
    Gen_Documents_table = Source{[Name="Gen_Documents",Signature="table"]}[Data],
    #"Filtered Rows" = Table.SelectRows(Gen_Documents_table, each([Void] = false)),
    #"Filtered Rows1" = Table.SelectRows(#"Filtered Rows", each [State] >= 30)
in
    #"Filtered Rows1"
    
Again through the interface, we can also add Entity_Name filtering as shown in the following image.

This will trigger the creation of the next step with code:

= Table.SelectRows(#"Filtered Rows1", each([Entity_Name] = "Crm_Sales_Orders" or [Entity_Name] = "Inv_Transactions"))

The following query will be executed to the data source on which the append from this filtration is marked:

HTTP GET /tableapi/data/Gen_Documents?$filter=Void eq false and State ge 30 and (Entity_Name eq 'Crm_Sales_Orders' or Entity_Name eq 'Inv_Transactions')&$top=1000

This example shows the maintenance in the TableAPI filter construct: 

and (Field_Name eq 'Value1' or Field_Name eq 'Value2' ... or Field_Name eq 'ValueN')

Everything described in this section can be used for nomenclatures that will be fully loaded or do not need filtering by reference fields.

4. Filtering by date type fields.
As an example, we will use the Document_Date field to select documents only from the first 6 months of the year.

=OData.Feed(baseURL & "/tableapi/odata/Gen_Documents?$filter=Void eq false and State ge 30 and Document_Date ge 2023-01-01T00:00:00Z and Document_Date le 2023-06-30T00:00:00Z", null, [Implementation="2.0"])

Note the date format 'YYYY-MM-DDThh:mm:ssZ' and observe it when using dates.

5. Filter by referenced fields (fields from related tables that are not present in the current table)

An example of such a query is filtering records from the Inv_Transactions table by taking only those that are for unreferenced(Void=false) and released(State>=30) documents.

=OData.Feed(baseURL & "/tableapi/odata/Inv_Transactions?$filter=Document_Reference/Void eq false and Document_Reference/State ge 30", null, [Implementation="2.0"])

As can be seen, the filtering is similar to that in item 2, the difference being the specification of the reference to the field to filter on. The names of the fields pointing to the reference tables can be defined as shown in Figure 3. 

In order to support filtering by reference, this needs to be explicitly written in the Table model documentation.

6. Filtering by fields from tables present in the Owner Tables Hierarchy list.
   
Filtering by fields for tables present in the Owner Tables Hierarchy list described in the Table model documentation is always supported! 

In addition, you can filter on a field that is not in a directly related table (located more than one level away in the hierarchy). 

For example, if we want to filter Inv_Transaction_Lines by the date of the document in which the rows are included (Document_Date field of Gen_Documents) we will have to pass through two consecutive references in the way shown in the example:

=OData.Feed(baseURL & "/tableapi/odata/Inv_Transaction_Lines?$filter=Transaction_Reference/Document_Date ge 2023-01-01T00:00:00Z and Transaction_Reference/Document_Reference/Document_Date le 2023-01-31T23:59:59Z", null, [Implementation="2.0"])

This is due to the hierarchical relationship between the tables which is obtained from the following connectivity scheme (Field(Table) format):

Transaction_Reference(Inv_Transaction_Lines) -> Transaction_Id(Inv_Transactions)
Document_Reference(Inv_Transactions) -> Id(Gen_Documents)

7. Data filtering by date of document in which it is included.
Use of OData feed does NOT allow incremental refresh of data. The example below is given using Web.Contents as a way to fetch data.

=Json.Document(Web.Contents(baseURL & "/tableapi/odata/Inv_Transaction_Lines",[Query=[#"$filter" = "Transaction_Reference/Document_Reference/Document_Date ge " & DateTime. ToText(RangeStart,[Format="yyyy-MM-dd'T'HH:mm:ss'Z'", Culture="en-US"] ) & " and Transaction_Reference/Document_Reference/Document_Date le " & DateTime. ToText(Date.EndOfDay(Date.AddDays(RangeEnd,-1)),[Format="yyyy-MM-dd'T'HH:mm:ss'Z'", Culture="en-US"]]]], 65001)

Since a comparison using 'le'(comparison operator <=) must be used, the following statement is used to calculate the correct end date of the period:

Date.EndOfDay(Date.AddDays(RangeEnd,-1))

> [!IMPORTANT]
> In the filter, you can use only:
> - the logical operator 'and';
> - the comparison operators 'eq', 'le', 'ge';
> - operator 'like';
> - comparison operator with list of values 'in'; <br>
> If a logical OR operator is to be used in the filter, the query must be split into several separate queries that do not contain an
> OR, then all of them must be combined into a single query using "Append queries".
> The exception is the above-described statement for filtering by enumerated values of the type: <br>
>        and (Field eq 'Value1' or Field eq 'Value2' ... or Field eq 'ValueN') <br>
> which is equivalent to the standard supported statement <br
>        and Field in ('Value1', 'Value2', ...,'ValueN')

In PowerBI, it is possible to set up incremental updating. 

For this purpose, it is necessary to create two parameters (RangeStart, RangeEnd) of type DateTime whose values are automatically changed according to the incremental updating policy set for the specific object in PowerBI. 

Here, it is now necessary to manually modify the query after the initial URL is set, because there is no way to add the information from the input parameters formatted in the desired way.

### Using incremental refresh to accelerate data updates in PowerBI

In order to achieve filtering for lines and other tables by Document_Date field of the document head, it is necessary to use a filter by reference field. 

However, an incremental refresh problem then arises. It is not possible to set the required filtering without manually setting parameters by which to filter, but PowerBI online does not support refreshing from the source so set.

**It is imperative that the general parameters described at the beginning are set before proceeding!**

Data loading via Web Content with the possibility to filter also by reference fields and compatibility with incremental refresh.

We apply the following approach:

1. We use Web Content as data source;
2. We manually set all possible filters that should be applied in the data source to reduce the transferred data as much as possible;
3. We manually set the list of fields we want to be returned again in order to reduce the amount of data;
4. We convert the returned data to JSON;
5. We convert the result to a table, after which other processing can continue.
   
It would be easiest to use the example provided, modifying as needed. 

Example of data filtering for table Crm_Sales_Order_Lines_Table:

```
let
    strEntity = "Crm_Sales_Order_Lines",
    strEntityHeadReference = "Sales_Order_Reference",
    strSelectFields = "",
    strEntHead = strEntityHeadReference & (if strEntityHeadReference = "" then "" else "/"),
    strDocHead = strEntHead & "Document_Reference/",
    strFilter = strDocHead &"Void eq false and " & strDocHead & "State ge 30 and " & strDocHead & "Document_Date ge " & strRangeStart &" and " & strDocHead & "Document_Date le "& strRangeEnd,
    strRangeStart = DateTime.ToText(DateTime.From(RangeStart),[Format="yyyy-MM-dd'T'HH:mm:ss'Z'", Culture="en-US"]),
    strRangeEnd = DateTime.ToText(Date.AddDays(DateTime.From(RangeEnd),-1),[Format="yyyy-MM-dd'T'HH:mm:ss'Z'", Culture="en-US"]),
    Source1 = Json.Document(Web.Contents(baseURL & "/tableapi/odata/" & strEntity, 
    [Query=[#"$filter" = strFilter, #"$select" = strSelectFields, #"$top" = Text.From(TopCount)], Timeout=#duration(0, 2, 0, 0)]), 65001),
    ResultList = Source1[value],
    value = if List.NonNullCount(ResultList) = 0 then List.Union({ResultList, {null}}) else ResultList,
    #"Converted to Table" = Table.FromList(value, Splitter.SplitByNothing(), null, null, ExtraValues.Error),
    #"Result" = Table.ExpandRecordColumn(#"Converted to Table", "Column1", {"Sales_Order_Id", "Line_No", "Product_Id", "Quantity", "Quantity_Unit_Id", "Product_Price_Id", "Unit_Price", "Line_Discount_Id", "Line_Standard_Discount_Percent", "Line_Custom_Discount_Percent", "Line_Amount", "Line_Store_Id", "Requested_Quantity", "Quantity_Base"}, {"Sales_Order_Id", "Line_No", "Product_Id", "Quantity", "Quantity_Unit_Id", "Product_Price_Id", "Unit_Price", "Line_Discount_Id", "Line_Standard_Discount_Percent", "Line_Custom_Discount_Percent", "Line_Amount", "Line_Store_Id", "Requested_Quantity", "Quantity_Base"})
in
    #"Result"
```

### Add new table with incremental step-by-step refresh capability

1. Switch to edit data sources mode (select from Home -> Queries -> Transform data);

2. Create a new query(table) from Home -> New Source -> Blank Query.

Then, by clicking Home -> Query -> Advanced editor, we open the window in which the M code of the query is written.  

The result is as follows:

3. Replace the text in the window with the above sample text(for Crm_Sales_Order_Lines_Table);
 
4. We edit the value of the strEntityHeadReference variable with the reference field to the new entity whose lines we will load.

Edit the value of strEntity with the name of the new entity. If it will be loaded from the head of the entity, the field should be empty. If we are going to load, for example, Crm_Sales_Orders_Table, we need to edit the fields as follows:

    strEntity = "Crm_Sales_Orders",
    strEntityHeadReference = "",
    
If it is loaded from Gen_Documents, then we need to change the strDocHead so

    strDocHead = "",

Then, select the columns to be included in the table by deleting the last step of the conversions and using the expanding (the yellow highlighted icon) from the "Converted to Table" step select the necessary fields as shown in the picture:

This is assuming that the string in strSelectFields is empty (strSelectFields="").

If it is filled with certain fields such as

strSelectFields="Sales_Order_Id, Customer_Id, Store_Id, Sales_Person_Id, Dealer_Id, Document_Currency_Id",

only these will be visible, and only these will be returned from TableAPI, which will save time in data transfer and speed up loading.

For this reason, specifying the field names to be returned by the query is highly recommended.

There is one line in the above code

    value = if List.NonNullCount(ResultList) = 0 then List.Union({ResultList, {null}}) else ResultList,
    
which may seem redundant, but is very important for the synchronization to run without error. 

It is used to check the returned result and if no data is returned, an empty line is added. Missing data causes an error in the next steps of conversion to the tabular form in which we need the data. An error would occur if any information is missing in any incremental refresh period.

Executing the above code will result in sending the following data fetch command:

HTTP GET /tableapi/data/Crm_Sales_Orders? $filter=Document_Reference/Void eq false and Document_Reference/State ge 30 and Document_Reference/Document_Date ge 2015-01-01T00:00:00Z and Document_Reference/Document_Date le 2024-01-09T00:00:00Z&$select=Sales_Order_Id, Customer_Id, Store_Id, Sales_Person_Id, Dealer_Id, Document_Currency_Id&$top=500

The Document_Date filtering and the value of the $top parameter are determined by the parameters we have defined. This is for PowerBI Desktop queries. When querying from online PowerBI and an incremental update is defined for the object, the parameter values will be automatically populated and separate queries will be generated and executed for each update period.

Once the project is published to PowerBI, the first thing to set is to change the TopCount parameter value to a large enough value, for example 500000000. 

After that, a manual Refresh can be run at the appropriate time because this will cause the data to be fully loaded (from the incremental refresh set in the incremental refresh processing period).

Let's also pay attention to the Timeout parameter set in the sample query, with value

Timeout=#duration(0, 2, 0, 0)

It is used to set the timeout of the single data download request and the above setting changes it to 2 hours. This is the maximum time given to one request(each incremental period) at a time. 

In PowerBI, this value defaults to 600 sec, which can be small and insufficient, especially during the initial data load when the archive period data is loaded. The default time for a single query may not be enough if the settings are, for example, as in the picture:

Archival data periods will be one year in size. This means that the amount of data to be loaded will be very large and the query will be slow to execute. Either we need to increase the timeout, as we have done in the example, or change the period to an equivalent but smaller size. 

We can set the following:

Archive data starting **60 Month** before refresh date

which is equivalent to the above settings, but the period of data that is processed with one query will be one month. 

This will reduce the execution time and probability of timeout of single queries on initial data load. 

The archive period data is loaded once on the first Refresh after the project is published. Data loading in the periods defined by Incrementally refresh data is done at each Refresh, but the refresh time will not increase proportionally to the total data in the database, but to the volume of data in the periods being processed.

The use of TableAPI allows multiple queries to pull data from the database at the same time without overloading the server and significantly affecting the underlying work. 

### Optimized loading of custom features

Custom features are also used in the analyses performed in PowerBI. Because their volume is significant, it is very important to load and work with them in an efficient way. 

#### Optimized loading of all necessary features

For this purpose, we use the following code defining a query named Gen_Property_Values:

```
let
    Source = OData.Feed(baseURL & "/tableapi/odata/", null, [Implementation="2.0"]),
    Gen_Property_Values_table = Source{[Name="Gen_Property_Values",Signature="table"]}[Data],
    #"Filtered Rows1" = Table.SelectRows(Gen_Property_Values_table, each ([Property_Id] = "18eb8480-a19b-4336-b935-c2715730f988" or [Property_Id] = "28e65696-2d8e-4e22-9932-686737235d88" or [Property_Id] = "41324c1d-6f64-4fa3-85e4-4c99fcf4f407" or [Property_Id] = "46d58fad-4ef2-409e-ba92-b5c55f7ea20b")),
    #"Removed Other Columns" = Table.SelectColumns(#"Filtered Rows1",{"Entity_Item_Id", "Property_Id", "Property_Value", "Description"}),
    #"Filtered Rows" = Table.Buffer(#"Removed Other Columns")
in
    #"Filtered Rows"
```

In this example, the four Guid feature numbers listed are randomly selected. You need to replace and supplement them with the IDs of the features you need. 

The construct 

... or [Property_Id] = "Guid"

can be reduced or expanded with more members.

Note the operation: 

    #"Filtered Rows" = Table.Buffer(#"Removed Other Columns")
    
It specifies that the data will be statically buffered in memory. In the following operations that use Gen_Property_Values (the query we define) as the source, data will be taken from the buffer in memory and will save a lot of re-fetching time.

Another optimization comes from loading only the required fields listed in the Table.SelectColumns operation. This reduces the returned data and speeds up the query. This is possible thanks to the Folding support when using a **OData feed** data source.

The processing can continue by creating a new query (e.g. Entity1_Property_Values) and the following code:

```
let
    Source = Gen_Property_Values,
    #"Filtered Rows" = Table.SelectRows(Source, each ([Property_Id] = "41324c1d-6f64-4fa3-85e4-4c99fcf4f407" or [Property_Id] = "46d58fad-4ef2-409e-ba92-b5c55f7ea20b") and ([Property_Value] <> null and [Property_Value] <> "")),
    #"Added Custom" = Table.AddColumn(#"Filtered Rows", "Param1", each if [Property_Id]="41324c1d-6f64-4fa3-85e4-4c99fcf4f407" then [Property_Value] else null),
    #"Added Custom1" = Table.AddColumn(#"Added Custom", "Param2", each if [Property_Id] = "46d58fad-4ef2-409e-ba92-b5c55f7ea20b" then [Property_Value] else null),
    #"Added Custom2" = Table.AddColumn(#"Added Custom1", "Descr1", each if [Property_Id] = "41324c1d-6f64-4fa3-85e4-4c99fcf4f407" then [Description] else null),
    #"Added Custom3" = Table.AddColumn(#"Added Custom2", "Descr2", each if [Property_Id]="46d58fad-4ef2-409e-ba92-b5c55f7ea20b" then [Description] else null),
    #"Grouped Rows" = Table.Group(#"Added Custom3", {"Entity_Item_Id"}, {{"Par1", each List.Max([Param1]), type nullable text}, {"Descr1", each List.Max([Descr1]), type nullable text}, {"Par2", each List.Max([Param2]), type nullable text}, {"Descr2", each List.Max([Descr2]), type nullable text}})
in
    #"Grouped Rows"
```

On the first step, we need to select the Gen_Property_Values query we created earlier as the data source, which buffers the data load. 

The next step does the filtering, taking the values for only two of the properties (that apply to an entity) that are not null or empty ("") in order to optimize and reduce the data to process. 

If needed, the filtering list can be expanded or reduced according to the features being processed and already loaded by the previous query.

In the following steps, new columns are created that contain only values for a particular Property_Id. This is necessary to prepare the data for grouping by Entity_Item_Id, which will convert the table to one with no more than one row for each entity. 

This will allow us to link it to the entity table to which these custom properties apply. These steps must be edited or completed for the specific Property_Id being processed.

As a final step, the grouping by Entity_Item_Id is performed and the data is ready to be associated with the Entity data it refers to. If we were to go ahead and add the link to the same query, the linking dialog would look something like this: 

The relationship must be **Outher** (not all entries in the entity table have a match in the feature value table) and in this case it is **Right**, because the base table where all the data is is the second (named **Documents_ODATA**). 

If we added the properties to Documents_ODATA in Entity1_Property_Values, then we would have a Join Kind of type Left Outher.

> [!WARNING]
> Reading data from the Gen_Property_Values_Table is always slow because it causes a scan( full traversal) of the table which is very large. This has a very negative impact on SQL server and slows down PowerBI updates. Therefore, it is imperative to follow the recommended and shown way of working! There may be other optimal ways, but the table scanning and the large execution time of a query should always be considered. <br>
> The only exception to the above might be if you filter by a list of values for Entity_Item_Id, in which case the query will execute quickly and use SEEK in the database. But due to the nature of BI, this option is hardly applicable.

When reading user attributes, there is no way to filter by date of the document they refer to and therefore no way to use incremental refresh. For this, the example uses the standard OData feed as the data source.

### PowerBI setup after uploading the project

These settings are required to set the access rights when connecting to the data source (in this case, TableAPI). 

It is important to set the correct settings so as not to create additional problems and loads when loading the data.

For each source (table) that will be read from TableAPI when using the WEB Content access method, it is necessary to set the rights separately. 

For a source using OData Feed, the necessary access rights are set once. Only the **Basic authentication** method is supported!

For these settings, it is necessary to check the box "Skip test connection", as shown in the picture.

For the OData source, it is not a problem to leave this check box empty. It is even a good idea to first set the access for the OData source with an empty check box, thus checking that the correct credentials (user, password) are set. 

If there is a problem, you will receive a notification about this. When there is no problem, you can set the access for the others in the same way, but with a check box.

Before the first run to load the data after uploading the project, it is necessary to set the “TopCount” parameter to a value that does not limit the volume of the loaded data (e.g. 500000000, as shown in the picture).

If for some reason the data source has been renamed, this can be easily corrected here by simply changing the “baseURL” parameter to match the correct one, without the need for a correction in the project and a new upload.

### Conclusion

These techniques implement the PowerBI incremental refresh capability, which makes the refresh time relatively constant and proportional to the growth of data in the last selected refresh periods.

This capability has been tested in practice, with 4 parallel connections, which did not lead to a significant load on either the TableAPI site, AppServer, or SQL Server. 

We can assume that Refresh even with 5-10 connections will still be within the permissible load limits and will not significantly affect the system's performance.

However, it is best to track the specific refresh process to determine its impact and the load it causes. This will make it possible to choose appropriate values ​​for the number of parallel refresh requests that are permissible without interfering with the normal operation of the system.
