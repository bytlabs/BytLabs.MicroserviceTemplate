using BytLabs.MicroserviceTemplate.Client;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;

namespace BytLabs.MicroserviceTemplate.Tests.Accpetance.Support;

public class CustomWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            // Authenticate every request in tests so [Authorize] resolvers are reachable.
            services.AddAuthentication(TestAuthHandler.SchemeName)
                    .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(TestAuthHandler.SchemeName, _ => { });
        });

        builder.ConfigureServices(serviceCollection =>
        {
            serviceCollection.AddSingleton<IHttpClientFactory>(_ => new CustomHttpClientFactory(() =>
            {
                HttpClient httpClient = CreateClient(new WebApplicationFactoryClientOptions
                {
                    BaseAddress = new("http://localhost/graphql")
                });
                httpClient.DefaultRequestHeaders.Add("Tenant", "Test");
                return httpClient;
            }));

            serviceCollection.AddMicroserviceTemplateGraphQLClient();
        });
    }

    public IMicroserviceTemplateGraphQLClient GetGraphQLClient()
        => Services.GetService<IMicroserviceTemplateGraphQLClient>()
           ?? throw new InvalidOperationException("Missing IMicroserviceTemplateGraphQLClient configuration.");
}
