using BytLabs.Api.Configuration;
using BytLabs.DataAccess.MongoDB.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using BytLabs.DataAccess.MongoDB;
using BytLabs.MicroserviceTemplate.Domain.Aggregates.OrderAggregate;
using BytLabs.MicroserviceTemplate.Domain.Aggregates.ProductAggregate;
using BytLabs.MicroserviceTemplate.Domain.Aggregates.EntityDefAggregate;
using BytLabs.Application;
using BytLabs.MicroserviceTemplate.Application.Commands.CreateOrder;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Bson;
using BytLabs.MicroserviceTemplate.Application.MappingProfiles;
using BytLabs.MicroserviceTemplate.Infrastructure.Services;
using BytLabs.MicroserviceTemplate.Application.Services;
using BytLabs.MicroserviceTemplate.Infrastructure.Shared.DynamicData;
using BytLabs.MicroserviceTemplate.Infrastructure.Utils.Serializers;

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
        services.AddCQS(new System.Reflection.Assembly[] { typeof(CreateOrderCommand).Assembly });
        services.AddAutoMapper(typeof(OrderMappingProfile), typeof(ProductMappingProfile), typeof(EntityDefMappingProfile));

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
