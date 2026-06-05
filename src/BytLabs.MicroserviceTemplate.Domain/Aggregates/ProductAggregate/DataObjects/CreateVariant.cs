namespace BytLabs.MicroserviceTemplate.Domain.Aggregates.ProductAggregate.DataObjects
{
    public record CreateVariant(Guid Id, string Sku, decimal Price);
}
