using BytLabs.Domain.DomainEvents;

namespace BytLabs.MicroserviceTemplate.Domain.EntityDefs.Events
{
    // Data-less domain event (soft delete).
    public record EntityDefRemoved(Guid Id) : DomainEventBase<Guid>(Id);
}
