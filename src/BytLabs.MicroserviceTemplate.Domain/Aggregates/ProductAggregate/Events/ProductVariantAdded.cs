using BytLabs.Domain.DomainEvents;
using BytLabs.MicroserviceTemplate.Domain.Aggregates.ProductAggregate.DataObjects;

namespace BytLabs.MicroserviceTemplate.Domain.Aggregates.ProductAggregate.Events
{
    public class ProductVariantAdded(Guid id, AddVariant data) : DomainEventBase<Guid, AddVariant>(id, data);
}
