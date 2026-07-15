using System.Text.Json;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace BytLabs.MicroserviceTemplate.Infrastructure.Postgres;

// Shared EF value converters/comparers for JsonElement dynamic data.
internal static class EfConverters
{
    public static readonly ValueConverter<JsonElement, string> JsonElement =
        new(v => v.ValueKind == JsonValueKind.Undefined ? "{}" : v.GetRawText(),
            v => string.IsNullOrWhiteSpace(v)
                ? JsonDocument.Parse("{}", default(JsonDocumentOptions)).RootElement.Clone()
                : JsonDocument.Parse(v, default(JsonDocumentOptions)).RootElement.Clone());

    public static readonly ValueComparer<JsonElement> JsonElementComparer =
        new((a, b) => a.GetRawText() == b.GetRawText(),
            v => v.GetRawText().GetHashCode(),
            v => v.Clone());
}
