using BytLabs.Domain.DomainEvents;

namespace BytLabs.MicroserviceTemplate.Domain.Orders.Events
{
    // Data-less domain event (soft delete).
    public class OrderRemoved(Guid id) : DomainEventBase
    {
        public Guid Id { get; } = id;
    }
}
