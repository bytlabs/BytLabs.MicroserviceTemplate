using System.Text.Json;
using BytLabs.Application.CQS.Commands;
using BytLabs.MicroserviceTemplate.Domain.Aggregates.OrderAggregate;

namespace BytLabs.MicroserviceTemplate.Application.Commands.CreateOrder
{
    public record CreateOrderCommand(Guid OrderId, DateTime OrderDate, IEnumerable<OrderItem> Items, JsonElement Data) : ICommand<CreateOrderResult>;
    public record CreateOrderResult(Guid OrderId);
}
