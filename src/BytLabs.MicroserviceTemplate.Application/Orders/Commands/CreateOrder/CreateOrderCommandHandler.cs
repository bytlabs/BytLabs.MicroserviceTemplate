using System.Text.Json;
using BytLabs.Application.CQS.Commands;
using BytLabs.Application.DataAccess;
using BytLabs.MicroserviceTemplate.Domain.Orders.Aggregates;
using BytLabs.MicroserviceTemplate.Domain.Orders.Entities;
using BytLabs.MicroserviceTemplate.Domain.Orders.ValueObjects;
using MediatR;

namespace BytLabs.MicroserviceTemplate.Application.Orders.Commands.CreateOrder
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
            var data = request.Data ?? JsonSerializer.SerializeToElement(new { });
            var order = Order.Create(request.OrderId, request.OrderDate, request.Items.ToHashSet(), data);
            await orderRepository.InsertAsync(order, cancellationToken);
            return new CreateOrderResult(request.OrderId);
        }
    }
}
