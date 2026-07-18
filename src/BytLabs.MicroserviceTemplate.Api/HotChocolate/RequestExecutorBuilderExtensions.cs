using BytLabs.Api.Graphql;
using BytLabs.MicroserviceTemplate.Api.HotChocolate;
using BytLabs.MicroserviceTemplate.Api.Graphql.Queries.Ef;
using BytLabs.MicroserviceTemplate.Api.Graphql.Queries.Mongo;
using HotChocolate.Data;
using HotChocolate.Data.Filters.Expressions;
using HotChocolate.Data.MongoDb;
using HotChocolate.Data.MongoDb.Filters;
using BytLabs.MicroserviceTemplate.Application.EntityDefs.Commands.CreateEntityDef;
using BytLabs.MicroserviceTemplate.Application.EntityDefs.Commands.RemoveEntityDef;
using BytLabs.MicroserviceTemplate.Application.EntityDefs.Commands.UpdateEntityDef;
using BytLabs.MicroserviceTemplate.Application.EntityDefs.Dtos;
using BytLabs.MicroserviceTemplate.Application.Orders.Commands.CreateOrder;
using BytLabs.MicroserviceTemplate.Application.Orders.Commands.RemoveOrder;
using BytLabs.MicroserviceTemplate.Application.Orders.Commands.ShipOrder;
using BytLabs.MicroserviceTemplate.Application.Orders.Commands.UpdateOrder;
using BytLabs.MicroserviceTemplate.Application.Orders.Dtos;
using BytLabs.MicroserviceTemplate.Application.Products.Commands.AddVariant;
using BytLabs.MicroserviceTemplate.Application.Products.Commands.CreateProduct;
using BytLabs.MicroserviceTemplate.Application.Products.Commands.RemoveProduct;
using BytLabs.MicroserviceTemplate.Application.Products.Commands.RemoveVariant;
using BytLabs.MicroserviceTemplate.Application.Products.Commands.UpdateProduct;
using BytLabs.MicroserviceTemplate.Application.Products.Dtos;
using BytLabs.MicroserviceTemplate.Domain.EntityDefs.Aggregates;
using BytLabs.MicroserviceTemplate.Domain.Orders.Aggregates;
using BytLabs.MicroserviceTemplate.Domain.Products.Aggregates;
using HotChocolate.Execution.Configuration;

namespace BytLabs.MicroserviceTemplate.Infrastructure.HotChocolate
{
    public static class RequestExecutorBuilderExtensions
    {

        // Provider-agnostic (IQueryable/EF) filtering/projection/sorting. Paging uses HotChocolate's
        // default queryable cursor provider. Used for the PostgreSQL/MongoDB GraphQL query root.
        public static IRequestExecutorBuilder AddQueryableQuerySettings(this IRequestExecutorBuilder builder)
        {
            return builder
                    .AddQueryableCursorPagingProvider()
                    .AddProjections()
                    // Register the dynamic-data `data` field handler on the queryable (EF) filter
                    // provider so it translates DataOperationFilter to a jsonb predicate (and the schema
                    // can build). AddDefaults keeps the built-in scalar/and/or handlers.
                    .AddFiltering(d => d
                        .AddDefaults()
                        .AddProviderExtension(new QueryableFilterProviderExtension(x =>
                            x.AddFieldHandler(ctx => new QueryableDynamicDataFilterFieldHandler(ctx.InputParser)))))
                    .AddSorting();
        }

        // MongoDB filtering/projection/sorting/paging. The dynamic-data `data` filter is applied in the
        // MongoQuery resolver (as a $match before the projection, which a post-resolver filter handler
        // cannot do); the default MongoDB filter provider handles scalar fields.
        public static IRequestExecutorBuilder AddMongoDbQuerySettings(this IRequestExecutorBuilder builder)
        {
            return builder
                    .AddMongoDbPagingProviders()
                    .AddMongoDbProjections()
                    .AddMongoDbSorting()
                    .AddFiltering(descriptor => descriptor
                        .AddMongoDbDefaults()
                        .Provider(new MongoDbFilterProvider(p => p
                            .AddFieldHandler(ctx => new MongoDynamicDataFilterFieldHandler(ctx.InputParser))
                            .AddDefaultMongoDbFieldHandlers())));
        }

        // Selects the filter/projection/sorting provider AND the query root for the configured store:
        // Postgres = queryable/EF (EfQuery, filtered via the queryable dynamic-data handler + AsPredicate),
        // otherwise MongoDB (MongoQuery, filtered in the aggregate). The schema is identical on both.
        public static IRequestExecutorBuilder AddStoreQueryType(
            this IRequestExecutorBuilder builder, IConfiguration configuration)
            => string.Equals(configuration["DataStore:Provider"], "Postgres", StringComparison.OrdinalIgnoreCase)
                ? builder.AddQueryableQuerySettings().AddQueryType<EfQuery>()
                : builder.AddMongoDbQuerySettings().AddQueryType<MongoQuery>();

        public static IRequestExecutorBuilder AddCommandTypes(this IRequestExecutorBuilder requestExecutorBuilder)
        {
            return requestExecutorBuilder
                .AddCommandType<CreateOrderCommand>()
                .AddCommandType<ShipOrderCommand>()
                .AddCommandType<UpdateOrderCommand>()
                .AddCommandType<RemoveOrderCommand>()
                .AddCommandType<CreateProductCommand>()
                .AddCommandType<UpdateProductCommand>()
                .AddCommandType<RemoveProductCommand>()
                .AddCommandType<AddVariantCommand>()
                .AddCommandType<RemoveVariantCommand>()
                .AddCommandType<CreateEntityDefCommand>()
                .AddCommandType<UpdateEntityDefCommand>()
                .AddCommandType<RemoveEntityDefCommand>();
        }

        public static IRequestExecutorBuilder AddDtoTypes(this IRequestExecutorBuilder requestExecutorBuilder)
        {
            return requestExecutorBuilder
                .AddDtoType<OrderDto>()
                .AddDtoType<OrderItemDto>()
                .AddType<ProductDtoType>()          // custom DtoType for Product
                .AddDtoType<ProductVariantDto>()
                .AddDtoType<EntityDefDto>();
        }

        public static IRequestExecutorBuilder AddAggregateTypes(this IRequestExecutorBuilder requestExecutorBuilder)
        {
            // Product and Order aggregates implements DynamicData and thus
            // needs the BytLabs aggregate filter/sort input types
            // to handle operations on this dynamic data.
            return requestExecutorBuilder
                .AddAggregateSortType<Order, Guid>()
                .AddAggregateFilterType<Order, Guid>()
                .AddAggregateSortType<Product, Guid>()
                .AddAggregateFilterType<Product, Guid>()
                
                // EntityDef query uses [UseSorting(Type=typeof(EntityDef))] which generates the sort
                // input itself, so only the aggregate filter type is registered here.
                // Registering the sort type too would duplicate `EntityDefSortInput`.
                .AddAggregateFilterType<EntityDef, Guid>();
        }
    }
}
