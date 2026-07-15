using BytLabs.Domain.DomainEvents;

namespace BytLabs.MicroserviceTemplate.Domain.Products.Events
{
    // Data-less domain event (soft delete).
    public record ProductRemoved(Guid Id) : DomainEventBase<Guid>(Id);
}
