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
    {
        return products.ExcludeSoftDeletedEntities()
            .PatchForEfDynamicFilter(context)
            .AppySortingWithDynamicData(order)
            .ProjectTo<ProductDto>(mapper.ConfigurationProvider);
    }
}
