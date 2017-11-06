using System;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.Documents.Client;
using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Demos.FileSharingService.API.Models;
using Microsoft.Demos.FileSharingService.API.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Microsoft.Demos.FileSharingService.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the built in DI container.
        public void ConfigureServices(IServiceCollection services)
        {
            var storageAccountKeyVaultSecret = this.GetSecretFromKeyVault(Configuration["StorageAccount:KeyVaultSecretUri"]);
            var cosmosDBKeyVaultSecret = this.GetSecretFromKeyVault(Configuration["CosmosDBAccount:KeyVaultSecretUri"]);
            storageAccountKeyVaultSecret.Wait();
            cosmosDBKeyVaultSecret.Wait();
            
            var storageAccountConnectionString = string.Format(Configuration["StorageAccount:ConnectionString"], storageAccountKeyVaultSecret.Result);            

            var cosmosDbAccountKey = cosmosDBKeyVaultSecret.Result;
            var cosmosDBUri = Configuration["CosmosDBAccount:ConnectionString"];            
            var cosmosDbDatabaseId = Configuration["CosmosDBAccount:DatabaseId"];
            var cosmosDbCollectionId = Configuration["CosmosDBAccount:CollectionId"];

            services.AddMvc();
            services.AddSingleton<IFileMapRepository>(new CosmosDBFileMapRepository(new DocumentClient(new Uri(cosmosDBUri), cosmosDbAccountKey), cosmosDbDatabaseId, cosmosDbCollectionId));            

            // You can move some of the parameters injected here like token TTL to config as well.
            services.AddTransient<ITokenProvider>((IServiceProvider svc) => (
                    new StorageAccountSASTokenProvider(
                        storageAccountConnectionString,
                        new FileAccessPolicy { Permissions = FilePermissions.Read, TTL = new TimeSpan(0, 2, 0), Protocol = FileAccessProtocol.HttpsOnly, UseSourceIPRestriction = true },
                        new FileAccessPolicy { Permissions = FilePermissions.Delete, TTL = new TimeSpan(0, 2, 0), Protocol = FileAccessProtocol.HttpsOnly, UseSourceIPRestriction = true },
                        new FileAccessPolicy { Permissions = FilePermissions.Read | FilePermissions.Delete, TTL = new TimeSpan(0, 2, 0), Protocol = FileAccessProtocol.HttpsOnly, UseSourceIPRestriction = true }
                        )));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            //if (env.IsDevelopment())
            //{
                app.UseDeveloperExceptionPage();
            //}

            // Authenticate requests here and inject the authenticated user name below instead of suneetnangia@yxz.com.
            // app.UseAuthentication();

            app.Use(async (context, next) =>
            {
                // Clear all existing query string parameters for security reasons 
                // and inject remote Ip address and authenticated username as a query string parameter for the controller methods.

                context.Request.QueryString = new QueryString();
                context.Request.QueryString = context.Request.QueryString.Add("remoteIPAddress", context.Request.HttpContext.Connection.RemoteIpAddress.ToString());
                context.Request.QueryString = context.Request.QueryString.Add("authenticatedUsername", "suneetnangia@yxz.com");

                // Do work that doesn't write to the Response.                
                await next.Invoke();
                // Do logging or other work that doesn't write to the Response.
            });

            app.UseMvc();
        }

        private async Task<string> GetSecretFromKeyVault(string keyVaultSecretUri)
        {
            var azureServiceTokenProvider = new AzureServiceTokenProvider();
            var keyVaultClient = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(azureServiceTokenProvider.KeyVaultTokenCallback));

            var secret = await keyVaultClient.GetSecretAsync(keyVaultSecretUri).ConfigureAwait(false);

            return secret.Value;
        }
    }
}