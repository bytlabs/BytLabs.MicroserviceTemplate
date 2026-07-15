using System.Linq.Expressions;
using BytLabs.Application.DataAccess;
using BytLabs.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BytLabs.MicroserviceTemplate.Infrastructure.Postgres;

/// <summary>
/// EF-only repository decorator that flushes pending changes (SaveChanges) before read operations.
/// <para>
/// The domain-event dispatcher publishes events inline during <c>InsertAsync</c>/<c>UpdateAsync</c>,
/// before the unit-of-work commits. Handlers that re-read the just-written aggregate (e.g.
/// SendEmailOrderCreatedEventHandler) would miss it on EF, because EfRepository reads hit the database
/// rather than the change tracker. Flushing before reads makes the pending insert/update visible within
/// the ambient command transaction, giving the same behaviour the MongoDB store has by writing eagerly.
/// </para>
/// </summary>
internal sealed class FlushOnReadRepository<TAggregateRoot, TIdentity> : IRepository<TAggregateRoot, TIdentity>
    where TAggregateRoot : class, IAggregateRoot<TIdentity>
{
    private readonly IRepository<TAggregateRoot, TIdentity> _inner;
    private readonly DbContext _dbContext;

    public FlushOnReadRepository(IRepository<TAggregateRoot, TIdentity> inner, DbContext dbContext)
    {
        _inner = inner;
        _dbContext = dbContext;
    }

    private async Task FlushAsync(CancellationToken ct)
    {
        if (_dbContext.ChangeTracker.HasChanges())
            await _dbContext.SaveChangesAsync(ct);
    }

    public async Task<TAggregateRoot> GetByIdAsync(TIdentity id, CancellationToken cancellationToken)
    {
        await FlushAsync(cancellationToken);
        return await _inner.GetByIdAsync(id, cancellationToken);
    }

    public async Task<TAggregateRoot?> FindByIdAsync(TIdentity id, CancellationToken cancellationToken)
    {
        await FlushAsync(cancellationToken);
        return await _inner.FindByIdAsync(id, cancellationToken);
    }

    public async Task<TAggregateRoot?> SingleOrDefaultAsync(CancellationToken cancellationToken)
    {
        await FlushAsync(cancellationToken);
        return await _inner.SingleOrDefaultAsync(cancellationToken);
    }

    public async Task<List<TAggregateRoot>> FindAllAsync(List<TIdentity> ids, CancellationToken cancellationToken)
    {
        await FlushAsync(cancellationToken);
        return await _inner.FindAllAsync(ids, cancellationToken);
    }

    public async Task<IEnumerable<TAggregateRoot>> FindAllByAsync(
        Expression<Func<TAggregateRoot, bool>> filterExpression, CancellationToken cancellationToken)
    {
        await FlushAsync(cancellationToken);
        return await _inner.FindAllByAsync(filterExpression, cancellationToken);
    }

    // Writes delegate straight through (persistence is deferred to the unit-of-work commit).
    public Task<TAggregateRoot> InsertAsync(TAggregateRoot entity, CancellationToken cancellationToken)
        => _inner.InsertAsync(entity, cancellationToken);

    public Task<TAggregateRoot> UpdateAsync(TAggregateRoot entity, CancellationToken cancellationToken)
        => _inner.UpdateAsync(entity, cancellationToken);

    public Task DeleteAsync(TAggregateRoot entity, CancellationToken cancellationToken)
        => _inner.DeleteAsync(entity, cancellationToken);

    public Task InsertBatchAsync(List<TAggregateRoot> aggregates, CancellationToken cancellationToken)
        => _inner.InsertBatchAsync(aggregates, cancellationToken);

    public Task UpdateBatchAsync(List<TAggregateRoot> aggregates, CancellationToken cancellationToken)
        => _inner.UpdateBatchAsync(aggregates, cancellationToken);

    public Task DeleteBatchAsync(List<TIdentity> ids, CancellationToken cancellationToken)
        => _inner.DeleteBatchAsync(ids, cancellationToken);

    public Task DeleteBatchAsync(List<TAggregateRoot> agregates, CancellationToken cancellationToken)
        => _inner.DeleteBatchAsync(agregates, cancellationToken);
}
