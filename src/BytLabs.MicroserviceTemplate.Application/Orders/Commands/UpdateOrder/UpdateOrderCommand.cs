using System.Text.Json;
using BytLabs.Application.CQS.Commands;
using BytLabs.MicroserviceTemplate.Application.Orders.Dtos;

namespace BytLabs.MicroserviceTemplate.Application.Orders.Commands.UpdateOrder
{
    public record UpdateOrderCommand(Guid Id, JsonElement Data) : ICommand<OrderDto>;
}
