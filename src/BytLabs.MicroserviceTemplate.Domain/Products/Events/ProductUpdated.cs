using BytLabs.Domain.DomainEvents;
using BytLabs.MicroserviceTemplate.Domain.Products.DataObjects;

namespace BytLabs.MicroserviceTemplate.Domain.Products.Events
{
    public record ProductUpdated(Guid Id, UpdateProduct Data) : DomainEventBase<Guid, UpdateProduct>(Id, Data);
}
