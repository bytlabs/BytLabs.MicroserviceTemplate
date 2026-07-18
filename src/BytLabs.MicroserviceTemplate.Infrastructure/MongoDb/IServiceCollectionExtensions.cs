using BytLabs.Api.Configuration;
using BytLabs.DataAccess.MongoDB;
using BytLabs.DataAccess.MongoDB.Configuration;
using BytLabs.DataAccess.MongoDB.Serializers;
using BytLabs.MicroserviceTemplate.Domain.EntityDefs.Aggregates;
using BytLabs.MicroserviceTemplate.Domain.Orders.Aggregates;
using BytLabs.MicroserviceTemplate.Domain.Orders.Entities;
using BytLabs.MicroserviceTemplate.Domain.Products.Aggregates;
using BytLabs.MicroserviceTemplate.Domain.Products.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BytLabs.MicroserviceTemplate.Infrastructure.MongoDb
{
    internal static class IServiceCollectionExtensions
    {
        /// <summary>MongoDB store: repositories + a read-side <see cref="IQueryable{T}"/> per aggregate.</summary>
        internal static IServiceCollection AddMongoStore(this IServiceCollection services, ConfigurationManager configuration)
        {
            var mongoDatabaseConfiguration = configuration.GetConfiguration<MongoDatabaseConfiguration>();
            // AddMongoRepository also registers a read-side IQueryable<T> over the tenant's collection,
            // used by REST/OData and the queryable GraphQL path. (The Mongo GraphQL recipe keeps using
            // IMongoDatabase directly.) Soft-deleted rows are excluded at the read site via
            // ExcludeSoftDeletedEntities, mirroring EF's global query filter.
            services.AddMongoDatabase(mongoDatabaseConfiguration)
                .RegisterMongoDBClassMaps()
                .AddMongoRepository<Order, Guid>()
                .AddMongoRepository<Product, Guid>()
                .AddMongoRepository<EntityDef, Guid>();

            return services;
        }

        private static IServiceCollection RegisterMongoDBClassMaps(this IServiceCollection services)
        {
            // Custom serializer so Product.Variants (IReadOnlySet<ProductVariant>) round-trips.
            BsonSerializer.TryRegisterSerializer(new IReadOnlySetSerializer<ProductVariant>());
            BsonSerializer.TryRegisterSerializer(new IReadOnlySetSerializer<OrderItem>());

            // Pin Mongo to Order's parameterized creator. Order also has a parameter-less ctor for EF
            // (child-table Items); without this pin Mongo could pick the wrong creator on deserialization.
            BsonClassMap.TryRegisterClassMap<Order>(cm =>
            {
                cm.AutoMap();
                cm.MapCreator(o => new Order(o.Id, o.OrderDate, o.Items, o.Data));
            });

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

            BsonClassMap.TryRegisterClassMap<ProductVariant>(cm =>
            {
                cm.AutoMap();
            });

            return services;
        }
    }
}
