namespace BytLabs.MicroserviceTemplate.Application.Dtos
{
    public class OrderItemDto : AuditInfoDto
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
}
