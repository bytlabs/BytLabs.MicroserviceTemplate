using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace BytLabs.MicroserviceTemplate.Infrastructure.Postgres;

// Lets `dotnet ef migrations add` build an AppDbContext without booting the API. The connection
// string is only used for provider/type resolution during scaffolding; migrations add does not connect.
public sealed class AppDbContextDesignTimeFactory : IDesignTimeDbContextFactory<AppDbContext>
{
    public AppDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseNpgsql(
                "Host=localhost;Port=5432;Database=microservicetemplate_designtime;Username=postgres;Password=postgres",
                npgsql => npgsql.MigrationsAssembly(typeof(AppDbContext).Assembly.GetName().Name))
            .Options;

        return new AppDbContext(options);
    }
}
