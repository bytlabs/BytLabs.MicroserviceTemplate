using BytLabs.Api.Graphql.ErrorTypes.Business;
using BytLabs.Api.Graphql.ErrorTypes.Validation;
using BytLabs.MicroserviceTemplate.Application.Orders.Commands.CreateOrder;
using BytLabs.MicroserviceTemplate.Application.Orders.Commands.ShipOrder;
using BytLabs.MicroserviceTemplate.Application.Orders.Commands.UpdateOrder;
using BytLabs.MicroserviceTemplate.Application.Orders.Commands.RemoveOrder;
using BytLabs.MicroserviceTemplate.Application.Orders.Dtos;
using HotChocolate;
using HotChocolate.Types;
using MediatR;

namespace BytLabs.MicroserviceTemplate.Api.Graphql.Mutations
{
    public partial class Mutation
    {
        [Error(typeof(BusinessError))]
        [Error(typeof(ValidationError))]
        public async Task<CreateOrderResult> CreateOrder(CreateOrderCommand input, [Service] IMediator mediator, CancellationToken cancellationToken)
        {
            return await mediator.Send(input, cancellationToken);
        }

        [Error(typeof(BusinessError))]
        [Error(typeof(ValidationError))]
        public async Task<ShipOrderResult> MarkOrderAsShipped(ShipOrderCommand input, [Service] IMediator mediator, CancellationToken cancellationToken)
        {
            return await mediator.Send(input, cancellationToken);
        }

        [Error(typeof(BusinessError))]
        [Error(typeof(ValidationError))]
        public async Task<OrderDto> UpdateOrder(UpdateOrderCommand input, [Service] IMediator mediator, CancellationToken cancellationToken)
            => await mediator.Send(input, cancellationToken);

        [Error(typeof(BusinessError))]
        [Error(typeof(ValidationError))]
        public async Task<OrderDto> RemoveOrder(RemoveOrderCommand input, [Service] IMediator mediator, CancellationToken cancellationToken)
            => await mediator.Send(input, cancellationToken);
    }
}
