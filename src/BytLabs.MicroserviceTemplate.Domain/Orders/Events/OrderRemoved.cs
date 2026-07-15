using BytLabs.Domain.DomainEvents;

namespace BytLabs.MicroserviceTemplate.Domain.Orders.Events
{
    // Data-less domain event (soft delete).
    public record OrderRemoved(Guid Id) : DomainEventBase<Guid>(Id);
}
