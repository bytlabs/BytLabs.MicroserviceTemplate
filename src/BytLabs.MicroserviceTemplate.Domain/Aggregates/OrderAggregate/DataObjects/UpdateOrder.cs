using System.Text.Json;

namespace BytLabs.MicroserviceTemplate.Domain.Aggregates.OrderAggregate.DataObjects
{
    // RECIPE: DataObject for updating an Order's dynamic data (merged into existing Data).
    public record UpdateOrder(JsonElement Data);
}
