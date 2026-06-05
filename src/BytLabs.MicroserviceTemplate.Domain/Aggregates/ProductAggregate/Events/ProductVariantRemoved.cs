using BytLabs.Domain.DomainEvents;
using BytLabs.MicroserviceTemplate.Domain.Aggregates.ProductAggregate.DataObjects;

namespace BytLabs.MicroserviceTemplate.Domain.Aggregates.ProductAggregate.Events
{
    public class ProductVariantRemoved(Guid id, RemoveVariant data) : DomainEventBase<Guid, RemoveVariant>(id, data);
}
