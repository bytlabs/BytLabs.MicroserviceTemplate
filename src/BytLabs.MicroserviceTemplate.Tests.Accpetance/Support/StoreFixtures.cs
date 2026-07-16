using BytLabs.DataAccess.EntityFramework;
using BytLabs.Multitenancy;
using BytLabs.MicroserviceTemplate.Infrastructure.Postgres;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace BytLabs.MicroserviceTemplate.Tests.Accpetance.Support;

/// <summary>Shared MongoDB-backed API for the acceptance matrix (database-per-tenant, no migration).</summary>
public sealed class MongoStoreFixture : IDisposable
{
    public MatrixApiFactory Factory { get; } = new("Mongo");
    public void Dispose() => Factory.Dispose();
}

/// <summary>
/// Shared PostgreSQL-backed API for the acceptance matrix. Applies EF migrations to the Test tenant's
/// database once before any test runs (requires the docker-compose <c>bytlabs-postgres</c> service).
/// </summary>
public sealed class PostgresStoreFixture : IDisposable
{
    public MatrixApiFactory Factory { get; } = new("Postgres");

    public PostgresStoreFixture()
    {
        var dbFactory = Factory.Services.GetRequiredService<EfDatabaseFactory<AppDbContext>>();
        using var db = dbFactory.GetDbContextForTenant(new TenantId(MatrixApiFactory.TenantId));
        db.Database.Migrate();
    }

    public void Dispose() => Factory.Dispose();
}

[CollectionDefinition("Mongo")]
public sealed class MongoCollection : ICollectionFixture<MongoStoreFixture> { }

[CollectionDefinition("Postgres")]
public sealed class PostgresCollection : ICollectionFixture<PostgresStoreFixture> { }
