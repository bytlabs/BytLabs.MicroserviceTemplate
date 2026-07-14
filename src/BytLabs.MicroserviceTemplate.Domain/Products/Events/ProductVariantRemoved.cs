using BytLabs.Domain.DomainEvents;
using BytLabs.MicroserviceTemplate.Domain.Products.DataObjects;

namespace BytLabs.MicroserviceTemplate.Domain.Products.Events
{
    public record ProductVariantRemoved(Guid Id, RemoveVariant Data) : DomainEventBase<Guid, RemoveVariant>(Id, Data);
}
