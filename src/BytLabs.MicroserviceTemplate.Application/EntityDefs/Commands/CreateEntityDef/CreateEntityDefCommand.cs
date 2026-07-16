using BytLabs.Application.CQS.Commands;
using BytLabs.Domain.DynamicData;
using BytLabs.MicroserviceTemplate.Application.EntityDefs.Dtos;

namespace BytLabs.MicroserviceTemplate.Application.EntityDefs.Commands.CreateEntityDef
{
    public record CreateEntityDefCommand(Guid Id, string EntityType, FormDataSchema Form, TableDataSchema Table)
        : ICommand<EntityDefDto>;
}
