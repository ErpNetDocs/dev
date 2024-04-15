using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Linq;
using ErpNet.Api.Client;
using ErpNet.Api.Client.DomainApi;
using ErpNet.Api.Client.DomainApi.General.Products;
using ErpNet.Api.Client.DomainApi.Crm.Sales;
using ErpNet.Api.Client.DomainApi.General;
using ErpNet.Api.Client.DomainApi.Crm;
using ErpNet.Api.Client.DomainApi.General.Contacts;
using ErpNet.Api.Client.OData;
using System.Diagnostics;
using System.ComponentModel;
using System.Reflection;
using ErpNet.Api.Client.DomainApi.Logistics.Inventory;

namespace ConsoleApp1
{
    static class Samples
    {
        public class Sample
        {
            public Sample(string name, string description, Func<DomainApiService, Task> action)
            {
                Name = name;
                Description = description;
                Action = action;
            }

            public string Name { get; }
            public string Description { get; }
            public Func<DomainApiService, Task> Action { get; }

            public async Task ExecuteAsync(DomainApiService service)
            {
                try
                {
                    await Action.Invoke(service);
                }
                catch (ODataException ex)
                {
                    Console.WriteLine("Error:");
                    Console.WriteLine($"  type: {ex.Type}");
                    Console.WriteLine($"  code: {ex.Code}");
                    Console.WriteLine($"  message: {ex.Message}");
                    Debug.WriteLine($"  info: {ex.Info}");
                }
            }
        }

        static IEnumerable<Sample> AllSamples()
        {
            foreach (var mi in typeof(Samples).GetMethods(BindingFlags.NonPublic | BindingFlags.Static))
            {
                var pars = mi.GetParameters();
                if (mi.ReturnType == typeof(Task) && pars.Length == 1 && pars[0].ParameterType == typeof(DomainApiService))
                    yield return new Sample(
                        mi.Name,
                        mi.GetCustomAttribute<DescriptionAttribute>()?.Description ?? mi.Name,
                        (svc) => (Task)mi.Invoke(null, new object[] { svc }));
            }
        }


        public static readonly Sample[] List = AllSamples().ToArray();

        public static async Task<DomainApiService> CreateServiceAsync(string erpNetDatabaseUri)
        {
            ErpNetServiceClient identityClient = new ErpNetServiceClient(
                erpNetDatabaseUri,
                "ServiceDemoClient",
                "DEMO");

            var apiRoot = await identityClient.GetDomainApiODataServiceRootAsync();

            DomainApiService service = new DomainApiService(
                 apiRoot,
                 identityClient.GetAccessTokenAsync);
            return service;
        }

        // Sample methods
        [Description("Use untyped commands")]
        static async Task UseUntypedCommandsAsync(DomainApiService service)
        {
            //LOAD BY FILTER
            Console.WriteLine("Query products");
            ODataCommand command = new ODataCommand("General_Products_Products");
            command.Type = ErpCommandType.Query;
            command.FilterClause = "contains(PartNumber,'001')";
            command.SelectClause = "Id,PartNumber,Name,ProductGroup";
            command.ExpandClause = "ProductGroup($select=Id,Code,Name)";
            command.TopClause = 5;
            Console.WriteLine(command);
            var stream = await service.ExecuteStreamAsync(command) ?? throw new Exception("Unexpected null result.");
            Guid productId = Guid.Empty;
            Guid productGroupId = Guid.Empty;
            // We use ReadODataQueryResult if we don't need the entire result into memory.
            // This way we use less memory.
            stream.ReadODataQueryResult((obj) =>
            {
                productId = Guid.Parse((string)obj["Id"]);
                productGroupId = Guid.Parse(obj.Member<string>("ProductGroup.Id"));
                Console.WriteLine($"{obj["PartNumber"]}: {obj.Member("Name.EN")}");
            });
            // Alternatively we can call service.ExecuteDictionaryAsync that parses the JSON result into IDictionary<string,object?>
            

            //LOAD BY ID
            Console.WriteLine("Get entity by id");
            // Assuming we know the product id
            command = new ODataCommand("General_Products_Products");
            command.Key = productId;
            command.Type = ErpCommandType.SingleEntity;
            Console.WriteLine(command);
            var product = await service.ExecuteDictionaryAsync(command);
            Console.WriteLine($"{product["PartNumber"]}: {product.Member("Name.EN")}");

            // INSERT
            Console.WriteLine("Insert new product");
            command = new ODataCommand("General_Products_Products");
            command.Type = ErpCommandType.Insert;
            var newData = new Dictionary<string,object?>
            {
                ["ProductGroup@odata.bind"] = $"General_Products_ProductGroups({productGroupId})",
                ["Name"] = new { DE = "Äpfel", EN = "Apple" },
            };
            command.Payload = JsonHelper.ToJson(newData);
            Console.WriteLine(command);
            // Insert the product
            product = await service.ExecuteDictionaryAsync(command);
            Console.WriteLine($"{product["PartNumber"]}: {product.Member("Name.EN")}");
        }

