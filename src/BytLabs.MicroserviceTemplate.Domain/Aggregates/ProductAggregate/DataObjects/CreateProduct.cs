using System.Text.Json;
using BytLabs.MicroserviceTemplate.Domain.Shared.DynamicData;

namespace BytLabs.MicroserviceTemplate.Domain.Aggregates.ProductAggregate.DataObjects
{
    // RECIPE: DataObject (factory parameter object) for creating a Product.
    public record CreateProduct(Guid Id, string Name, JsonElement Data, FormDataSchema AttributesSchema);
}
