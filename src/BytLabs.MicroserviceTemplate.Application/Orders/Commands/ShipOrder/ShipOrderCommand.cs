using BytLabs.Application.CQS.Commands;

namespace BytLabs.MicroserviceTemplate.Application.Orders.Commands.ShipOrder
{
    public record ShipOrderCommand(Guid OrderId) : ICommand<ShipOrderResult>;
    public record ShipOrderResult(Guid OrderId);
}
