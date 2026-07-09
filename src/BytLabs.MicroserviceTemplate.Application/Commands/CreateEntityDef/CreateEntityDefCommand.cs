using BytLabs.Application.CQS.Commands;
using BytLabs.MicroserviceTemplate.Application.Dtos;
using BytLabs.MicroserviceTemplate.Domain.Shared.DynamicData;

namespace BytLabs.MicroserviceTemplate.Application.Commands.CreateEntityDef
{
    public record CreateEntityDefCommand(Guid Id, string EntityType, FormDataSchema Form, TableDataSchema Table)
        : ICommand<EntityDefDto>;
}
