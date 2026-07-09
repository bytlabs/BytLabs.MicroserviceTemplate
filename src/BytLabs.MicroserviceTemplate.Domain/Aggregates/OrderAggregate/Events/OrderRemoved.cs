using BytLabs.Domain.DomainEvents;

namespace BytLabs.MicroserviceTemplate.Domain.Aggregates.OrderAggregate.Events
{
    // Data-less domain event (soft delete).
    public class OrderRemoved(Guid id) : DomainEventBase
    {
        public Guid Id { get; } = id;
    }
}
