using AutoMapper;
using AutoMapper.QueryableExtensions;
using BytLabs.Application.DynamicData;
using BytLabs.MicroserviceTemplate.Infrastructure.Postgres.DynamicData;
using BytLabs.MicroserviceTemplate.Application.EntityDefs.Dtos;
using BytLabs.MicroserviceTemplate.Application.Orders.Dtos;
using BytLabs.MicroserviceTemplate.Application.Products.Dtos;
using BytLabs.MicroserviceTemplate.Domain.EntityDefs.Aggregates;
using BytLabs.MicroserviceTemplate.Domain.Orders.Aggregates;
using BytLabs.MicroserviceTemplate.Domain.Products.Aggregates;
using HotChocolate;
using HotChocolate.Authorization;
using HotChocolate.Data;
using HotChocolate.Resolvers;
using HotChocolate.Types;

namespace BytLabs.MicroserviceTemplate.Api.Graphql.Queries.Ef;

/// <summary>
/// EF/PostgreSQL GraphQL query root. Mirrors the MongoDB <c>Query</c> field-for-field (same field
/// names, same <c>where</c>/<c>order</c>/paging arguments, same DTO output) so the generated schema is
/// identical and the single client works against either store. The store's <see cref="IQueryable{T}"/>
/// (EF, no-tracking, soft-delete-filtered) is projected to the DTO with AutoMapper; scalar filtering,
/// sorting and paging are applied by HotChocolate's queryable middleware. Dynamic-data (jsonb) filter
/// and sort are layered on in Phase 4.
/// </summary>
public partial class EfQuery
{
    [UsePaging]
    [UseProjection]
    [UseFiltering(Type = typeof(Order))]
    public IQueryable<OrderDto> GetOrders(
        [Service] IQueryable<Order> orders,
        [Service] IMapper mapper,
        IResolverContext context,
        List<SortInput<Order>>? order,
        CancellationToken cancellationToken)
        => orders
            .ApplyDynamicDataFilteration(context.ArgumentValue<InputFilteringDynamicData?>("where"))
            .ApplyDynamicDataSorting(order)
            .ProjectTo<OrderDto>(mapper.ConfigurationProvider);

    [Authorize]
    [UsePaging]
    [UseProjection]
    [UseFiltering(Type = typeof(Product))]
    public IQueryable<ProductDto> GetProducts(
        [Service] IQueryable<Product> products,
        [Service] IMapper mapper,
        IResolverContext context,
        List<SortInput<Product>>? order,
        CancellationToken cancellationToken)
        => products
            .ApplyDynamicDataFilteration(context.ArgumentValue<InputFilteringDynamicData?>("where"))
            .ApplyDynamicDataSorting(order)
            .ProjectTo<ProductDto>(mapper.ConfigurationProvider);

    [Authorize]
    [UsePaging]
    [UseProjection]
    [UseFiltering(Type = typeof(EntityDef))]
    [UseSorting(Type = typeof(EntityDef))]
    public IQueryable<EntityDefDto> GetEntityDefs(
        [Service] IQueryable<EntityDef> entityDefs,
        [Service] IMapper mapper,
        CancellationToken cancellationToken)
        => entityDefs.ProjectTo<EntityDefDto>(mapper.ConfigurationProvider);
}
