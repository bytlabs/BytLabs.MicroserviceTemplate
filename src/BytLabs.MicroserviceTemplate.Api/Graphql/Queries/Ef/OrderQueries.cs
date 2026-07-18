using AutoMapper;
using AutoMapper.QueryableExtensions;
using BytLabs.Application.DynamicData;
using BytLabs.DataAccess.EntityFramework;
using BytLabs.MicroserviceTemplate.Api.HotChocolate;
using BytLabs.MicroserviceTemplate.Application.Orders.Dtos;
using BytLabs.MicroserviceTemplate.Domain.Orders.Aggregates;
using BytLabs.MicroserviceTemplate.Infrastructure.Postgres.DynamicData;
using HotChocolate;
using HotChocolate.Authorization;
using HotChocolate.Data;
using HotChocolate.Data.Filters;
using HotChocolate.Resolvers;
using HotChocolate.Types;

namespace BytLabs.MicroserviceTemplate.Api.Graphql.Queries.Ef;

/// <summary>
/// EF/PostgreSQL GraphQL query root. Mirrors the MongoDB <c>Query</c> field-for-field (same field
/// names, same <c>where</c>/<c>order</c>/paging arguments, same DTO output) so the generated schema is
/// identical and the single client works against either store. The store's <see cref="IQueryable{T}"/>
/// (EF, no-tracking, soft-delete-filtered) is projected to the DTO with AutoMapper; scalar filtering,
/// sorting and paging are applied by HotChocolate's queryable middleware. Dynamic-data (jsonb) filter
/// and sort are layered on explicitly.
/// </summary>
public partial class EfQuery
{
    // No [UseProjection]: AutoMapper's ProjectTo already projects to the DTO; HotChocolate's queryable
    // projection can't member-init OrderItemDto (no parameterless ctor).
    [Authorize]
    [UsePaging]
    [UseFiltering(Type = typeof(Order))]
    public IQueryable<OrderDto> GetOrders(
        [Service] IQueryable<Order> orders,
        [Service] IMapper mapper,
        IResolverContext context,
        List<SortInput<Order>>? order,
        CancellationToken cancellationToken)
    {
        var query = orders.ExcludeSoftDeletedEntities();

        // Apply the whole `where` (scalar + dynamic-data `data`, via the registered field handlers) to
        // the entity queryable before projecting, then tell the middleware it's handled so it doesn't
        // try to re-apply it to the projected DTO queryable.
        var filter = context.GetFilterContext();
        if (filter?.AsPredicate<Order>() is { } predicate)
            query = query.Where(predicate);
        filter?.Handled(true);

        return query
            .AppySortingWithDynamicData(order)
            .ProjectTo<OrderDto>(mapper.ConfigurationProvider);
    }
}
