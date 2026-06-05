using System.Text.Json;
using BytLabs.Domain.Audit;
using BytLabs.Domain.DynamicData;
using BytLabs.MicroserviceTemplate.Domain.Shared.DynamicData;

namespace BytLabs.MicroserviceTemplate.Application.Dtos
{
    // RECIPE: DTO that exposes dynamic data (IHaveDynamicData) and audit info. Used both for
    // AutoMapper (command results) and MongoDB projection (As<ProductDto>) in queries.
    public record ProductDto(
        Guid Id,
        string Name,
        JsonElement Data,
        FormDataSchema AttributesSchema,
        IReadOnlyCollection<ProductVariantDto> Variants,
        AuditInfo AuditInfo) : IAuditable, IHaveDynamicData;
}
