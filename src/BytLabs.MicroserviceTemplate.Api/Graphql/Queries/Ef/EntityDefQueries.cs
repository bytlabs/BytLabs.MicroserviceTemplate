using AutoMapper;
using AutoMapper.QueryableExtensions;
using BytLabs.DataAccess.EntityFramework;
using BytLabs.MicroserviceTemplate.Application.EntityDefs.Dtos;
using BytLabs.MicroserviceTemplate.Domain.EntityDefs.Aggregates;
using HotChocolate;
using HotChocolate.Authorization;
using HotChocolate.Data;
using HotChocolate.Data.Filters;
using HotChocolate.Resolvers;
using HotChocolate.Types;
using BytLabs.MicroserviceTemplate.Api.HotChocolate;

namespace BytLabs.MicroserviceTemplate.Api.Graphql.Queries.Ef;

public partial class EfQuery
{
    [Authorize]
    [UsePaging]
    [UseFiltering]
    [UseSorting]
    public IQueryable<EntityDefDto> GetEntityDefs(
        [Service] IQueryable<EntityDef> entityDefs,
        [Service] IMapper mapper,
        IResolverContext context,
        CancellationToken cancellationToken)
    {
        return entityDefs
            .ExcludeSoftDeletedEntities()
            .ProjectTo<EntityDefDto>(mapper.ConfigurationProvider);
    }
}
