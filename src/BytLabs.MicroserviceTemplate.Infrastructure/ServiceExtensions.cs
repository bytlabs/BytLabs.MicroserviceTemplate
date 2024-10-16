using BytLabs.Api.Configuration;
using BytLabs.DataAccess.MongDB.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using BytLabs.DataAccess.MongDB;
using BytLabs.MicroserviceTemplate.Domain.Aggregates.OrderAggregate;
using BytLabs.Application;
using BytLabs.MicroserviceTemplate.Application.Commands.CreateOrder;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Bson;

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


        //Setup Application
        services.AddCQS(new System.Reflection.Assembly[] { typeof(CreateOrderCommand).Assembly });


        //Setup Database
        var mongoDatabaseConfiguration = configuration.GetConfiguration<MongoDatabaseConfiguration>();

        services.AddMongoDatabase(mongoDatabaseConfiguration)
            .RegisterMongoDBClassMaps()
            .AddMongoRepository<Order, Guid>();

        return services;
    }

    private static IServiceCollection RegisterMongoDBClassMaps(this IServiceCollection services)
    {
        BsonClassMap.TryRegisterClassMap<OrderItem>(cm =>
        {
            cm.AutoMap();
            cm.MapMember(c => c.ProductId)
                .SetSerializer(new GuidSerializer(BsonType.String));
        });

        return services;
    }

}

