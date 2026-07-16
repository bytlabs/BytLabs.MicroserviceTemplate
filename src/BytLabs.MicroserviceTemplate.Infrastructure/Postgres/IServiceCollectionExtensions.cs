using BytLabs.Api.Configuration;
using BytLabs.DataAccess.EntityFramework;
using BytLabs.DataAccess.EntityFramework.Configuration;
using BytLabs.MicroserviceTemplate.Domain.EntityDefs.Aggregates;
using BytLabs.MicroserviceTemplate.Domain.Orders.Aggregates;
using BytLabs.MicroserviceTemplate.Domain.Products.Aggregates;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BytLabs.MicroserviceTemplate.Infrastructure.Postgres
{
    internal static class IServiceCollectionExtensions
    {
        /// <summary>PostgreSQL store (EF Core): per-tenant DbContext, repositories, and read-side IQueryable.</summary>
        internal static IServiceCollection AddPostgresStore(this IServiceCollection services, ConfigurationManager configuration)
        {
            var efConfig = configuration.GetConfiguration<EfDatabaseConfiguration>();
            services.AddEntityFrameworkDatabase<AppDbContext>(
                    efConfig,
                    (options, connectionString) => options.UseNpgsql(
                        connectionString,
                        npgsql => npgsql.MigrationsAssembly(typeof(AppDbContext).Assembly.GetName().Name)))
                .AddEfRepository<Order, Guid>()
                .AddEfRepository<Product, Guid>()
                .AddEfRepository<EntityDef, Guid>();

            // EfRepository flushes each write via SaveChangesAsync, so inline domain-event handlers that
            // re-read the just-written aggregate work the same as on MongoDB (which writes eagerly).

            return services;
        }
    }
}
