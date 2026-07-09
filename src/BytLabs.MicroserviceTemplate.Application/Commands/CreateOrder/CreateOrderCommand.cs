using System.Text.Json;
using BytLabs.Application.CQS.Commands;
using BytLabs.MicroserviceTemplate.Domain.Aggregates.OrderAggregate;

namespace BytLabs.MicroserviceTemplate.Application.Commands.CreateOrder
{
    // Data is optional: creating an order doesn't require dynamic fields (defaults to an empty object).
    public record CreateOrderCommand(Guid OrderId, DateTime OrderDate, IEnumerable<OrderItem> Items, JsonElement? Data = null) : ICommand<CreateOrderResult>;
    public record CreateOrderResult(Guid OrderId);
}
