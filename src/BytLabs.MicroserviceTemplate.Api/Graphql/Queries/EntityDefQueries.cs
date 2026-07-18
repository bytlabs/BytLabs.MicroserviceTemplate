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
        IResolverContext context,
        CancellationToken cancellationToken)
    {
        var query = entityDefs.ExcludeSoftDeletedEntities();

        // Apply the scalar `where` (e.g. entityType eq) to the entity queryable before projecting,
        // so it isn't the middleware no-op on the projected DTO queryable.
        var filter = context.GetFilterContext();
        if (filter?.AsPredicate<EntityDef>() is { } predicate)
            query = query.Where(predicate);
        filter?.Handled(true);

        return query.ProjectTo<EntityDefDto>(mapper.ConfigurationProvider);
    }
}
