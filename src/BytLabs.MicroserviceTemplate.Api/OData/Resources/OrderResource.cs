namespace BytLabs.MicroserviceTemplate.Api.OData.Resources;

// OData read-projection of Order. Dynamic data is a raw JSON string (OData/EDM cannot serialize
// System.Text.Json.JsonElement). Items is an inline complex-type collection.
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
