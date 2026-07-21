using BytLabs.Hotchocolate.ErrorTypes.Business;
using BytLabs.Hotchocolate.ErrorTypes.Validation;
using BytLabs.MicroserviceTemplate.Application.EntityDefs.Commands.CreateEntityDef;
using BytLabs.MicroserviceTemplate.Application.EntityDefs.Commands.RemoveEntityDef;
using BytLabs.MicroserviceTemplate.Application.EntityDefs.Commands.UpdateEntityDef;
using BytLabs.MicroserviceTemplate.Application.EntityDefs.Dtos;
using HotChocolate;
using HotChocolate.Authorization;
using HotChocolate.Types;
using MediatR;

namespace BytLabs.MicroserviceTemplate.Api.Graphql.Mutations
{
    public partial class Mutation
    {
        [Authorize]
        [Error(typeof(BusinessError))]
        [Error(typeof(ValidationError))]
        public async Task<EntityDefDto> CreateEntityDef(CreateEntityDefCommand input, [Service] IMediator mediator, CancellationToken cancellationToken)
            => await mediator.Send(input, cancellationToken);

        [Authorize]
        [Error(typeof(BusinessError))]
        [Error(typeof(ValidationError))]
        public async Task<EntityDefDto> UpdateEntityDef(UpdateEntityDefCommand input, [Service] IMediator mediator, CancellationToken cancellationToken)
            => await mediator.Send(input, cancellationToken);

        [Authorize]
        [Error(typeof(BusinessError))]
        [Error(typeof(ValidationError))]
        public async Task<EntityDefDto> RemoveEntityDef(RemoveEntityDefCommand input, [Service] IMediator mediator, CancellationToken cancellationToken)
            => await mediator.Send(input, cancellationToken);
    }
}
