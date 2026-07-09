using BytLabs.Application.CQS.Commands;
using BytLabs.MicroserviceTemplate.Application.Dtos;

namespace BytLabs.MicroserviceTemplate.Application.Commands.RemoveOrder
{
    public record RemoveOrderCommand(Guid Id) : ICommand<OrderDto>;
}
