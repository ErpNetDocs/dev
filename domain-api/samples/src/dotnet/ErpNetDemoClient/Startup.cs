using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;

namespace ErpNetDemoClient
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        /// <summary>
        /// The url of the ERP.net instance.
        /// </summary>
        public const string ErpNetInstanceUrl = "https://demodb.my.erp.net";

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.TryAddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddHttpClient();

            services.AddRazorPages();

            

            string identityServerUrl = $"{ErpNetInstanceUrl}/id";
            string applicationUri = "ErpNetDemoClient";
            string applicationSecret = "DEMO";

            services.AddAuthentication(options =>
            {
                options.DefaultScheme = "Cookies";
                options.DefaultChallengeScheme = "erpnet";
            })
           .AddCookie("Cookies")
           .AddOpenIdConnect("erpnet", options =>
           {
               options.Authority = identityServerUrl;
               
               options.ClientId = applicationUri;
               options.ClientSecret = applicationSecret;

               options.ResponseType = "code id_token";

               options.SaveTokens = true;
               options.GetClaimsFromUserInfoEndpoint = true;

               options.Scope.Add("openid");
               options.Scope.Add("profile");
               options.Scope.Add("offline_access");

               options.Events = new Microsoft.AspNetCore.Authentication.OpenIdConnect.OpenIdConnectEvents
               {
                   OnUserInformationReceived = context =>
                   {
                       string rawAccessToken = context.ProtocolMessage.AccessToken;
                       string rawIdToken = context.ProtocolMessage.IdToken;
                       var handler = new JwtSecurityTokenHandler();
                       var accessToken = handler.ReadJwtToken(rawAccessToken);
                       var idToken = handler.ReadJwtToken(rawIdToken);

                       // do something with the JWTs

                       System.Diagnostics.Debug.Print($"access-token: {rawAccessToken}, id-token: {rawIdToken}");

                       return Task.CompletedTask;
                   },
                   // called if user clicks Cancel during login
                   OnAccessDenied = context =>
                   {
                       context.HandleResponse();
                       context.Response.Redirect("/");
                       return Task.CompletedTask;
                   },
                   OnSignedOutCallbackRedirect = context =>
                   {
                       context.HandleResponse();
                       context.Response.Redirect("/");
                       return Task.CompletedTask;
                   }
               };
           });

        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapRazorPages();

                // Register the /Login and /Logout endpoints.

                endpoints.MapGet("Login", async (ctx) =>
                {
                    var authProps = new AuthenticationProperties
                    {
                        IsPersistent = true,
                        ExpiresUtc = null,
                        RedirectUri = $"{ctx.Request.Scheme}://{ctx.Request.Host}"
                    };
                    await ctx.ChallengeAsync("erpnet", authProps);
                });

                endpoints.MapGet("Logout", async (ctx) =>
                {
                    var authProps = new AuthenticationProperties
                    {
                        RedirectUri = $"{ctx.Request.Scheme}://{ctx.Request.Host}"
                    };

                    // Sign out from ERP.net identity server and remove authentication cookies.
                    await ctx.SignOutAsync("erpnet", authProps);
                    await ctx.SignOutAsync("Cookies", authProps);
                    
                });
            });
        }
    }

}
