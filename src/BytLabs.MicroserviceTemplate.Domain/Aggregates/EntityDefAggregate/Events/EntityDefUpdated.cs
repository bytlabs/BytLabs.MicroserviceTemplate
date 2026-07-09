using BytLabs.Domain.DomainEvents;
using BytLabs.MicroserviceTemplate.Domain.Aggregates.EntityDefAggregate.DataObjects;

namespace BytLabs.MicroserviceTemplate.Domain.Aggregates.EntityDefAggregate.Events
{
    public class EntityDefUpdated(Guid id, UpdateEntityDef data) : DomainEventBase<Guid, UpdateEntityDef>(id, data);
}
