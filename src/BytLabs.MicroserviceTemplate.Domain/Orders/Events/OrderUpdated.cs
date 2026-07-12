using BytLabs.Domain.DomainEvents;
using BytLabs.MicroserviceTemplate.Domain.Orders.DataObjects;

namespace BytLabs.MicroserviceTemplate.Domain.Orders.Events
{
    public class OrderUpdated(Guid id, UpdateOrder data) : DomainEventBase<Guid, UpdateOrder>(id, data);
}
