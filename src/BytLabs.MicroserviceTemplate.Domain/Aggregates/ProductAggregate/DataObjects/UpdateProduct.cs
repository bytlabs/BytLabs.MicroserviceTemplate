using System.Text.Json;

namespace BytLabs.MicroserviceTemplate.Domain.Aggregates.ProductAggregate.DataObjects
{
    public record UpdateProduct(string Name, JsonElement Data);
}
