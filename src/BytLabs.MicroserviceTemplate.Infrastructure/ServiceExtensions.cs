using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using BytLabs.Application;
using BytLabs.MicroserviceTemplate.Infrastructure.MongoDb;
using BytLabs.MicroserviceTemplate.Infrastructure.Postgres;
using BytLabs.MicroserviceTemplate.Application.Orders.Commands.CreateOrder;
using BytLabs.MicroserviceTemplate.Application.Common.Services;
using BytLabs.MicroserviceTemplate.Application.Products.Mapping;
using BytLabs.MicroserviceTemplate.Application.Orders.Mapping;
using BytLabs.MicroserviceTemplate.Application.EntityDefs.Mapping;
using BytLabs.MicroserviceTemplate.Infrastructure.Common.Services;

namespace BytLabs.MicroserviceTemplate.Infrastructure;

public static class ServiceExtensions
{
    /// <summary>
    /// Adds infrastructure services. The persistence store is selected by
    /// <c>DataStore:Provider</c> in configuration ("Mongo" [default] or "Postgres").
    /// Both transports (GraphQL + REST) run on whichever store is wired.
    /// </summary>
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, ConfigurationManager configuration)
    {
        if (services == null) throw new ArgumentNullException(nameof(services));
        if (configuration == null) throw new ArgumentNullException(nameof(configuration));

        services.AddCQS([typeof(CreateOrderCommand).Assembly]);
        services.AddAutoMapper(
            typeof(OrderMappingProfile),
            typeof(ProductMappingProfile),
            typeof(EntityDefMappingProfile));
        services.AddSingleton<IEmailService, MyCustomEmailService>();

        var store = configuration.GetSection("DataStore").Get<DataStoreConfiguration>() ?? new DataStoreConfiguration();
        if (string.Equals(store.Provider, "Postgres", StringComparison.OrdinalIgnoreCase))
            services.AddPostgresStore(configuration);
        else
            services.AddMongoStore(configuration);

        return services;
    }
   
}
