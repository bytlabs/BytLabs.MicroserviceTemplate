using BytLabs.Domain.DomainEvents;

namespace BytLabs.MicroserviceTemplate.Domain.Aggregates.OrderAggregate.Events
{
    // Data-less domain event: derive from DomainEventBase (records cannot inherit it),
    // which supplies the CreatedAt/CreatedBy members required by IDomainEvent in 4.x.
    public class OrderCreatedEvent(Guid orderId) : DomainEventBase
    {
        public Guid OrderId { get; } = orderId;
    }
}
