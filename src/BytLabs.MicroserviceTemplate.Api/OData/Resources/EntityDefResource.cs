namespace BytLabs.MicroserviceTemplate.Api.OData.Resources;

// Form/Table schemas are surfaced as raw JSON strings for the same reason as dynamic Data.
public class EntityDefResource
{
    public Guid Id { get; set; }
    public string EntityType { get; set; } = default!;
    public string Form { get; set; } = "{}";
    public string Table { get; set; } = "{}";
    public DateTime? CreatedAt { get; set; }
    public string? CreatedBy { get; set; }
}
