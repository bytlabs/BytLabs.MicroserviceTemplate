using System.Text.Json;
using BytLabs.Application.CQS.Commands;
using BytLabs.MicroserviceTemplate.Domain.Orders.Aggregates;
using BytLabs.MicroserviceTemplate.Domain.Orders.Entities;
using BytLabs.MicroserviceTemplate.Domain.Orders.ValueObjects;

namespace BytLabs.MicroserviceTemplate.Application.Orders.Commands.CreateOrder
{
    // Data is optional: creating an order doesn't require dynamic fields (defaults to an empty object).
    public record CreateOrderCommand(Guid OrderId, DateTime OrderDate, IEnumerable<OrderItem> Items, JsonElement? Data = null) : ICommand<CreateOrderResult>;
    public record CreateOrderResult(Guid OrderId);
}
