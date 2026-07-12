using BytLabs.Domain.DomainEvents;
using BytLabs.MicroserviceTemplate.Domain.EntityDefs.DataObjects;

namespace BytLabs.MicroserviceTemplate.Domain.EntityDefs.Events
{
    public class EntityDefUpdated(Guid id, UpdateEntityDef data) : DomainEventBase<Guid, UpdateEntityDef>(id, data);
}
