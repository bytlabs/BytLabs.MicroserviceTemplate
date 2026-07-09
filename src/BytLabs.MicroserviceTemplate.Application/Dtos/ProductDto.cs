using System.Text.Json;
using BytLabs.Domain.Audit;
using BytLabs.Domain.DynamicData;

namespace BytLabs.MicroserviceTemplate.Application.Dtos
{
    // RECIPE: DTO that exposes dynamic data (IHaveDynamicData) and audit info. Used both for
    // AutoMapper (command results) and MongoDB projection (As<ProductDto>) in queries. The
    // render schema for a Product lives on the EntityDef aggregate, not on the product itself.
    public record ProductDto(
        Guid Id,
        string Name,
        JsonElement Data,
        IReadOnlyCollection<ProductVariantDto> Variants,
        AuditInfo AuditInfo) : IAuditable, IHaveDynamicData;
}
