using BytLabs.MicroserviceTemplate.Domain.Aggregates.OrderAggregate;

namespace BytLabs.MicroserviceTemplate.Application.Dtos
{
    public class OrderDto : AuditInfoDto
    {
        public Guid Id { get; set; }
        public DateTime OrderDate { get; set; }
        public OrderStatus Status { get; set; }
        public ICollection<OrderItemDto> Items { get; set; } = new List<OrderItemDto>();
    }
}
