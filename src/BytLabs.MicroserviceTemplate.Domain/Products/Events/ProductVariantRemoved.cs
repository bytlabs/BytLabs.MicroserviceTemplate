using BytLabs.Domain.DomainEvents;
using BytLabs.MicroserviceTemplate.Domain.Products.DataObjects;

namespace BytLabs.MicroserviceTemplate.Domain.Products.Events
{
    public class ProductVariantRemoved(Guid id, RemoveVariant data) : DomainEventBase<Guid, RemoveVariant>(id, data);
}
