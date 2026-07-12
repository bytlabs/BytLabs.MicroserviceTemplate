using BytLabs.Application.CQS.Commands;
using BytLabs.MicroserviceTemplate.Application.EntityDefs.Dtos;
using BytLabs.MicroserviceTemplate.Domain.Common.DynamicData;

namespace BytLabs.MicroserviceTemplate.Application.EntityDefs.Commands.UpdateEntityDef
{
    public record UpdateEntityDefCommand(Guid Id, FormDataSchema Form, TableDataSchema Table)
        : ICommand<EntityDefDto>;
}