        [Description("Use typed filter functions")]
        static async Task UseFilterFunctionsAsync(DomainApiService service)
        {
            async Task ExecuteCommand(ODataCommand command)
            {
                Console.WriteLine(command);
                Console.WriteLine(await service.ExecuteStringAsync(command));
                Console.WriteLine();
            }

            Console.WriteLine("CONTAINS");
            // Contains on string property
            ODataCommand command = service.Command<Product>()
                .Filter(p => p.PartNumber.Contains("001"))
                .Select(p => new { p.PartNumber, p.Name })
                .Top(10);
            Console.WriteLine(command);
            await service.ExecuteStringAsync(command);

            // Contains on MultilanguageString property
            command = service.Command<Product>()
                .Filter(p => p.Name.Contains("001"))
                .Select(p => new { p.PartNumber, p.Name })
                .Top(10);
            await ExecuteCommand(command);


            Console.WriteLine("IN OPERATOR");
            // Using IN operator
            Guid[] ids = new Guid[] { Guid.NewGuid(), Guid.NewGuid() };
            command = service.Command<Product>()
                .Filter(p => p.Id.In(ids))
                .Select(p => new { p.PartNumber, p.Name })
                .Top(10);
            await ExecuteCommand(command);

            // Using IN operator for enum property DocumentState.
            DocumentState[] states = new DocumentState[] { DocumentState.FirmPlanned, DocumentState.Released };
            command = service.Command<SalesOrder>()
                .Filter(o => o.State.In(states))
                .Select(o => new { o.DocumentNo, o.DocumentId})
                .Top(10);
            await ExecuteCommand(command);

            // Filter by references
            var types = await service.Command<DocumentType>().Filter(dt => dt.EntityName == "Crm_Sales_Orders").Top(3).LoadAsync();
            command = service.Command<SalesOrder>().Filter(o => o.DocumentType.In(types)).Top(5);
            await ExecuteCommand(command);

            // Filter by references using odata id
            EntityIdentifier[] typeIds = new EntityIdentifier[]{
                new EntityIdentifier("General_DocumentTypes", Guid.NewGuid()),
                new EntityIdentifier("General_DocumentTypes", Guid.NewGuid())};
            command = service.Command<SalesOrder>().Filter(o => o.DocumentType.In(typeIds)).Top(5);
            await ExecuteCommand(command);

            Console.WriteLine("Filter by owner properties");
            // SalesOrder is owner of SalesOrderLine.
            command = service.Command<SalesOrderLine>().Filter(l => l.SalesOrder.DocumentDate >= DateTime.Today).Top(10);
            await ExecuteCommand(command);

            Console.WriteLine("Filter by Quantity Value");
            command = service.Command<StoreTransactionLine>()
                .Filter(l => l.QuantityBase.Value >= 5)
                .Select(l=> new { l.LineNo })
                .Top(10);
            await ExecuteCommand(command);

            Console.WriteLine("Filter by Custom Property Value");
            var prop = await service.Command<CustomProperty>().Filter(p => p.EntityName == "Gen_Documents").FirstOrDefaultAsync();
            var propName = "CustomProperty_" + prop.Code;
            command = service.Command<SalesOrder>().Filter(o => o.GetPropertyValue<CustomPropertyValue>(propName) == "PropertyValue").Top(10);
            await ExecuteCommand(command);

            Console.WriteLine("Query $count");
            var date = new DateTime(2020, 09, 20);
            var count = await service.Command<SalesOrder>().Filter(o => o.DocumentDate <= date).CountAsync();
            Console.WriteLine($"count = {count}");

        }

