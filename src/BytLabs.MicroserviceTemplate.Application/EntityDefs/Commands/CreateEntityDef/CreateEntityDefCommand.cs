using BytLabs.Application.CQS.Commands;
using BytLabs.MicroserviceTemplate.Application.EntityDefs.Dtos;
using BytLabs.MicroserviceTemplate.Domain.Common.DynamicData;

namespace BytLabs.MicroserviceTemplate.Application.EntityDefs.Commands.CreateEntityDef
{
    public record CreateEntityDefCommand(Guid Id, string EntityType, FormDataSchema Form, TableDataSchema Table)
        : ICommand<EntityDefDto>;
}
