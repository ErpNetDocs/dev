using IdentityModel.Client;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace ErpNetServiceClient
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            DoWork().Wait();
          
        }

        static async Task DoWork()
        {
            // Store access token locally because each access token request opens a license session on the application server.
            string accessToken;
            var accessTokenFile = "access_token.txt";

            if (File.Exists(accessTokenFile))
            {
                accessToken = File.ReadAllText(accessTokenFile);
                // If the access token is expired, request a new one.
                JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
                var at = handler.ReadJwtToken(accessToken);
                if (at.ValidTo < DateTime.UtcNow)
                    accessToken = (await RequestServiceTokenAsync()).AccessToken;
            }
            else
            {
                accessToken = (await RequestServiceTokenAsync()).AccessToken;
            }

            File.WriteAllText(accessTokenFile, accessToken);
            
            using var httpClient = new HttpClient();

            httpClient.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

            var response = await httpClient.GetAsync("https://demodb.my.erp.net/api/domain/odata/General_Products_Products?$top=10");
            var productsJson = await response.Content.ReadAsStringAsync();

            //TODO: Examine the json result. 
            // If an unauthorized error is returned, probably the access token is expired, so we need to request new access token.

            Console.WriteLine(productsJson);
        }

        static async Task<TokenResponse> RequestServiceTokenAsync()
        {
            HttpClientHandler handler = new HttpClientHandler();
            handler.ServerCertificateCustomValidationCallback = (a, b, c, d) => true;
            using var httpClient = new HttpClient(handler);

            var disco = await httpClient.GetDiscoveryDocumentAsync("https://demodb.my.erp.net/id");
            if (disco.IsError) 
                throw new Exception(disco.Error);
            
            var response = await httpClient.RequestClientCredentialsTokenAsync(new ClientCredentialsTokenRequest
            {
                Address = disco.TokenEndpoint,
                ClientId = "ServiceDemoClient",
                ClientSecret = "DEMO"
            });

            if (response.IsError) 
                throw new Exception(response.Error);
            return response;
        }


    }
}
