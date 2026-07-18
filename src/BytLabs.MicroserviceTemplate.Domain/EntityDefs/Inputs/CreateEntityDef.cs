using BytLabs.Domain.DynamicData;

namespace BytLabs.MicroserviceTemplate.Domain.EntityDefs.Inputs
{
    // RECIPE: DataObject (factory parameter object) for creating an EntityDef.
    public record CreateEntityDef(Guid Id, string EntityType, FormDataSchema Form, TableDataSchema Table);
}
