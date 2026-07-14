using BytLabs.Domain.DomainEvents;

namespace BytLabs.MicroserviceTemplate.Domain.Orders.Events
{
    // Data-less domain event as an immutable record (DomainEventBase supplies CreatedAt/CreatedBy).
    public record OrderCreatedEvent(Guid OrderId) : DomainEventBase;
}
