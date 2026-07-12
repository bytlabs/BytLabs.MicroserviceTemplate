using BytLabs.Domain.DomainEvents;
using BytLabs.MicroserviceTemplate.Domain.Products.DataObjects;

namespace BytLabs.MicroserviceTemplate.Domain.Products.Events
{
    // RECIPE: Typed domain event carrying the data that caused it (DomainEventBase<TId, TData>).
    public class ProductCreated(Guid id, CreateProduct data) : DomainEventBase<Guid, CreateProduct>(id, data);
}
