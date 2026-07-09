using AutoMapper;
using BytLabs.Application.CQS.Commands;
using BytLabs.Application.DataAccess;
using BytLabs.MicroserviceTemplate.Application.Dtos;
using BytLabs.MicroserviceTemplate.Domain.Aggregates.EntityDefAggregate;

namespace BytLabs.MicroserviceTemplate.Application.Commands.UpdateEntityDef
{
    public class UpdateEntityDefCommandHandler : ICommandHandler<UpdateEntityDefCommand, EntityDefDto>
    {
        private readonly IRepository<EntityDef, Guid> repository;
        private readonly IMapper mapper;

        public UpdateEntityDefCommandHandler(IRepository<EntityDef, Guid> repository, IMapper mapper)
        {
            this.repository = repository;
            this.mapper = mapper;
        }

        public async Task<EntityDefDto> Handle(UpdateEntityDefCommand request, CancellationToken cancellationToken)
        {
            var def = await repository.GetByIdAsync(request.Id, cancellationToken);
            def.Update(new(request.Form, request.Table));
            var result = await repository.UpdateAsync(def, cancellationToken);
            return mapper.Map<EntityDefDto>(result);
        }
    }
}
