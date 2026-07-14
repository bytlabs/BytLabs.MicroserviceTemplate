using BytLabs.Domain.DomainEvents;

namespace BytLabs.MicroserviceTemplate.Domain.Orders.Events
{
    public record OrderShippedEvent(Guid OrderId) : DomainEventBase;
}
