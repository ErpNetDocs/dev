using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using ErpNet.Api.Client.DomainApi;
using ErpNet.Api.Client.DomainApi.General.Products;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace ErpNetDemoClient.Pages
{
    public class ProductsModel : PageModel
    {
        private readonly ILogger<ProductsModel> _logger;

        private IEnumerable<Product> products;

        public ProductsModel(ILogger<ProductsModel> logger)
        {
            _logger = logger;
        }

        public IEnumerable<Product> Products => products;

        public async Task OnGet()
        {
            // Use the access token of the logged user to obtain resources from Domain API
            DomainApiService service = new DomainApiService(
                $"{Startup.ErpNetInstanceUrl}/api/domain/odata/",
                () => HttpContext.GetTokenAsync("access_token")
                );

            // We know the ID of the root product group.
            var rootProductGroup = (await service.Command<ProductGroup>()
                .Id(new Guid("dc92f2e4-19f9-40eb-b257-565182f09f08"))
                .LoadAsync()).First();

            // Load child groups.
            var groups = await service.Command<ProductGroup>()
                .Filter(pg => pg.FullPath.StartsWith(rootProductGroup.FullPath))
                .LoadAsync();
            
            // Load products whithin these child groups.
            products = await service.Command<Product>()
                 .Top(20)
                 .Filter(p => p.ProductGroup.In(groups))
                 .Expand(p => p.Pictures)
                 .LoadAsync();
        }
    }
}
