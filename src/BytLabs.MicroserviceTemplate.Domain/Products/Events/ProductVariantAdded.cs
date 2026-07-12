using BytLabs.Domain.DomainEvents;
using BytLabs.MicroserviceTemplate.Domain.Products.DataObjects;

namespace BytLabs.MicroserviceTemplate.Domain.Products.Events
{
    public class ProductVariantAdded(Guid id, AddVariant data) : DomainEventBase<Guid, AddVariant>(id, data);
}
