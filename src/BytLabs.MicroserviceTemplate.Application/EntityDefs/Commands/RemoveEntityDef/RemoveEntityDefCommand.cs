using BytLabs.Application.CQS.Commands;
using BytLabs.MicroserviceTemplate.Application.EntityDefs.Dtos;

namespace BytLabs.MicroserviceTemplate.Application.EntityDefs.Commands.RemoveEntityDef
{
    public record RemoveEntityDefCommand(Guid Id) : ICommand<EntityDefDto>;
}
