using BytLabs.Domain.DomainEvents;

namespace BytLabs.MicroserviceTemplate.Domain.Aggregates.OrderAggregate.Events
{
    public record class OrderCreatedEvent(Guid OrderId) : IDomainEvent;
}
