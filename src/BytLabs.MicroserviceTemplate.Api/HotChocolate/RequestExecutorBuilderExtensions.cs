using BytLabs.Api.Graphql;
using BytLabs.MicroserviceTemplate.Api.Graphql.Queries.Ef;
using BytLabs.MicroserviceTemplate.Api.Graphql.Queries.Mongo;
using BytLabs.MicroserviceTemplate.Api.HotChocolate;
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
using HotChocolate.Data;
using HotChocolate.Data.Filters.Expressions;
using HotChocolate.Execution.Configuration;

namespace BytLabs.MicroserviceTemplate.Infrastructure.HotChocolate
{
    public static class RequestExecutorBuilderExtensions
    {

        public static IRequestExecutorBuilder AddQueryType(this IRequestExecutorBuilder builder, IConfiguration configuration)
        {
            // The read resolvers + query middleware are store-selected, but they register the SAME
            // schema types (commands/DTOs/aggregate filter+sort), so the schema is identical on both
            // stores and one client works against either.
            var isPostgres = string.Equals(
                configuration["DataStore:Provider"], "Postgres", StringComparison.OrdinalIgnoreCase);


            if (isPostgres)
                builder.AddQueryableQuerySettings().AddQueryType<EfQuery>();
            else
                builder.AddMongoDbQuerySettings().AddQueryType<MongoQuery>();

            return builder;
        }


        // RECIPE: enable MongoDB-aware filtering/projection/sorting/paging for GraphQL queries.
        public static IRequestExecutorBuilder AddMongoDbQuerySettings(this IRequestExecutorBuilder builder)
        {
            return builder
                    .AddMongoDbFiltering()
                    .AddMongoDbProjections()
                    .AddMongoDbSorting()
                    .AddMongoDbPagingProviders();
        }

        // Provider-agnostic (IQueryable/EF) filtering/projection/sorting. Paging uses HotChocolate's
        // default queryable cursor provider. Used for the PostgreSQL GraphQL query root.
        public static IRequestExecutorBuilder AddQueryableQuerySettings(this IRequestExecutorBuilder builder)
        {
            return builder
                    .AddFiltering(d => d
                        .AddDefaults()
                        .AddProviderExtension(new QueryableFilterProviderExtension(x =>
                            x.AddFieldHandler<DataPassThroughFilterFieldHandler>())))
                    .AddProjections()
                    .AddSorting()
                    .AddQueryableCursorPagingProvider();
        }

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
            // Only the dynamic-data Product aggregate needs the BytLabs aggregate filter/sort
            // input types. Order uses HotChocolate's built-in [UseFiltering]/[UseSorting].
            return requestExecutorBuilder
                .AddAggregateSortType<Order, Guid>()
                .AddAggregateFilterType<Order, Guid>()
                .AddAggregateSortType<Product, Guid>()
                .AddAggregateFilterType<Product, Guid>()
                // EntityDef query uses [UseSorting(Type=typeof(EntityDef))] which generates the sort
                // input itself, so only the aggregate filter type is registered here (mirrors
                // CandidateManagement's OrganizationSubEntityDef). Registering the sort type too
                // would duplicate `EntityDefSortInput`.
                .AddAggregateFilterType<EntityDef, Guid>();
        }
    }
}
