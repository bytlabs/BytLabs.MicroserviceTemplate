using System.Text.Json;
using BytLabs.MicroserviceTemplate.Domain.Products.Aggregates;
using BytLabs.MicroserviceTemplate.Domain.Products.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

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

        builder.Property(x => x.Data)
            .HasColumnType("jsonb")
            .HasConversion(EfConverters.JsonElement)
            .Metadata.SetValueComparer(EfConverters.JsonElementComparer);

        var variantsConverter = new ValueConverter<IReadOnlySet<ProductVariant>, string>(
            v => JsonSerializer.Serialize(v, PostgresJson.Options),
            v => JsonSerializer.Deserialize<HashSet<ProductVariant>>(v, PostgresJson.Options) ?? new HashSet<ProductVariant>());

        var variantsComparer = new ValueComparer<IReadOnlySet<ProductVariant>>(
            (a, b) => JsonSerializer.Serialize(a, PostgresJson.Options) == JsonSerializer.Serialize(b, PostgresJson.Options),
            v => JsonSerializer.Serialize(v, PostgresJson.Options).GetHashCode(),
            v => v);

        builder.Property(x => x.Variants)
            .HasColumnType("jsonb")
            .HasConversion(variantsConverter)
            .Metadata.SetValueComparer(variantsComparer);
    }
}
