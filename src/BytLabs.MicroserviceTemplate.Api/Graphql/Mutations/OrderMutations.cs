using BytLabs.Domain.Exceptions;
using BytLabs.MicroserviceTemplate.Application.Commands.CreateOrder;
using BytLabs.MicroserviceTemplate.Application.Commands.ShipOrder;
using FluentValidation;
using HotChocolate;
using HotChocolate.Types;
using MediatR;
using ApplicationException = BytLabs.Application.Exceptions.ApplicationException;

namespace BytLabs.MicroserviceTemplate.Api.Graphql.Mutations
{
    public partial class Mutation
    {
        [Error(typeof(ValidationException))]
        [Error(typeof(DomainException))]
        [Error(typeof(ApplicationException))]
        public async Task<CreateOrderResult> CreateOrder(CreateOrderCommand input, [Service] IMediator mediator, CancellationToken cancellationToken)
        {
            return await mediator.Send(input, cancellationToken);
        }

        [Error(typeof(ValidationException))]
        [Error(typeof(DomainException))]
        [Error(typeof(ApplicationException))]
        public async Task<ShipOrderResult> MarkOrderAsShipped(ShipOrderCommand input, [Service] IMediator mediator, CancellationToken cancellationToken)
        {
            return await mediator.Send(input, cancellationToken);
        }
    }
}
