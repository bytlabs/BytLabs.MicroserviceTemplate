using BytLabs.MicroserviceTemplate.Client;
using BytLabs.MicroserviceTemplate.Client.Rest;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.Extensions.DependencyInjection;

namespace BytLabs.MicroserviceTemplate.Tests.Accpetance.Support;

/// <summary>
/// Boots the API in-memory against a chosen store (<c>DataStore:Provider</c> = Mongo or Postgres) and
/// exposes both typed clients — the StrawberryShake GraphQL client and the Microsoft.OData.Client REST
/// client — wired to the in-memory test server. Every request carries the <c>Tenant: Test</c> header.
/// </summary>
public class MatrixApiFactory : WebApplicationFactory<Program>
{
    public const string TenantId = "Test";
    private readonly string _provider;

    public MatrixApiFactory(string provider) => _provider = provider;

    public string Provider => _provider;

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseSetting("DataStore:Provider", _provider);

        builder.ConfigureTestServices(services =>
        {
            // Authenticate every request as a fixed test user so [Authorize] resolvers are reachable.
            services.AddAuthentication(TestAuthHandler.SchemeName)
                .AddScheme<AuthenticationSchemeOptions, TestAuthHandler>(TestAuthHandler.SchemeName, _ => { });
        });

        builder.ConfigureServices(services =>
        {
            services.AddSingleton<IHttpClientFactory>(_ => new CustomHttpClientFactory(() =>
            {
                var http = CreateClient(new WebApplicationFactoryClientOptions
                {
                    BaseAddress = new("http://localhost/graphql")
                });
                http.DefaultRequestHeaders.Add("Tenant", TenantId);
                return http;
            }));
            services.AddMicroserviceTemplateGraphQLClient();
        });
    }

    /// <summary>The StrawberryShake GraphQL client bound to the in-memory server.</summary>
    public IMicroserviceTemplateGraphQLClient GraphQLClient
        => Services.GetService<IMicroserviceTemplateGraphQLClient>()
           ?? throw new InvalidOperationException("Missing IMicroserviceTemplateGraphQLClient configuration.");

    /// <summary>A fresh Microsoft.OData.Client context bound to the in-memory server.</summary>
    public RestServiceContext CreateODataClient()
        => new(new Uri("http://localhost/odata/"), CreateTenantClient());

    /// <summary>A raw HttpClient carrying the tenant header (for OData bound actions / metadata).</summary>
    public HttpClient CreateTenantClient()
    {
        var http = CreateClient();
        http.DefaultRequestHeaders.Add("Tenant", TenantId);
        return http;
    }
}
