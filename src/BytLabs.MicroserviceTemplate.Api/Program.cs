using BytLabs.Api.Graphql;
using BytLabs.Api;
using BytLabs.MicroserviceTemplate.Infrastructure;
using BytLabs.MicroserviceTemplate.Api.Graphql.Mutations;
using BytLabs.MicroserviceTemplate.Api.Graphql.Queries;
using BytLabs.MicroserviceTemplate.Api.Utils;
using BytLabs.MicroserviceTemplate.Api.Extensions;
using BytLabs.MicroserviceTemplate.Api.ConsoleApp;
using BytLabs.Api.UserContextResolvers;
using BytLabs.Api.TenantProvider;
using Microsoft.AspNetCore.WebSockets;

try
{
    var builder = WebApplication.CreateBuilder(args);
    var webAppBuilder = ApiServiceBuilder.CreateBuilder(builder)
            .WithHttpContextAccessor(userContextBuilder =>
            {
                userContextBuilder.AddResolver<HttpUserContextResolver>();
            })
            .WithMultiTenantContext(multitenancyBuilder =>
            {
                multitenancyBuilder.AddResolver<FromHeaderTenantIdResolver>();
            })
            .WithLogging()
            .WithMetrics()
            .WithTracing()
            .WithHealthChecks()
            .WithServiceConfiguration(services =>
            {
                services.AddInfrastructure(builder.Configuration);

                services.AddJwtAuthentication(builder.Configuration);

                services.AddWebSockets(op => op.KeepAliveInterval = TimeSpan.FromSeconds(30));

                services.AddGraphQLService()
                    .AddMongoDbQuerySettings()
                    .AddDynamicDataTypes()
                    .AddCommandTypes()
                    .AddDtoTypes()
                    .AddAggregateTypes()
                    .AddMutationType<Mutation>()
                    .AddQueryType<Query>()
                    .ModifyCostOptions(o => o.EnforceCostLimits = false)
                    .ModifyOptions(o => o.RemoveUnreachableTypes = true)
                    .ModifyPagingOptions(opt => opt.IncludeTotalCount = true);
            });

    WebApplication app = webAppBuilder.BuildWebApp(app =>
    {
        // Serve the bundled static console (Next.js export) from wwwroot/console.
        app.UseDefaultFiles();
        app.UseStaticFiles();

        app.UseAuthentication();
        app.UseAuthorization();
        app.UseWebSockets();
        app.MapGraphQL();
        app.MapConsoleEndpoints();

        // SPA fallback so client-side console routes resolve to the exported shell.
        app.MapFallbackToFile("/console/{*path:nonfile}", "console/index.html");
    });

    app.Run();
}
catch (Exception ex)
{
    Console.WriteLine($"Application failed to start: {ex.Message}");
    throw;
}

//For acceptance tests reference
public partial class Program { }
