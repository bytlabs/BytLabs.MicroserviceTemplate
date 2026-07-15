using BytLabs.MicroserviceTemplate.Api.OData.Resources;
using Microsoft.OData.Edm;
using Microsoft.OData.ModelBuilder;

namespace BytLabs.MicroserviceTemplate.Api.OData;

public static class EdmModel
{
    public static IEdmModel GetEdmModel()
    {
        var builder = new ODataConventionModelBuilder();
        // Declare child rows as complex types (not entities) so they serialize inline in responses.
        // Without this the convention infers a key from their `Id` and models Items/Variants as
        // navigation properties, which are omitted unless $expand-ed.
        builder.ComplexType<OrderItemResource>();
        builder.ComplexType<ProductVariantResource>();
        builder.EntitySet<OrderResource>("Orders");
        builder.EntitySet<ProductResource>("Products");
        builder.EntitySet<EntityDefResource>("EntityDefs");
        return builder.GetEdmModel();
    }
}
