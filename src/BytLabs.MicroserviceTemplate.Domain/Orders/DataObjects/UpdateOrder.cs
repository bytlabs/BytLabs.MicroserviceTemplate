using System.Text.Json;

namespace BytLabs.MicroserviceTemplate.Domain.Orders.DataObjects
{
    // RECIPE: DataObject for updating an Order's dynamic data (merged into existing Data).
    public record UpdateOrder(JsonElement Data);
}
