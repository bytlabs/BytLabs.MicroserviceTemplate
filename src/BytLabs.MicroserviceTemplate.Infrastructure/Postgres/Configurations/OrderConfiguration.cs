using BytLabs.MicroserviceTemplate.Domain.Orders.Aggregates;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BytLabs.MicroserviceTemplate.Infrastructure.Postgres.Configurations;

internal sealed class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.ToTable("Orders");
        builder.HasKey(x => x.Id);
        builder.OwnsOne(x => x.AuditInfo);
        builder.HasQueryFilter(x => !x.IsDeleted);

        // Dynamic data as NATIVE jsonb (JsonElement, no string converter) so Npgsql can translate
        // jsonb LINQ (data->>'x') for GraphQL dynamic-data filtering/sorting.
        builder.Property(x => x.Data)
            .HasColumnType("jsonb");
        builder.Property(x => x.Data)
            .Metadata.SetValueComparer(EfConverters.JsonElementComparer);

        // Items is a relational child table (Order has a parameter-less EF ctor so the navigation can
        // be populated). Stable OrderItem.Id serves as PK and drives change tracking.
        builder.OwnsMany(x => x.Items, items =>
        {
            items.ToTable("OrderItems");
            items.WithOwner().HasForeignKey("OrderId");
            items.HasKey("Id");
            items.Property(i => i.Id).ValueGeneratedNever();
        });
    }
}
