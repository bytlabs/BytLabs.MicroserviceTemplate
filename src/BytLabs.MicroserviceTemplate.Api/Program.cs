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
using BytLabs.MicroserviceTemplate.Infrastructure.HotChocolate;

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
        // Serve the bundled console SPA (Vite build) from wwwroot/console.
        // UseDefaultFiles maps /console/ -> /console/index.html; UseStaticFiles serves the hashed
        // /console/assets/* files.
        app.UseDefaultFiles();
        app.UseStaticFiles();

        app.UseAuthentication();
        app.UseAuthorization();
        app.UseWebSockets();
        app.MapGraphQL();
        app.MapConsoleEndpoints();

        // SPA fallback: client-side routes (e.g. /console/entities/Product) resolve to the console
        // shell. `:nonfile` excludes asset requests so a missing asset 404s instead of returning HTML.
        // The bare /console (no trailing slash, which UseDefaultFiles doesn't rewrite) also serves the
        // shell. (/console/ is served as index.html by UseDefaultFiles.)
        app.MapFallbackToFile("/console/{*path:nonfile}", "console/index.html");
        app.MapFallbackToFile("/console", "console/index.html");
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