        [Description("Use anonymous types for $select and $expand")]
        static async Task SelectAndExpand(DomainApiService service)
        {
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
            Console.WriteLine(cmd);

            var result = await cmd.LoadAsync();

            foreach (var p in result)
            {
                Console.WriteLine($"{p.Id}, {p.PartNumber}: {p.Name.AnyString}, ProductGroup: {p.ProductGroup.Code}: {p.ProductGroup.Name.AnyString}");
            }

        }

        [Description("Expand sales order reference properties")]
        static async Task ExpandAsync(DomainApiService service)
        {
            // This sample demonstrates the use of typed Expand methods

            var fromDate = DateTime.Today.AddYears(-1);
            var command = service.Command<SalesOrder>()
                .Filter(so => so.DocumentDate >= fromDate)
                .Top(10)
                .Expand(
                    // expand DocumentType and it's DocumentTypeProperties child collection
                    so => so.DocumentType.ExpandCollection(dt => dt.DocumentTypeProperties),
                    // expand Customer and then expand the customer's Party
                    so => so.Customer.Expand(c => c.Party),
                    // expand Lines and then for each line expand Product
                    so => so.Lines.ExpandItems(l => l.Product),
                    // expand another reference of sales order lines - QuantityUnit
                    so => so.Lines.ExpandItems(l => l.QuantityUnit));

            // Check the $expand clause of the command.
            Console.Write("$expand=");
            Console.WriteLine(command.ExpandClause);

            var orders = await command.LoadAsync();



            foreach (var order in orders)
            {
                // Here order.DocumentType is not null because it is included in $expand clause.
                Console.WriteLine($"{order.DocumentType?.TypeName?.AnyString} {order.DocumentNo} - {order.Customer.Party.PartyName.AnyString}");
                // Lines are expanded too
                foreach (var line in order.Lines)
                {
                    Console.WriteLine($"  #{line.LineNo}  {line.Product.Name.AnyString}   {line.Quantity.Value} {line.Quantity.Unit}");
                }
            }

        }


        [Description("Updates a product property")]
        static async Task UpdateProductAsync(DomainApiService service)
        {
            var product = await service.Command<Product>().Filter(p => p.PartNumber.EndsWith("001")).FirstOrDefaultAsync();

            if (product == null)
                throw new Exception("No product found.");

            product.ABCClass = ErpNet.Api.Client.DomainApi.General.Products.ProductsRepository.ABCClass.B;
            await service.UpdateAsync(product);

            // We can refresh the product instance and even expand some properties.
            await service.ReloadAsync(product, p => p.ProductGroup, p => p.BaseMeasurementCategory);

            Console.WriteLine(product.ToJson(true));
        }


        [Description("Work with custom properties.")]
        static async Task CustomPropertiesAsync(DomainApiService service)
        {
            Console.WriteLine("Filter by custom property value");
            
            // Example - filter by custom property value
            var product = await service.Command<Product>()
                .Filter(p => p.GetPropertyValue<CustomPropertyValue>("CustomProperty_0001") == "Value")
                .FirstOrDefaultAsync();

            // Create custom property if not found

            const string propertyCode = "FavoritеProduct";
            var property = await service.Command<CustomProperty>()
                .Filter(p => p.Code == propertyCode)
                .FirstOrDefaultAsync();

            if (property == null)
            {
                property = new CustomProperty()
                {
                    Code = propertyCode,
                    EntityName = Customer.EntityTableName,
                    AllowedValuesEntityName = Product.EntityTableName,
                    LimitToAllowedValues = true,
                    Name = new MultilanguageString("en","Favorite Product")
                };
                await service.InsertAsync(property);
                await service.ResetModelAsync();
            }

            Console.WriteLine($"Set customer's custom property CustomProperty_{propertyCode}");
            var customer = await service.Command<Customer>().FirstOrDefaultAsync();

            // Well set the custom property FavoritеProduct
            // Select the favorite product first.
            var favoriteProduct = await service.Command<Product>().Filter(p => p.PartNumber.Contains("00")).FirstOrDefaultAsync();

            var value = new CustomPropertyValue();
            value.Value = favoriteProduct.PartNumber;
            value.Description = favoriteProduct.Name;
            value.ValueId = favoriteProduct.Id;

            customer.SetPropertyValue($"CustomProperty_{propertyCode}", value);

            await service.UpdateAsync(customer);
        }

