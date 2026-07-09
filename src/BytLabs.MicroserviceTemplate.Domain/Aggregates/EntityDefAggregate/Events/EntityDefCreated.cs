using BytLabs.Domain.DomainEvents;
using BytLabs.MicroserviceTemplate.Domain.Aggregates.EntityDefAggregate.DataObjects;

namespace BytLabs.MicroserviceTemplate.Domain.Aggregates.EntityDefAggregate.Events
{
    public class EntityDefCreated(Guid id, CreateEntityDef data) : DomainEventBase<Guid, CreateEntityDef>(id, data);
}
