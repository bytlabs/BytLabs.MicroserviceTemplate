using BytLabs.MicroserviceTemplate.Domain.Shared.DynamicData;

namespace BytLabs.MicroserviceTemplate.Domain.Aggregates.EntityDefAggregate.DataObjects
{
    // RECIPE: DataObject for updating an EntityDef — replaces form + table schemas wholesale.
    public record UpdateEntityDef(FormDataSchema Form, TableDataSchema Table);
}
