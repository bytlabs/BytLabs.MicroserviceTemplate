using BytLabs.DataAccess.EntityFramework;
using BytLabs.DataAccess.EntityFramework.Configuration;
using BytLabs.Multitenancy;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Npgsql;

namespace BytLabs.MicroserviceTemplate.Infrastructure.Postgres;

/// <summary>
/// Startup helpers for the PostgreSQL store.
/// </summary>
public static class PostgresMigrationExtensions
{
    /// <summary>
    /// For every configured tenant, creates the tenant's database if it does not exist and applies any
    /// pending EF Core migrations. Runs on startup so the schema is ready before traffic is served.
    /// A no-op unless the PostgreSQL store is active (only <c>AddPostgresStore</c> registers the EF
    /// factory), so it is safe to call unconditionally on either store.
    /// </summary>
    /// <remarks>
    /// Each tenant's migration is guarded by a PostgreSQL advisory lock so that multiple app instances
    /// starting together (replicas, or parallel test hosts) serialize instead of racing on DDL — the
    /// first builds the schema and the rest see it already applied and no-op.
    /// </remarks>
    public static void MigratePostgresTenantDatabases(this IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var sp = scope.ServiceProvider;

        // The EF factory is only registered by the Postgres store; on MongoDB this returns null.
        var factory = sp.GetService<EfDatabaseFactory<AppDbContext>>();
        if (factory is null)
            return;

        var configuration = sp.GetRequiredService<EfDatabaseConfiguration>();
        var logger = sp.GetRequiredService<ILoggerFactory>().CreateLogger("PostgresTenantMigrations");

        foreach (var tenant in configuration.Tenants)
        {
            logger.LogInformation(
                "Ensuring database and applying migrations for tenant {TenantId}.", tenant.TenantId);

            using var dbContext = factory.GetDbContextForTenant(new TenantId(tenant.TenantId));
            MigrateUnderAdvisoryLock(dbContext, tenant.ConnectionString);
        }
    }

    /// <summary>
    /// Holds a session-level advisory lock (taken on the always-present <c>postgres</c> maintenance
    /// database, which EF also connects to when creating a database) for the duration of
    /// <see cref="RelationalDatabaseFacadeExtensions.Migrate"/>, so concurrent starters do not race.
    /// </summary>
    private static void MigrateUnderAdvisoryLock(DbContext dbContext, string connectionString)
    {
        var tenantDatabase = new NpgsqlConnectionStringBuilder(connectionString).Database ?? string.Empty;
        var lockKey = StableLockKey(tenantDatabase);

        // Lock on the maintenance database so the lock is reachable even before the tenant DB exists.
        var maintenance = new NpgsqlConnectionStringBuilder(connectionString) { Database = "postgres" };
        using var connection = new NpgsqlConnection(maintenance.ConnectionString);
        connection.Open();

        Execute(connection, "SELECT pg_advisory_lock(@key)", lockKey);
        try
        {
            dbContext.Database.Migrate();
        }
        finally
        {
            Execute(connection, "SELECT pg_advisory_unlock(@key)", lockKey);
        }
    }

    private static void Execute(NpgsqlConnection connection, string sql, long key)
    {
        using var command = connection.CreateCommand();
        command.CommandText = sql;
        command.Parameters.AddWithValue("key", key);
        command.ExecuteNonQuery();
    }

    // Deterministic 64-bit key (FNV-1a) so every instance derives the same advisory-lock id for a tenant.
    private static long StableLockKey(string value)
    {
        unchecked
        {
            ulong hash = 14695981039346656037UL;
            foreach (var c in value)
            {
                hash ^= c;
                hash *= 1099511628211UL;
            }
            return (long)hash;
        }
    }
}
