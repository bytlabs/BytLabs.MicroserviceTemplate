using System.Text.Json;
using BytLabs.Application.CQS.Commands;
using BytLabs.MicroserviceTemplate.Application.Orders.Dtos;
using BytLabs.MicroserviceTemplate.Domain.Orders.Aggregates;
using BytLabs.MicroserviceTemplate.Domain.Orders.Entities;
using BytLabs.MicroserviceTemplate.Domain.Orders.ValueObjects;

namespace BytLabs.MicroserviceTemplate.Application.Orders.Commands.UpdateOrder
{
    // Items is optional: when supplied it replaces the order's line items (reuses OrderItemInput,
    // already generated for CreateOrder); null leaves the existing items untouched.
    public record UpdateOrderCommand(Guid Id, JsonElement Data, IEnumerable<OrderItem>? Items = null) : ICommand<OrderDto>;
}
