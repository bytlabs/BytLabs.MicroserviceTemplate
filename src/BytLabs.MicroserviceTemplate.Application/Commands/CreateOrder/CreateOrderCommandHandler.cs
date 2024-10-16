using BytLabs.Application.CQS.Commands;
using BytLabs.Application.DataAccess;
using BytLabs.MicroserviceTemplate.Domain.Aggregates.OrderAggregate;
using MediatR;

namespace BytLabs.MicroserviceTemplate.Application.Commands.CreateOrder
{
    public class CreateOrderCommandHandler : ICommandHandler<CreateOrderCommand, CreateOrderResult>
    {
        private readonly IRepository<Order, Guid> orderRepository;

        public CreateOrderCommandHandler(IRepository<Order, Guid> orderRepository)
        {
            this.orderRepository = orderRepository;
        }

        public async Task<CreateOrderResult> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            var order = new Order(request.OrderId, request.OrderDate, request.Items);
            await orderRepository.InsertAsync(order, cancellationToken);
            return new CreateOrderResult(request.OrderId);
        }
    }
}
