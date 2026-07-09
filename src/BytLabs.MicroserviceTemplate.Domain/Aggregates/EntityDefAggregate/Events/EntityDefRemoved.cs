using BytLabs.Domain.DomainEvents;

namespace BytLabs.MicroserviceTemplate.Domain.Aggregates.EntityDefAggregate.Events
{
    // Data-less domain event (soft delete). Derives from DomainEventBase to satisfy IDomainEvent.
    public class EntityDefRemoved(Guid id) : DomainEventBase
    {
        public Guid Id { get; } = id;
    }
}
