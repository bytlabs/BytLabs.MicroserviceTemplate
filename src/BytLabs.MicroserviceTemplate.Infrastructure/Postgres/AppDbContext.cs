using BytLabs.DataAccess.EntityFramework;
using BytLabs.MicroserviceTemplate.Domain.Orders.Aggregates;
using Microsoft.EntityFrameworkCore;

namespace BytLabs.MicroserviceTemplate.Infrastructure.Postgres;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

        // Unmap DomainEvents on every aggregate root in the Domain assembly.
        modelBuilder.IgnoreDomainEvents(typeof(Order).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}
