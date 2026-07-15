namespace BytLabs.MicroserviceTemplate.Api.OData.Resources;

public class ProductResource
{
    public Guid Id { get; set; }
    public string Name { get; set; } = default!;
    public string Data { get; set; } = "{}";
    public List<ProductVariantResource> Variants { get; set; } = new();
    public DateTime? CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
}

public class ProductVariantResource
{
    public Guid Id { get; set; }
    public string Sku { get; set; } = default!;
    public decimal Price { get; set; }
}
