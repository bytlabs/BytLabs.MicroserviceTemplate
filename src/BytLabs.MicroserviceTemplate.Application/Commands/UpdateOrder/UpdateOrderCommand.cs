using System.Text.Json;
using BytLabs.Application.CQS.Commands;
using BytLabs.MicroserviceTemplate.Application.Dtos;

namespace BytLabs.MicroserviceTemplate.Application.Commands.UpdateOrder
{
    public record UpdateOrderCommand(Guid Id, JsonElement Data) : ICommand<OrderDto>;
}
