using ErpNet.Api.Client;
using ErpNet.Api.Client.DomainApi;
using ErpNet.Api.Client.DomainApi.Crm;

var identityClient = new ErpNetServiceClient(
    "https://demodb.my.erp.net",
    "ServiceDemoClient",
    "DEMO");

var apiRoot = await identityClient.GetDomainApiODataServiceRootAsync();

var service = new DomainApiService(
    apiRoot,
    identityClient.GetAccessTokenAsync);

var command = service.Command<Customer>()
    .Filter(c => c.Number.Contains("001"))
    .Top(10);

Console.WriteLine($"request: {command}");
Console.WriteLine("===================");
Console.WriteLine($"response: ");

var customers = await command.LoadAsync();

foreach (var customer in customers)
    Console.WriteLine(customer.ToJson());

Console.ReadLine();
