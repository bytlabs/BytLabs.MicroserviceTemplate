using BytLabs.Domain.DomainEvents;
using BytLabs.MicroserviceTemplate.Domain.Products.Inputs;

namespace BytLabs.MicroserviceTemplate.Domain.Products.Events
{
    public record ProductCreated(Guid Id, CreateProduct Data) : DomainEventBase<Guid, CreateProduct>(Id, Data);
}
