using System.Text.Json;

namespace BytLabs.MicroserviceTemplate.Domain.Products.DataObjects
{
    public record UpdateProduct(string Name, JsonElement Data);
}
