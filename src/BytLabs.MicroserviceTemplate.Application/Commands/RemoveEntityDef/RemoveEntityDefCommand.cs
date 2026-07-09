using BytLabs.Application.CQS.Commands;
using BytLabs.MicroserviceTemplate.Application.Dtos;

namespace BytLabs.MicroserviceTemplate.Application.Commands.RemoveEntityDef
{
    public record RemoveEntityDefCommand(Guid Id) : ICommand<EntityDefDto>;
}
