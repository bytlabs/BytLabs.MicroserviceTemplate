using BytLabs.Domain.DomainEvents;
using BytLabs.MicroserviceTemplate.Domain.EntityDefs.Inputs;

namespace BytLabs.MicroserviceTemplate.Domain.EntityDefs.Events
{
    public record EntityDefCreated(Guid Id, CreateEntityDef Data) : DomainEventBase<Guid, CreateEntityDef>(Id, Data);
}
