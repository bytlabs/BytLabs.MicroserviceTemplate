using Microsoft.OData.Client;

namespace BytLabs.MicroserviceTemplate.Client.Rest;

// Client-side entity types for the REST/OData API, kept in a separate namespace from the
// StrawberryShake GraphQL client. Dynamic data / schemas are raw JSON strings (as the API exposes them).

[Key(nameof(Id))]
public class OrderResource
{
    public Guid Id { get; set; }
    public DateTime OrderDate { get; set; }
    public string Status { get; set; } = default!;
    public string Data { get; set; } = "{}";
    public List<OrderItemResource> Items { get; set; } = new();
    public DateTime? CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
}

public class OrderItemResource
{
    public Guid Id { get; set; }
    public Guid ProductId { get; set; }
    public int Quantity { get; set; }
    public decimal Price { get; set; }
}

[Key(nameof(Id))]
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

[Key(nameof(Id))]
public class EntityDefResource
{
    public Guid Id { get; set; }
    public string EntityType { get; set; } = default!;
    public string Form { get; set; } = "{}";
    public string Table { get; set; } = "{}";
    public DateTime? CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
}
