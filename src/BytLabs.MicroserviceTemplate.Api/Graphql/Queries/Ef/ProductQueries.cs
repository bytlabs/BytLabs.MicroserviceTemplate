using AutoMapper;
using AutoMapper.QueryableExtensions;
using BytLabs.Application.DynamicData;
using BytLabs.MicroserviceTemplate.Infrastructure.Postgres.DynamicData;
using BytLabs.MicroserviceTemplate.Application.Products.Dtos;
using BytLabs.MicroserviceTemplate.Domain.Products.Aggregates;
using HotChocolate;
using HotChocolate.Authorization;
using HotChocolate.Data;
using HotChocolate.Data.Filters;
using HotChocolate.Resolvers;
using HotChocolate.Types;
using BytLabs.DataAccess.EntityFramework;
using BytLabs.MicroserviceTemplate.Api.HotChocolate;

namespace BytLabs.MicroserviceTemplate.Api.Graphql.Queries.Ef;

public partial class EfQuery
{
   
    // No [UseProjection]: AutoMapper's ProjectTo already projects to the DTO; HotChocolate's queryable
    // projection can't member-init ProductVariantDto (no parameterless ctor).
    [Authorize]
    [UsePaging]
    [UseFiltering(Type = typeof(Product))]
    public IQueryable<ProductDto> GetProducts(
        [Service] IQueryable<Product> products,
        [Service] IMapper mapper,
        IResolverContext context,
        List<SortInput<Product>>? order,
        CancellationToken cancellationToken)
    {
        var query = products.ExcludeSoftDeletedEntities();

        var filter = context.GetFilterContext();
        if (filter?.AsPredicate<Product>() is { } predicate)
            query = query.Where(predicate);
        filter?.Handled(true);

        return query
            .AppySortingWithDynamicData(order)
            .ProjectTo<ProductDto>(mapper.ConfigurationProvider);
    }
}
