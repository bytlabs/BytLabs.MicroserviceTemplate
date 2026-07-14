using BytLabs.Domain.DomainEvents;
using BytLabs.MicroserviceTemplate.Domain.EntityDefs.DataObjects;

namespace BytLabs.MicroserviceTemplate.Domain.EntityDefs.Events
{
    public record EntityDefUpdated(Guid Id, UpdateEntityDef Data) : DomainEventBase<Guid, UpdateEntityDef>(Id, Data);
}
