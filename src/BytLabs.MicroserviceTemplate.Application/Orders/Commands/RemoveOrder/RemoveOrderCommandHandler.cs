using AutoMapper;
using BytLabs.Application.CQS.Commands;
using BytLabs.Application.DataAccess;
using BytLabs.MicroserviceTemplate.Application.Orders.Dtos;
using BytLabs.MicroserviceTemplate.Domain.Orders.Aggregates;
using BytLabs.MicroserviceTemplate.Domain.Orders.Entities;
using BytLabs.MicroserviceTemplate.Domain.Orders.ValueObjects;

namespace BytLabs.MicroserviceTemplate.Application.Orders.Commands.RemoveOrder
{
    public class RemoveOrderCommandHandler : ICommandHandler<RemoveOrderCommand, OrderDto>
    {
        private readonly IRepository<Order, Guid> orderRepository;
        private readonly IMapper mapper;

        public RemoveOrderCommandHandler(IRepository<Order, Guid> orderRepository, IMapper mapper)
        {
            this.orderRepository = orderRepository;
            this.mapper = mapper;
        }

        public async Task<OrderDto> Handle(RemoveOrderCommand request, CancellationToken cancellationToken)
        {
            var order = await orderRepository.GetByIdAsync(request.Id, cancellationToken);
            order.Remove(); // soft delete
            var result = await orderRepository.UpdateAsync(order, cancellationToken);
            return mapper.Map<OrderDto>(result);
        }
    }
}
