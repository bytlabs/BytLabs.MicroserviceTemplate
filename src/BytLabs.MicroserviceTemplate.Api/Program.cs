using BytLabs.Api.Graphql;
using BytLabs.Api;
using BytLabs.MicroserviceTemplate.Infrastructure;
using BytLabs.MicroserviceTemplate.Api.Graphql.Mutations;
using BytLabs.MicroserviceTemplate.Api.Graphql.Queries.Mongo;
using BytLabs.MicroserviceTemplate.Api.Utils;
using BytLabs.MicroserviceTemplate.Api.Extensions;
using BytLabs.MicroserviceTemplate.Api.ConsoleApp;
using BytLabs.Api.UserContextResolvers;
using BytLabs.Api.TenantProvider;
using Microsoft.AspNetCore.WebSockets;
using BytLabs.MicroserviceTemplate.Infrastructure.HotChocolate;
using BytLabs.MicroserviceTemplate.Api.OData;
using BytLabs.MicroserviceTemplate.Api.Graphql.Queries.Ef;
using Microsoft.AspNetCore.OData;

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

                // The read resolvers + query middleware are store-selected, but they register the SAME
                // schema types (commands/DTOs/aggregate filter+sort), so the schema is identical on both
                // stores and one client works against either.
                var isPostgres = string.Equals(
                    builder.Configuration["DataStore:Provider"], "Postgres", StringComparison.OrdinalIgnoreCase);

                var graphql = services.AddGraphQLService();
                if (isPostgres)
                    graphql.AddQueryableQuerySettings().AddQueryType<EfQuery>();
                else
                    graphql.AddMongoDbQuerySettings().AddQueryType<Query>();

                graphql
                    .AddDynamicDataTypes()
                    .AddCommandTypes()
                    .AddDtoTypes()
                    .AddAggregateTypes()
                    .AddMutationType<Mutation>()
                    .ModifyCostOptions(o => o.EnforceCostLimits = false)
                    .ModifyOptions(o => o.RemoveUnreachableTypes = true)
                    .ModifyPagingOptions(opt => opt.IncludeTotalCount = true);

                // REST/OData surface (same store as GraphQL, selected by DataStore:Provider).
                services.AddControllers().AddOData(options =>
                {
                    // Bind/serialize date-times in UTC (don't convert 'Z' inputs to the server's local
                    // zone), so OrderDate round-trips identically on Mongo and Postgres.
                    options.TimeZone = TimeZoneInfo.Utc;
                    options.Select().Filter().OrderBy().Expand().Count().SetMaxTop(100)
                        .AddRouteComponents("odata", EdmModel.GetEdmModel());
                });
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
        app.MapControllers();
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
