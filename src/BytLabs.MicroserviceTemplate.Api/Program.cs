using BytLabs.Api.Graphql;
using Microsoft.AspNetCore.WebSockets;
using BytLabs.Api;
using BytLabs.MicroserviceTemplate.Infrastructure;
using BytLabs.MicroserviceTemplate.Api.Graphql.Mutations;
using BytLabs.MicroserviceTemplate.Api.Graphql.Queries;
using BytLabs.Api.UserContextResolvers;
using BytLabs.Api.TenantProvider;
using Serilog;


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

                services
                    .AddWebSockets(op => op.KeepAliveInterval = TimeSpan.FromSeconds(30));

                services.AddGraphQLService()
                    .AddCommandTypes()
                    .AddAggregateTypes()
                    .AddMutationType<Mutation>()
                    .AddQueryType<Query>();

            });

    WebApplication app = webAppBuilder.BuildWebApp(app =>
    {
        app.UseWebSockets();
        app.MapGraphQL();
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