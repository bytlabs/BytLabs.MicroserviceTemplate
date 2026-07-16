using AutoMapper;
using AutoMapper.QueryableExtensions;
using BytLabs.MicroserviceTemplate.Application.EntityDefs.Dtos;
using BytLabs.MicroserviceTemplate.Domain.EntityDefs.Aggregates;
using HotChocolate;
using HotChocolate.Authorization;
using HotChocolate.Data;
using HotChocolate.Types;

namespace BytLabs.MicroserviceTemplate.Api.Graphql.Queries.Ef;

public partial class EfQuery
{
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
