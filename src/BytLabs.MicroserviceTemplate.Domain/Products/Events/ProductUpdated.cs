using BytLabs.Domain.DomainEvents;
using BytLabs.MicroserviceTemplate.Domain.Products.DataObjects;

namespace BytLabs.MicroserviceTemplate.Domain.Products.Events
{
    public class ProductUpdated(Guid id, UpdateProduct data) : DomainEventBase<Guid, UpdateProduct>(id, data);
}
