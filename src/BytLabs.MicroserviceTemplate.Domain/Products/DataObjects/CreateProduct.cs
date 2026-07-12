using System.Text.Json;

namespace BytLabs.MicroserviceTemplate.Domain.Products.DataObjects
{
    // RECIPE: DataObject (factory parameter object) for creating a Product. The form/table schema
    // that describes how to render a Product lives on the EntityDef aggregate, not here.
    public record CreateProduct(Guid Id, string Name, JsonElement Data);
}
