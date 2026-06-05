using BytLabs.Domain.DomainEvents;
using BytLabs.MicroserviceTemplate.Domain.Shared.DynamicData;

namespace BytLabs.MicroserviceTemplate.Domain.Aggregates.ProductAggregate.Events
{
    public class ProductAttributesSchemaUpdated(Guid id, FormDataSchema data) : DomainEventBase<Guid, FormDataSchema>(id, data);
}
