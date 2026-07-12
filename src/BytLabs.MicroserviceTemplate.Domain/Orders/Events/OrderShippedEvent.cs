using BytLabs.Domain.DomainEvents;

namespace BytLabs.MicroserviceTemplate.Domain.Orders.Events
{
    public class OrderShippedEvent(Guid orderId) : DomainEventBase
    {
        public Guid OrderId { get; } = orderId;
    }
}
