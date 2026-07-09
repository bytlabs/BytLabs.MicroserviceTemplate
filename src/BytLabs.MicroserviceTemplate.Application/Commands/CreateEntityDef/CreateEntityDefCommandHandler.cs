using AutoMapper;
using BytLabs.Application.CQS.Commands;
using BytLabs.Application.DataAccess;
using BytLabs.MicroserviceTemplate.Application.Dtos;
using BytLabs.MicroserviceTemplate.Domain.Aggregates.EntityDefAggregate;

namespace BytLabs.MicroserviceTemplate.Application.Commands.CreateEntityDef
{
    public class CreateEntityDefCommandHandler : ICommandHandler<CreateEntityDefCommand, EntityDefDto>
    {
        private readonly IRepository<EntityDef, Guid> repository;
        private readonly IMapper mapper;

        public CreateEntityDefCommandHandler(IRepository<EntityDef, Guid> repository, IMapper mapper)
        {
            this.repository = repository;
            this.mapper = mapper;
        }

        public async Task<EntityDefDto> Handle(CreateEntityDefCommand request, CancellationToken cancellationToken)
        {
            var def = EntityDef.Create(new(request.Id, request.EntityType, request.Form, request.Table));
            var result = await repository.InsertAsync(def, cancellationToken);
            return mapper.Map<EntityDefDto>(result);
        }
    }
}
