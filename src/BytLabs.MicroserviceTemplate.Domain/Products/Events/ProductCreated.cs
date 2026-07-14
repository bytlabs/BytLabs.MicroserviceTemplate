using BytLabs.Domain.DomainEvents;
using BytLabs.MicroserviceTemplate.Domain.Products.DataObjects;

namespace BytLabs.MicroserviceTemplate.Domain.Products.Events
{
    public record ProductCreated(Guid Id, CreateProduct Data) : DomainEventBase<Guid, CreateProduct>(Id, Data);
}
