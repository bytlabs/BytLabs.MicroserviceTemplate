using BytLabs.Domain.DomainEvents;

namespace BytLabs.MicroserviceTemplate.Domain.Orders.Events
{
    // Data-less domain event: DomainEventBase<Guid> supplies Id (+ CreatedAt/CreatedBy).
    public record OrderCreatedEvent(Guid Id) : DomainEventBase<Guid>(Id);
}
