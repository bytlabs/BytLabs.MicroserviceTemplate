using BytLabs.MicroserviceTemplate.Client;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using TechTalk.SpecFlow.Infrastructure;

namespace BytLabs.MicroserviceTemplate.Tests.Accpetance.Support;

public class CustomWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
{
    private readonly ISpecFlowOutputHelper? _outputHelper;

    public CustomWebApplicationFactory()
    {

    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureServices(serviceCollection =>
        {

            serviceCollection.AddSingleton<IHttpClientFactory>(services => new CustomHttpClientFactory(() =>
            {
                HttpClient httpClient = CreateClient(new WebApplicationFactoryClientOptions
                {
                    BaseAddress = new("http://localhost/graphql")
                });
                httpClient.DefaultRequestHeaders.Add("Tenant", "Test");
                return httpClient;
            }));

            serviceCollection
            .AddMicroserviceTemplateGraphQLClient();
        });
    }

    public IMicroserviceTemplateGraphQLClient GetGraphQLClient()
    {
        return Services.GetService<IMicroserviceTemplateGraphQLClient>()
            ?? throw new NotImplementedException("Missing service configuration for IMicroserviceTemplateGraphQLClient.");
    }
}