        [Description("Creates a sales order in an API transaction.")]
        static async Task CreateSalesOrderAsync(DomainApiService service)
        {
            Console.WriteLine("\r\nUse API transaction and create sales order.\r\n");

            var tr = await service.BeginTransactionAsync(model: DomainModel.FrontEnd);
            try
            {
                // Collect the required references.
                var enterpriseCompanies = await tr.Command<EnterpriseCompany>()
                    .Expand(ec => ec.BaseCurrency) // we'll need the currency sign later
                    .LoadAsync();
                var enterpriseCompany = enterpriseCompanies.First(ec => ec.BaseCurrency?.CurrencySign == "EUR");
                var enterpriseCompanyLocation = await tr.Command<CompanyLocation>()
                    .Filter(l => l.Company == enterpriseCompany.Company)
                    .FirstOrDefaultAsync();

                var company = await tr.Command<Company>()
                    .Filter(c => c.RegistrationNumber == "123456")
                    .FirstOrDefaultAsync();

                // Find or create customer 
                Customer? customer = null;
                if (company != null)
                {
                    // The company exists - so search for customer.
                    customer = await tr.Command<Customer>()
                        .Filter(c => c.Party == company && c.EnterpriseCompany == enterpriseCompany)
                        .FirstOrDefaultAsync();
                }
                else
                {
                    // Create company because it is not found
                    company = new Company()
                    {
                        Name = new MultilanguageString { CurrentString = "Test Company" },
                        RegistrationType = new MultilanguageString { CurrentString = "Ltd." },
                        RegistrationNumber = "123456"
                    };
                }

                if (customer == null)
                {
                    // Customer is not found - create new.
                    customer = new Customer()
                    {
                        Party = company,
                        EnterpriseCompany = enterpriseCompany
                    };
                }
                // Create sales order.
                var order = new SalesOrder();
                order.EnterpriseCompany = enterpriseCompany;
                order.EnterpriseCompanyLocation = enterpriseCompanyLocation;
                order.DocumentType = await tr.Command<DocumentType>()
                    .Filter(dt => dt.EntityName == "Crm_Sales_Orders")
                    .FirstOrDefaultAsync();
                order.Customer = customer;
                order.DocumentCurrency = enterpriseCompany.BaseCurrency;

                // Add sales order lines.
                var products = await tr.Command<Product>().Expand(p => p.MeasurementUnit).Top(10).LoadAsync();
                List<SalesOrderLine> newLines = new List<SalesOrderLine>();
                foreach (var p in products)
                {
                    newLines.Add(new SalesOrderLine()
                    {
                        Product = p,
                        Quantity = new Quantity { Value = 10, Unit = p.MeasurementUnit?.Code },
                        UnitPrice = new Amount { Value = 1.25m, Currency = order.DocumentCurrency?.CurrencySign }
                    });
                }
                order.Lines = newLines;

                Console.WriteLine(order.GetRawChanges()?.ToJson(true));


                Console.WriteLine("Inserting...");

                await tr.InsertAsync(order);

                Console.WriteLine(order);
            }
            finally
            {
                // Pass commit=false to reject the changes
                await tr.EndTransactionAsync(commit: false);
            }
        }

        [Description("Changes a document state")]
        static async Task ChangeDocumentStateAsync(DomainApiService service)
        {
            var order = await service.Command<SalesOrder>()
                .Filter(so => so.State == DocumentState.Released)
                .Expand(so => so.DocumentType.ExpandCollection(dt => dt.UserStatuses))
                .FirstOrDefaultAsync();

            if (order == null)
                throw new Exception("No sales order found.");

            var userStatus = order.DocumentType?.UserStatuses?.FirstOrDefault(us => us.State == DocumentState.Released);

            await order.ChangeStateAsync(service, DocumentState.Released, userStatus);
        }

        [Description("Get calculated attribute value")]
        static async Task GetCalculatedAttributeValue(DomainApiService service)
        {
            var command = service.Command<SalesOrder>();
            command.SelectClause = "DEFAULT, CalculatedAttribute_MyAttribute1";

            var order = await command.FirstOrDefaultAsync()
                    ?? throw new Exception("No sales order found.");

            var calculatedAttributeValue = order.GetPropertyValue("CalculatedAttribute_ForTest");

            Debug.WriteLine(@$"{nameof(SalesOrder)}({order.Id}):
CalculatedAttribute_ForTest = {calculatedAttributeValue}");
        }
    }
}
