# Welcome to the ERP.net Developer Documentation

Here you will find all the necessary resources to start developing applications, which consume the ERP.net services.

This documentation contains developer resources. For business logic and other technical documentation, you might want to check the [Technical Documentation](https://docs.erp.net/tech).

## ERP Instances
The ERP.net service is a hosted ERP service.
When you sign up at [erp.net](https://erp.net), you can create a new ERP Instance. 

An ERP Instance is, under the hoods, a database.
It allows managing multiple related legal entities (companies) in one instance, so, generally, it is not one company.

Each ERP Instance has a unique name. 
The instance can be accessed at:
https://<<instance_name>>.my.erp.net

For example, the demonstration database, DEMODB, is located at:

[https://demodb.my.erp.net](https://demodb.my.erp.net)

## The API
Each ERP Instance CAN have an API site. 
It "can" have, because, the API is a site, similar to other site, which the instance can launch. 
In order for the API to function, it needs to be configured and launched for the ERP Instance.

The common URL for the API site is /api/ inside the ERP instance. For example, the DEMODB has API site at:

[https://demodb.my.erp.net/api/](https://demodb.my.erp.net/api/)

## The Domain API
The Domain API is the primary means for accessing and manipulating data in the ERP Instance. 
It can currently be consumed in OData 4 format. 

The Domain API is an API proxy for the Domain Model of ERP.net.
It is targeted toward UI and service apps, which read and update moderate amounts of data (as the UI and service apps usually do).
The Domain API is NOT adequate for reading (dumping) very large amounts of data, which is usually done by BI applications.
BI applications should find other solutions or try to read data by small chunks.

The Domain API is located at /domain/odata/ within the API site. For DEMODB, this is at:

[https://demodb.my.erp.net/api/domain/odata/](https://demodb.my.erp.net/api/domain/odata/)

If you try the above link, it will ask you for user credentials. 
Most ERP Instances should, by default, be configured to NOT accept basic authentication.
However, for demonstration purposes, DEMODB is configured to allow it.
You can use user:"admin" / pwd:"123" to access the API of DEMODB.

## Sample Query
OData v4 allows the creation of URL-based queries.
When you query for data, you use queries, similar to this:
[https://demodb.my.erp.net/api/domain/odata/General_Products_Products?$top=10](https://demodb.my.erp.net/api/domain/odata/General_Products_Products?$top=10)

## The Query Builder
ERP.net has integrated visual Query Builder.
The Query Builder allows building queries, specific to the selected database (ERP Instance).
When you use the Query Builder for an ERP Instance, it allows you to select the user-defined data and calculated attributes.

The Query Builder allows the developers to create the query visually and then just re-use the query text, replacing the parameters.

To access the Query Builder for DEMODB, go to:
[https://demodb.my.erp.net/api/domain/querybuilder](https://demodb.my.erp.net/api/domain/querybuilder)

## The Query Tool
The Query Tool is simple Postman-like tool for querying the database.
It's mostly useful for transferring queries for issues, posts, etc.
It can be used to specify queries, function calls, updates, and generally any rest-based operation.

To access the Query Tool for DEMODB, go to:
[https://demodb.my.erp.net/api/domain/query](https://demodb.my.erp.net/api/domain/query)

##
