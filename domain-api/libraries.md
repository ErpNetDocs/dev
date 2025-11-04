# Domain API Libraries

The **Domain API** is based on the [OData protocol](https://www.odata.org/).  
It provides object-oriented access to the data exposed by @@name domain objects.

The Domain API is an HTTP RESTful API and can be accessed through standard HTTP requests.  
However, using a client library simplifies authentication, request building, and result parsing.

## @@name Libraries

### .NET

#### [ErpNet.Api.Client](https://github.com/erpnet/ErpNet.Api.Client)

**ErpNet.Api.Client** is the official .NET client library for @@name APIs.  

It supports both **Table API** and **Domain API**, built on top of the OData standard.

**NuGet packages:**

- [ErpNet.Api.Client](https://www.nuget.org/packages/ErpNet.Api.Client)
- [ErpNet.Api.Client.DomainApi](https://www.nuget.org/packages/ErpNet.Api.Client.DomainApi)

**Example:**

```csharp
var erpNetDatabaseUri = "https://demodb.my.erp.net";

// Create ErpNetServiceClient object to obtain a service access token.
// In the database there must be trusted application registration with 
// ApplicationUri: "ServiceDemoClient" and ApplcationSecretHash=Sha256("DEMO").
ErpNetServiceClient identityClient = new ErpNetServiceClient(
    erpNetDatabaseUri,
    "ServiceDemoClient",
    "DEMO");

// Obtain the web address of the DomainApi for the database.
var apiRoot = await identityClient.GetDomainApiODataServiceRootAsync();

// Create the service
DomainApiService service = new DomainApiService(
    apiRoot,
    identityClient.GetAccessTokenAsync);

// Create query command with $top, $filter, $select and $expand clauses.
ODataCommand command = new ODataCommand("General_Products_Products");
command.Type = ErpCommandType.Query;
command.FilterClause = "contains(PartNumber,'001')";
command.SelectClause = "Id,PartNumber,Name,ProductGroup";
command.ExpandClause = "ProductGroup($select=Id,Code,Name)";
command.TopClause = 5;

// Get the ODATA json result as IDictionary<string,object>
var result = await service.ExecuteDictionaryAsync(command);
```

**Using typed entities:**

```csharp
// Use anonymous types for $select and $expand clause
var cmd = service.Command<Product>()
    .Top(5)
    .Filter(p => p.PartNumber.Contains("001"))
    .Select(p => new 
    {
        p.Id, 
        p.PartNumber, 
        p.Name, 
        ProductGroup = new 
        { 
            p.ProductGroup.Id, 
            p.ProductGroup.Code, 
            p.ProductGroup.Name
        }
    });

var result = await cmd.LoadAsync();

// The HTTP command is:
// GET General_Products_Products?$top=5&$filter=contains(PartNumber,'001')
// &$select=Id,PartNumber,Name,ProductGroup
// &$expand=ProductGroup($select=Id,Code,Name)
```

For more samples, see:  
[ErpNet.Api.Client Samples](https://github.com/ErpNetDocs/dev/tree/master/domain-api/samples/src/dotnet/ErpNet.Api.Client.Samples)

## Generic OData Libraries

The Domain API follows the OData standard, so any OData-compliant library can be used to access it.

You can explore a list of available client libraries for different programming languages at:  
[https://www.odata.org/libraries/](https://www.odata.org/libraries/)
