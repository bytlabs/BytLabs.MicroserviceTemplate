using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace BytLabs.MicroserviceTemplate.Infrastructure.Postgres;

// Lets `dotnet ef migrations add` build an AppDbContext without booting the API. `migrations add`
// never connects — the connection string is only used for provider/type resolution during scaffolding.
// Commands that do connect (`database update`, `migrations list`) aren't the intended workflow here;
// the app migrates each tenant itself on startup. The value defaults to a throwaway local database and
// can be overridden with the EF_DESIGNTIME_CONNECTION environment variable (e.g. on CI).
public sealed class AppDbContextDesignTimeFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    private const string DefaultConnectionString =
        "Host=localhost;Port=5432;Database=microservicetemplate_designtime;Username=postgres;Password=postgres";

    public AppDbContext CreateDbContext(string[] args)
    {
        var connectionString =
            Environment.GetEnvironmentVariable("EF_DESIGNTIME_CONNECTION") ?? DefaultConnectionString;

        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseNpgsql(
                connectionString,
                npgsql => npgsql.MigrationsAssembly(typeof(AppDbContext).Assembly.GetName().Name))
            .Options;

        return new AppDbContext(options);
    }
}
