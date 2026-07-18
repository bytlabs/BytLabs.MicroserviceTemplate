using System.Text.Json;
using BytLabs.Domain.Audit;
using BytLabs.Domain.DynamicData;

namespace BytLabs.MicroserviceTemplate.Application.Products.Dtos
{
    // RECIPE: DTO that exposes dynamic data (IHaveDynamicData) and audit info. A plain get/set POCO (with a
    // parameterless ctor) so HotChocolate's queryable projection can member-init it. Used for AutoMapper
    // (command results) and MongoDB projection (As<ProductDto>) in queries.
    public class ProductDto : IAuditable, IHaveDynamicData
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public JsonElement Data { get; set; }
        public IReadOnlyCollection<ProductVariantDto> Variants { get; set; } = new List<ProductVariantDto>();
        public AuditInfo AuditInfo { get; set; } = default!;
    }
}
