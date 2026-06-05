using BytLabs.Domain.DomainEvents;
using BytLabs.MicroserviceTemplate.Domain.Aggregates.ProductAggregate.DataObjects;

namespace BytLabs.MicroserviceTemplate.Domain.Aggregates.ProductAggregate.Events
{
    public class ProductUpdated(Guid id, UpdateProduct data) : DomainEventBase<Guid, UpdateProduct>(id, data);
}
