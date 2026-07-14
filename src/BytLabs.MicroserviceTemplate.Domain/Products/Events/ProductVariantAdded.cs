using BytLabs.Domain.DomainEvents;
using BytLabs.MicroserviceTemplate.Domain.Products.DataObjects;

namespace BytLabs.MicroserviceTemplate.Domain.Products.Events
{
    public record ProductVariantAdded(Guid Id, AddVariant Data) : DomainEventBase<Guid, AddVariant>(Id, Data);
}
