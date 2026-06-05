using BytLabs.Domain.DomainEvents;
using BytLabs.MicroserviceTemplate.Domain.Aggregates.ProductAggregate.DataObjects;

namespace BytLabs.MicroserviceTemplate.Domain.Aggregates.ProductAggregate.Events
{
    // RECIPE: Typed domain event carrying the data that caused it (DomainEventBase<TId, TData>).
    public class ProductCreated(Guid id, CreateProduct data) : DomainEventBase<Guid, CreateProduct>(id, data);
}
