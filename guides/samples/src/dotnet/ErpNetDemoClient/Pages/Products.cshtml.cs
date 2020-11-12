using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Logging;

namespace ErpNetDemoClient.Pages
{
    public class ProductsModel : PageModel
    {
        private readonly ILogger<ProductsModel> _logger;
        private readonly IHttpClientFactory _clientFactory;

        private string productsJson;

        public ProductsModel(ILogger<ProductsModel> logger,  IHttpClientFactory clientFactory)
        {
            _logger = logger;
            _clientFactory = clientFactory;
        }

        public string ProductsJson => productsJson;

        public async Task OnGet()
        {
            // Use the access token of the logged user to obtain resources from Domain API
            var accessToken = await HttpContext.GetTokenAsync("access_token");

            var httpClient = _clientFactory.CreateClient("DomainApi");

            // Set the Authorization header to 'Bearer {access_token}'
            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

            // Load products
            var response = await httpClient.GetAsync($"{Startup.ErpNetInstanceUrl}/api/domain/odata/General_Products_Products?$top=10");
            productsJson = await response.Content.ReadAsStringAsync();

        }
    }
}
