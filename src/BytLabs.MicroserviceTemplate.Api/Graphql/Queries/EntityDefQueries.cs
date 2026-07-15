using BytLabs.DataAccess.MongoDB.Extensions;
using BytLabs.DataAccess.MongoDB.DynamicData;
using BytLabs.MicroserviceTemplate.Application.EntityDefs.Dtos;
using BytLabs.MicroserviceTemplate.Domain.EntityDefs.Aggregates;
using HotChocolate;
using HotChocolate.Authorization;
using HotChocolate.Data;
using HotChocolate.Types;
using MongoDB.Driver;

namespace BytLabs.MicroserviceTemplate.Api.Graphql.Queries
{
    public partial class Query
    {
        // RECIPE: definition query — excludes soft-deleted rows and projects to a DTO. Clients
        // filter by entityType via `where`. Not dynamic-data (the def has structured fields), so it
        // uses aggregate filter/sort input types, not ApplyDynamicDataFilteration.
        [Authorize]
        [UsePaging]
        [UseProjection]
        [UseFiltering(Type = typeof(EntityDef))]
        [UseSorting(Type = typeof(EntityDef))]
        public IExecutable<EntityDefDto> GetEntityDefs(
            [Service] IMongoDatabase db,
            CancellationToken cancellationToken)
        {
            return db.GetCollection<EntityDef>()
                     .Aggregate()
                     .ExcludeSoftDeletedEntites()
                     .Project(Builders<EntityDef>.Projection.As<EntityDefDto>())
                     .AsExecutable();
        }
    }
}
