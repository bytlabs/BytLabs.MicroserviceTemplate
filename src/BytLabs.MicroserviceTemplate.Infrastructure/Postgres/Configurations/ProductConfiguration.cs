using BytLabs.MicroserviceTemplate.Domain.Products.Aggregates;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace BytLabs.MicroserviceTemplate.Infrastructure.Postgres.Configurations;

internal sealed class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.ToTable("Products");
        builder.HasKey(x => x.Id);
        builder.OwnsOne(x => x.AuditInfo);
        builder.HasQueryFilter(x => !x.IsDeleted);
        builder.Property(x => x.Name);

        // Dynamic data stays jsonb (schema-less).
        builder.Property(x => x.Data)
            .HasColumnType("jsonb")
            .HasConversion(EfConverters.JsonElement)
            .Metadata.SetValueComparer(EfConverters.JsonElementComparer);

        // Variants is a relational child table (not jsonb) so GraphQL/OData can project/filter/sort it
        // in SQL. ProductVariant carries a stable Id used as PK and for change tracking.
        builder.OwnsMany(x => x.Variants, variants =>
        {
            variants.ToTable("ProductVariants");
            variants.WithOwner().HasForeignKey("ProductId");
            variants.HasKey("Id");
            variants.Property(v => v.Id).ValueGeneratedNever();
        });
    }
}
