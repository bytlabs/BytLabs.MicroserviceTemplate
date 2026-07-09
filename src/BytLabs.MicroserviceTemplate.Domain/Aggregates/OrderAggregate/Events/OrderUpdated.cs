using BytLabs.Domain.DomainEvents;
using BytLabs.MicroserviceTemplate.Domain.Aggregates.OrderAggregate.DataObjects;

namespace BytLabs.MicroserviceTemplate.Domain.Aggregates.OrderAggregate.Events
{
    public class OrderUpdated(Guid id, UpdateOrder data) : DomainEventBase<Guid, UpdateOrder>(id, data);
}
