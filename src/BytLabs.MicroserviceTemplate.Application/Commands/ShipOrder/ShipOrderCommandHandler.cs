using BytLabs.Application.CQS.Commands;
using BytLabs.Application.DataAccess;
using BytLabs.MicroserviceTemplate.Domain.Aggregates.OrderAggregate;
using MediatR;

namespace BytLabs.MicroserviceTemplate.Application.Commands.ShipOrder
{
    public class ShipOrderCommandHandler : ICommandHandler<ShipOrderCommand, ShipOrderResult>
    {
        private readonly IRepository<Order, Guid> orderRepository;

        public ShipOrderCommandHandler(IRepository<Order,Guid> orderRepository)
        {
            this.orderRepository = orderRepository;
        }

        public async Task<ShipOrderResult> Handle(ShipOrderCommand request, CancellationToken cancellationToken)
        {
            var order = await orderRepository.GetByIdAsync(request.OrderId, cancellationToken);
            order.MarkAsShipped();
            await orderRepository.UpdateAsync(order, cancellationToken);
            return new ShipOrderResult(request.OrderId);
        }
    }
}
