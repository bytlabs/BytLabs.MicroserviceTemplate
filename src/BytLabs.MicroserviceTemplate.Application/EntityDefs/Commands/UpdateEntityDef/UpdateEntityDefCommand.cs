using BytLabs.Application.CQS.Commands;
using BytLabs.Domain.DynamicData;
using BytLabs.MicroserviceTemplate.Application.EntityDefs.Dtos;

namespace BytLabs.MicroserviceTemplate.Application.EntityDefs.Commands.UpdateEntityDef
{
    public record UpdateEntityDefCommand(Guid Id, FormDataSchema Form, TableDataSchema Table)
        : ICommand<EntityDefDto>;
}
