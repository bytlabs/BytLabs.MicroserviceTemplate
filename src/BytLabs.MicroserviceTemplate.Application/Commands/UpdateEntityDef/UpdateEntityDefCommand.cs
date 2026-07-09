using BytLabs.Application.CQS.Commands;
using BytLabs.MicroserviceTemplate.Application.Dtos;
using BytLabs.MicroserviceTemplate.Domain.Shared.DynamicData;

namespace BytLabs.MicroserviceTemplate.Application.Commands.UpdateEntityDef
{
    public record UpdateEntityDefCommand(Guid Id, FormDataSchema Form, TableDataSchema Table)
        : ICommand<EntityDefDto>;
}
