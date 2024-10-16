using BytLabs.Domain.DomainEvents;

namespace BytLabs.MicroserviceTemplate.Domain.Aggregates.OrderAggregate.Events
{
    public record class OrderShippedEvent(Guid OrderId) : IDomainEvent;
}
