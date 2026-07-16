using AutoMapper;
using AutoMapper.QueryableExtensions;
using BytLabs.Application.DynamicData;
using BytLabs.MicroserviceTemplate.Infrastructure.Postgres.DynamicData;
using BytLabs.MicroserviceTemplate.Application.Products.Dtos;
using BytLabs.MicroserviceTemplate.Domain.Products.Aggregates;
using HotChocolate;
using HotChocolate.Authorization;
using HotChocolate.Data;
using HotChocolate.Resolvers;
using HotChocolate.Types;

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
        => products
            .ApplyDynamicDataFilteration(context.ArgumentValue<InputFilteringDynamicData?>("where"))
            .ApplyDynamicDataSorting(order)
            .ProjectTo<ProductDto>(mapper.ConfigurationProvider);
}
