using BytLabs.Domain.DomainEvents;

namespace BytLabs.MicroserviceTemplate.Domain.Products.Events
{
    // Data-less domain event (soft delete). Derives from DomainEventBase to satisfy IDomainEvent in 4.x.
    public class ProductRemoved(Guid productId) : DomainEventBase
    {
        public Guid ProductId { get; } = productId;
    }
}
