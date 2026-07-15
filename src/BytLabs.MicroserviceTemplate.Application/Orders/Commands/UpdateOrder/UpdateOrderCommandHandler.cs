using AutoMapper;
using BytLabs.Application.CQS.Commands;
using BytLabs.Application.DataAccess;
using BytLabs.MicroserviceTemplate.Application.Orders.Dtos;
using BytLabs.MicroserviceTemplate.Domain.Orders;
using OrderDataObjects = BytLabs.MicroserviceTemplate.Domain.Orders.DataObjects;

namespace BytLabs.MicroserviceTemplate.Application.Orders.Commands.UpdateOrder
{
    public class UpdateOrderCommandHandler : ICommandHandler<UpdateOrderCommand, OrderDto>
    {
        private readonly IRepository<Order, Guid> orderRepository;
        private readonly IMapper mapper;

        public UpdateOrderCommandHandler(IRepository<Order, Guid> orderRepository, IMapper mapper)
        {
            this.orderRepository = orderRepository;
            this.mapper = mapper;
        }

        public async Task<OrderDto> Handle(UpdateOrderCommand request, CancellationToken cancellationToken)
        {
            var order = await orderRepository.GetByIdAsync(request.Id, cancellationToken);
            order.Update(new OrderDataObjects.UpdateOrder(request.Data, request.Items?.ToList()));
            var result = await orderRepository.UpdateAsync(order, cancellationToken);
            return mapper.Map<OrderDto>(result);
        }
    }
}
