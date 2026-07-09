using BytLabs.MicroserviceTemplate.Domain.Shared.DynamicData;

namespace BytLabs.MicroserviceTemplate.Domain.Aggregates.EntityDefAggregate.DataObjects
{
    // RECIPE: DataObject (factory parameter object) for creating an EntityDef.
    public record CreateEntityDef(Guid Id, string EntityType, FormDataSchema Form, TableDataSchema Table);
}
