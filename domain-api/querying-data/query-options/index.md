# Query options

Query options allow you to control the amount and order of the data that a data service returns for the resource identified by the URI.

OData system query options are provided by the OData framework and documented in detail in the OData specification at <http://docs.oasis-open.org/odata/odata/v4.0/odata-v4.0-part2-url-conventions.html>.

For a great introduction to query options, read the [OData query data tutorial](https://www.odata.org/getting-started/basic-tutorial/#queryData).

## List of supported query options

Query Option | Origin | Description
-------------|--------|------------
$count | OData | The $count system query option allows clients to request a count of the matching resources included with the resources in the response. The $count query option has a Boolean value of true or false.
$expand | OData | The $expand system query option specifies the related resources or media streams to be included in line with retrieved resources.
[$filter](filter.md) | OData | The $filter system query option allows clients to filter a collection of resources that are addressed by a request URL.
$metadata | OData | Returns the data model (which is the structure of all resources).
$orderby | OData | Specifies an expression for determining which values are used to order the collection of records identified by the resource path section of the URI.
[$select](select.md) | OData | Limits the data to the specified attributes.
$skip | OData | The $skip query option requests the number of items in the queried collection that are to be skipped and not included in the result.
$skiptoken | OData | The $skiptoken is an opaque, server-generated identifier used for server-side paging that directs the client to the specific starting point of the next subset of results.
$top | OData | The $top system query option requests the number of items in the queried collection to be included in the result.
[$search](search.md) | OData | The $search system query option allows clients to request items within a collection matching a free-text. Each entity implements the searching in a different way.
[options](options.md) | Extension | List of comma separated options/flags that affect the behavior of the system.

> [!note]
> Options marked as "Extension" are not part of the OData standard and are specific to the @@name Domain API.
> They do not use $ in front of their name, as it is reserved for standard OData query options.
