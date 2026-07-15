using BytLabs.Domain.DomainEvents;
using BytLabs.MicroserviceTemplate.Domain.Orders.Inputs;

namespace BytLabs.MicroserviceTemplate.Domain.Orders.Events
{
    public record OrderUpdated(Guid Id, UpdateOrder Data) : DomainEventBase<Guid, UpdateOrder>(Id, Data);
}
