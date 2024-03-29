# Recommendations for using TableAPI as a data source for BI

Respecting the technical capabilities of the TableAPI, such as its filtering and data presentation capabilities, is necessary to ensure fast, efficient, and trouble-free operation. 
To achieve this, the following recommendations should be followed:

1) **Select only the necessary tables from which you will load data.**
2) **Filter the table as early as possible by using solid constraints that the data must meet. 
Negative constructions, OR statements, and unsupported types of filtering should not be used.
This should be aligned with the documentation for each field being filtered.**

    For example, do not use "<>", "<", ">", "OR", "NOT" as they are not supported.  
    The supported constructions are _**"AND"**_ ,_**"="**_ (including _**"= null"**_), _**"<="**_, _**">="**_.  

4) **Remove unnecessary fields as early as possible. Here it matters whether you choose "Remove other columns" or "Remove columns" (different functions, where one keeps the listed columns and the other removes them). Use the former ("Remove other columns"), which will ensure that if there is a modification in the table structure, the result will remain the same.**  
5) **Supplement the data (add rows) from other sources if more complex data filtering containing OR clauses is required. 
All OR-separated filters should be divided into several sub-filters, where each sub-filter requires a separate query to the same table with the respective sub-filter. 
Finally, all these sub-queries should be combined into one table by appending queries.**  
6) **When filtering data based on related tables (filter is not on a field from the current table), use table merging (Merge queries), where the main table to which you are making a connection should already be filtered, and use the "Inner join - only matching rows" type of join.**  

    For example, if you need to filter only certain documents from **Crm_Sales_Orders**, you should first filter **Gen_Documents** for the desired documents (based on *Entity_Name*, *Void*, *State*, *Document_Date*, etc.), and then merge **Crm_Sales_Orders** with **Gen_Documents** using an __*Inner join*__ and specifying the corresponding fields *Document_Id* (**Crm_Sales_Orders**) **->** *Id* (**Gen_Documents**).  
7) **If the tables have a relatively small volume (number of records), you may not need to follow the above filtering recommendations, as in that case, the entire table will likely be loaded into BI, and the filtering will be applied afterwards by BI.**
