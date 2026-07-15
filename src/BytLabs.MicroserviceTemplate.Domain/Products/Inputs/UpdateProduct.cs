using System.Text.Json;

namespace BytLabs.MicroserviceTemplate.Domain.Products.Inputs
{
    public record UpdateProduct(string Name, JsonElement Data, IReadOnlyCollection<VariantData>? Variants = null);
}
