using System.Text.Json;
using BytLabs.MicroserviceTemplate.Domain.Orders.Aggregates;
using BytLabs.MicroserviceTemplate.Domain.Orders.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace BytLabs.MicroserviceTemplate.Infrastructure.Postgres.Configurations;

internal sealed class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("Orders");
        builder.HasKey(x => x.Id);
        builder.OwnsOne(x => x.AuditInfo);
        builder.HasQueryFilter(x => !x.IsDeleted);

        builder.Property(x => x.Data)
            .HasColumnType("jsonb")
            .HasConversion(EfConverters.JsonElement)
            .Metadata.SetValueComparer(EfConverters.JsonElementComparer);

        // NOTE: Items is mapped as jsonb (not a child table) because Order's only constructor takes
        // `items`, and EF cannot bind a navigation to a constructor parameter. Switching to a child
        // table would require a Domain change to Order. See design decision log.
        var itemsConverter = new ValueConverter<IReadOnlySet<OrderItem>, string>(
            v => JsonSerializer.Serialize(v, PostgresJson.Options),
            v => JsonSerializer.Deserialize<HashSet<OrderItem>>(v, PostgresJson.Options) ?? new HashSet<OrderItem>());

        var itemsComparer = new ValueComparer<IReadOnlySet<OrderItem>>(
            (a, b) => JsonSerializer.Serialize(a, PostgresJson.Options) == JsonSerializer.Serialize(b, PostgresJson.Options),
            v => JsonSerializer.Serialize(v, PostgresJson.Options).GetHashCode(),
            v => v);

        builder.Property(x => x.Items)
            .HasColumnType("jsonb")
            .HasConversion(itemsConverter)
            .Metadata.SetValueComparer(itemsComparer);
    }
}
