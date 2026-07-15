using System.Text.Json;

namespace BytLabs.MicroserviceTemplate.Domain.Orders.DataObjects
{
    // RECIPE: DataObject for updating an Order. `Data` is merged into existing dynamic data;
    // `Items`, when provided, replaces the whole line-item collection (null = leave items untouched).
    public record UpdateOrder(JsonElement Data, IReadOnlySet<OrderItem>? Items = null);
}
