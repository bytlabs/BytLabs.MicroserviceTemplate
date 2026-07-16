using BytLabs.Domain.DynamicData;

namespace BytLabs.MicroserviceTemplate.Domain.EntityDefs.Inputs
{
    // RECIPE: DataObject for updating an EntityDef — replaces form + table schemas wholesale.
    public record UpdateEntityDef(FormDataSchema Form, TableDataSchema Table);
}
