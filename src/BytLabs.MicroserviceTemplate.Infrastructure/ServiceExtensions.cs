using BytLabs.Api.Configuration;
using BytLabs.DataAccess.MongoDB.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using BytLabs.DataAccess.MongoDB;
using BytLabs.DataAccess.MongoDB.Extensions;
using BytLabs.DataAccess.EntityFramework;
using BytLabs.DataAccess.EntityFramework.Configuration;
using BytLabs.Application;
using BytLabs.Application.DataAccess;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Microsoft.EntityFrameworkCore;
using BytLabs.MicroserviceTemplate.Infrastructure.MongoDB;
using BytLabs.MicroserviceTemplate.Infrastructure.Postgres;
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
    /// Adds infrastructure services. The persistence store is selected by
    /// <c>DataStore:Provider</c> in configuration ("Mongo" [default] or "Postgres").
    /// Both transports (GraphQL + REST) run on whichever store is wired.
    /// </summary>
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, ConfigurationManager configuration)
    {
        if (services == null) throw new ArgumentNullException(nameof(services));
        if (configuration == null) throw new ArgumentNullException(nameof(configuration));

        services.AddApplicationCore();

        var store = configuration.GetSection("DataStore").Get<DataStoreConfiguration>() ?? new DataStoreConfiguration();
        if (string.Equals(store.Provider, "Postgres", StringComparison.OrdinalIgnoreCase))
            services.AddPostgresStore(configuration);
        else
            services.AddMongoStore(configuration);

        return services;
    }

    /// <summary>
    /// Application-layer wiring shared by every store: CQS handlers, AutoMapper profiles,
    /// and cross-cutting services.
    /// </summary>
    private static IServiceCollection AddApplicationCore(this IServiceCollection services)
    {
        // CQS scans the whole Application assembly, so Product/EntityDef commands are included.
        services.AddCQS([typeof(CreateOrderCommand).Assembly]);
        services.AddAutoMapper(
            typeof(OrderMappingProfile),
            typeof(ProductMappingProfile),
            typeof(EntityDefMappingProfile));
        services.AddSingleton<IEmailService, MyCustomEmailService>();
        return services;
    }

    /// <summary>MongoDB store: repositories + a read-side <see cref="IQueryable{T}"/> per aggregate.</summary>
    private static IServiceCollection AddMongoStore(this IServiceCollection services, ConfigurationManager configuration)
    {
        var mongoDatabaseConfiguration = configuration.GetConfiguration<MongoDatabaseConfiguration>();
        services.AddMongoDatabase(mongoDatabaseConfiguration)
            .RegisterMongoDBClassMaps()
            .RegisterDynamicDataClassMaps()
            .AddMongoRepository<Order, Guid>()
            .AddMongoRepository<Product, Guid>()
            .AddMongoRepository<EntityDef, Guid>();

        // Unified read handle: a LINQ queryable over the tenant's collection, used by REST/OData and
        // the queryable GraphQL path. (The Mongo GraphQL recipe keeps using IMongoDatabase directly.)
        services.AddMongoQueryable<Order>();
        services.AddMongoQueryable<Product>();
        services.AddMongoQueryable<EntityDef>();

        return services;
    }

    private static IServiceCollection AddMongoQueryable<T>(this IServiceCollection services)
        where T : class, BytLabs.Domain.Entities.IEntity, BytLabs.Domain.Entities.ISoftDeletable
        // Exclude soft-deleted rows to match EF's global query filter, so REST/OData reads behave
        // identically on both stores.
        => services.AddScoped<IQueryable<T>>(sp =>
            sp.GetRequiredService<IMongoDatabase>().GetCollection<T>().AsQueryable().Where(x => !x.IsDeleted));

    /// <summary>PostgreSQL store (EF Core): per-tenant DbContext, repositories, and read-side IQueryable.</summary>
    private static IServiceCollection AddPostgresStore(this IServiceCollection services, ConfigurationManager configuration)
    {
        var efConfig = configuration.GetConfiguration<EfDatabaseConfiguration>();
        services.AddEntityFrameworkDatabase<AppDbContext>(
                efConfig,
                (options, connectionString) => options.UseNpgsql(
                    connectionString,
                    npgsql => npgsql.MigrationsAssembly(typeof(AppDbContext).Assembly.GetName().Name)))
            .AddEfRepository<Order, Guid>()
            .AddEfRepository<Product, Guid>()
            .AddEfRepository<EntityDef, Guid>();

        // Flush pending EF changes before repository reads so inline domain-event handlers that re-read
        // the just-written aggregate work the same as on MongoDB (which writes eagerly). Outermost
        // decorator, wrapping the domain-event dispatcher.
        services.Decorate<IRepository<Order, Guid>, FlushOnReadRepository<Order, Guid>>();
        services.Decorate<IRepository<Product, Guid>, FlushOnReadRepository<Product, Guid>>();
        services.Decorate<IRepository<EntityDef, Guid>, FlushOnReadRepository<EntityDef, Guid>>();

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

        return services;
    }
}
