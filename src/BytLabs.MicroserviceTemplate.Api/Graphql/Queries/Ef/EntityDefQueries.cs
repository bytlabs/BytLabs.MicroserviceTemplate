using AutoMapper;
using AutoMapper.QueryableExtensions;
using BytLabs.MicroserviceTemplate.Api.Querying;
using BytLabs.MicroserviceTemplate.Application.EntityDefs.Dtos;
using BytLabs.MicroserviceTemplate.Domain.EntityDefs.Aggregates;
using HotChocolate;
using HotChocolate.Authorization;
using HotChocolate.Data;
using HotChocolate.Types;

namespace BytLabs.MicroserviceTemplate.Api.Graphql.Queries.Ef;

public partial class EfQuery
{
    // No [UseProjection]: AutoMapper's ProjectTo already projects to the DTO, and HotChocolate's
    // queryable projection cannot member-init the DataSchema value objects (no parameterless ctor).
    [Authorize]
    [UsePaging]
    [UseFiltering(Type = typeof(EntityDef))]
    [UseSorting(Type = typeof(EntityDef))]
    public IQueryable<EntityDefDto> GetEntityDefs(
        [Service] IQueryable<EntityDef> entityDefs,
        [Service] IMapper mapper,
        CancellationToken cancellationToken)
        => entityDefs
        .ExcludeSoftDeletedEntities()
        .ProjectTo<EntityDefDto>(mapper.ConfigurationProvider);
}
