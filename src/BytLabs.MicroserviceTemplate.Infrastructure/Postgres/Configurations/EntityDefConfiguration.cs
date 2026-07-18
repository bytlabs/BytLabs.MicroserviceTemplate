using System.Text.Json;
using BytLabs.Domain.DynamicData;
using BytLabs.MicroserviceTemplate.Domain.EntityDefs.Aggregates;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace BytLabs.MicroserviceTemplate.Infrastructure.Postgres.Configurations;

internal sealed class EntityDefConfiguration : IEntityTypeConfiguration<EntityDef>
{
    // PascalCase to match the stored jsonb. The DataSchema value objects round-trip via their public
    // constructors (System.Text.Json ctor binding), so no custom converters are needed.
    private static readonly JsonSerializerOptions SchemaJson = new() { PropertyNamingPolicy = null };

    public void Configure(EntityTypeBuilder<EntityDef> builder)
    {
        builder.ToTable("EntityDefs");
        builder.HasKey(x => x.Id);
        builder.OwnsOne(x => x.AuditInfo);
        builder.HasQueryFilter(x => !x.IsDeleted);
        builder.Property(x => x.EntityType);

        builder.Property(x => x.Form)
            .HasColumnType("jsonb")
            .HasConversion(
                v => JsonSerializer.Serialize(v, SchemaJson),
                v => JsonSerializer.Deserialize<FormDataSchema>(v, SchemaJson)!)
            .Metadata.SetValueComparer(new ValueComparer<FormDataSchema>(
                (a, b) => a!.Equals(b),
                v => v.GetHashCode(),
                v => v));

        builder.Property(x => x.Table)
            .HasColumnType("jsonb")
            .HasConversion(
                v => JsonSerializer.Serialize(v, SchemaJson),
                v => JsonSerializer.Deserialize<TableDataSchema>(v, SchemaJson)!)
            .Metadata.SetValueComparer(new ValueComparer<TableDataSchema>(
                (a, b) => a!.Equals(b),
                v => v.GetHashCode(),
                v => v));
    }
}
