using System.Text.Json;
using System.Text.Json.Serialization;
using BytLabs.MicroserviceTemplate.Domain.Orders.Entities;
using BytLabs.MicroserviceTemplate.Domain.Products.Entities;
using BytLabs.MicroserviceTemplate.Domain.Products.Inputs;

namespace BytLabs.MicroserviceTemplate.Infrastructure.Postgres;

// OrderItem/ProductVariant have private setters and factory-based construction, so System.Text.Json
// cannot round-trip them by default. These converters serialize all fields (incl. Id) and rebuild via
// the public ctor/factory so identity survives the jsonb round-trip.
internal sealed class OrderItemJsonConverter : JsonConverter<OrderItem>
{
    public override OrderItem Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using var doc = JsonDocument.ParseValue(ref reader);
        var e = doc.RootElement;
        return new OrderItem(
            e.GetProperty("Id").GetGuid(),
            e.GetProperty("ProductId").GetGuid(),
            e.GetProperty("Quantity").GetInt32(),
            e.GetProperty("Price").GetDecimal());
    }

    public override void Write(Utf8JsonWriter writer, OrderItem value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        writer.WriteString("Id", value.Id);
        writer.WriteString("ProductId", value.ProductId);
        writer.WriteNumber("Quantity", value.Quantity);
        writer.WriteNumber("Price", value.Price);
        writer.WriteEndObject();
    }
}

internal sealed class ProductVariantJsonConverter : JsonConverter<ProductVariant>
{
    public override ProductVariant Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        using var doc = JsonDocument.ParseValue(ref reader);
        var e = doc.RootElement;
        return ProductVariant.Create(new CreateVariant(
            e.GetProperty("Id").GetGuid(),
            e.GetProperty("Sku").GetString()!,
            e.GetProperty("Price").GetDecimal()));
    }

    public override void Write(Utf8JsonWriter writer, ProductVariant value, JsonSerializerOptions options)
    {
        writer.WriteStartObject();
        writer.WriteString("Id", value.Id);
        writer.WriteString("Sku", value.Sku);
        writer.WriteNumber("Price", value.Price);
        writer.WriteEndObject();
    }
}

internal static class PostgresJson
{
    public static readonly JsonSerializerOptions Options = new()
    {
        PropertyNamingPolicy = null, // PascalCase, matches the Write() property names above
        Converters = { new OrderItemJsonConverter(), new ProductVariantJsonConverter() }
    };
}
