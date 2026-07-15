using BytLabs.Api.Configuration;
using BytLabs.DataAccess.MongoDB.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using BytLabs.DataAccess.MongoDB;
using BytLabs.Application;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Bson;
using BytLabs.MicroserviceTemplate.Infrastructure.MongoDB;
using BytLabs.MicroserviceTemplate.Application.Orders.Commands.CreateOrder;
using BytLabs.MicroserviceTemplate.Application.Common.Services;
using BytLabs.MicroserviceTemplate.Application.Products.Mapping;
using BytLabs.MicroserviceTemplate.Application.Orders.Mapping;
using BytLabs.MicroserviceTemplate.Application.EntityDefs.Mapping;
using BytLabs.MicroserviceTemplate.Domain.EntityDefs.Aggregates;
using BytLabs.MicroserviceTemplate.Domain.Products.Aggregates;
using BytLabs.MicroserviceTemplate.Domain.Products.Entities;
using BytLabs.MicroserviceTemplate.Domain.Orders.Aggregates;
using BytLabs.MicroserviceTemplate.Domain.Orders.Entities;
using BytLabs.MicroserviceTemplate.Domain.Orders.ValueObjects;
using BytLabs.MicroserviceTemplate.Infrastructure.Common.Services;

namespace BytLabs.MicroserviceTemplate.Infrastructure;

public static class ServiceExtensions
{
    /// <summary>
    /// Adds infrastructure related services to the provided service collection.
    /// </summary>
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, ConfigurationManager configuration)
    {
        if (services == null) throw new ArgumentNullException(nameof(services));
        if (configuration == null) throw new ArgumentNullException(nameof(configuration));

        // Setup Application (CQS scans the whole Application assembly, so Product commands are included)
        services.AddCQS([typeof(CreateOrderCommand).Assembly]);
        services.AddAutoMapper(
            typeof(OrderMappingProfile), 
            typeof(ProductMappingProfile), 
            typeof(EntityDefMappingProfile));

        // Setup Database
        var mongoDatabaseConfiguration = configuration.GetConfiguration<MongoDatabaseConfiguration>();
        services.AddMongoDatabase(mongoDatabaseConfiguration)
            .RegisterMongoDBClassMaps()
            .RegisterDynamicDataClassMaps()
            .AddMongoRepository<Order, Guid>()
            .AddMongoRepository<Product, Guid>()
            .AddMongoRepository<EntityDef, Guid>();

        // Setup services
        services.AddSingleton<IEmailService, MyCustomEmailService>();

        return services;
    }

    private static IServiceCollection RegisterMongoDBClassMaps(this IServiceCollection services)
    {
        // Custom serializer so Product.Variants (IReadOnlySet<ProductVariant>) round-trips.
        BsonSerializer.TryRegisterSerializer(new IReadOnlySetSerializer<ProductVariant>());
        BsonSerializer.TryRegisterSerializer(new IReadOnlySetSerializer<OrderItem>());

        BsonClassMap.TryRegisterClassMap<OrderItem>(cm =>
        {
            cm.AutoMap();
            cm.MapMember(c => c.ProductId)
                .SetSerializer(new GuidSerializer(BsonType.String));
        });

        BsonClassMap.TryRegisterClassMap<ProductVariant>(cm =>
        {
            cm.AutoMap();
        });

        return services;
    }
}
