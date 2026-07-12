using BytLabs.MicroserviceTemplate.Domain.Common.DynamicData;

namespace BytLabs.MicroserviceTemplate.Domain.EntityDefs.DataObjects
{
    // RECIPE: DataObject (factory parameter object) for creating an EntityDef.
    public record CreateEntityDef(Guid Id, string EntityType, FormDataSchema Form, TableDataSchema Table);
}
