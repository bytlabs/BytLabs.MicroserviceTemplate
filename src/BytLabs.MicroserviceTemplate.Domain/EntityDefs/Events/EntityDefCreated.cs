using BytLabs.Domain.DomainEvents;
using BytLabs.MicroserviceTemplate.Domain.EntityDefs.DataObjects;

namespace BytLabs.MicroserviceTemplate.Domain.EntityDefs.Events
{
    public record EntityDefCreated(Guid Id, CreateEntityDef Data) : DomainEventBase<Guid, CreateEntityDef>(Id, Data);
}
