using BytLabs.MicroserviceTemplate.Domain.Common.DynamicData;

namespace BytLabs.MicroserviceTemplate.Domain.EntityDefs.DataObjects
{
    // RECIPE: DataObject for updating an EntityDef — replaces form + table schemas wholesale.
    public record UpdateEntityDef(FormDataSchema Form, TableDataSchema Table);
}
