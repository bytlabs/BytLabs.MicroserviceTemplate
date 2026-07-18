namespace BytLabs.MicroserviceTemplate.Application.Products.Dtos
{
    public class ProductVariantDto
    {
        public Guid Id { get; set; }
        public string Sku { get; set; } = string.Empty;
        public decimal Price { get; set; }
    }
}
