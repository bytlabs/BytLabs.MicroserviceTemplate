using BytLabs.Domain.Entities;

namespace BytLabs.MicroserviceTemplate.Api.Querying;

/// <summary>
/// Read-side <see cref="IQueryable{T}"/> helpers. Mirrors the MongoDB GraphQL
/// <c>IAggregateFluentExtensions.ExcludeSoftDeletedEntites</c>, but for the LINQ queryable used by the
/// REST/OData controllers.
/// </summary>
public static class QueryableExtensions
{
    /// <summary>
    /// Excludes soft-deleted rows. Applied at the controller read site so MongoDB reads match EF, whose
    /// global query filter already excludes them (making this a harmless no-op on the EF store).
    /// </summary>
    public static IQueryable<T> ExcludeSoftDeletedEntities<T>(this IQueryable<T> source)
        where T : ISoftDeletable
        => source.Where(x => !x.IsDeleted);
}
