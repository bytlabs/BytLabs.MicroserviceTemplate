using BytLabs.Domain.DomainEvents;
using BytLabs.MicroserviceTemplate.Domain.EntityDefs.DataObjects;

namespace BytLabs.MicroserviceTemplate.Domain.EntityDefs.Events
{
    public class EntityDefCreated(Guid id, CreateEntityDef data) : DomainEventBase<Guid, CreateEntityDef>(id, data);
}
