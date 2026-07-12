using BytLabs.Application.CQS.Commands;
using BytLabs.MicroserviceTemplate.Application.Orders.Dtos;

namespace BytLabs.MicroserviceTemplate.Application.Orders.Commands.RemoveOrder
{
    public record RemoveOrderCommand(Guid Id) : ICommand<OrderDto>;
}
