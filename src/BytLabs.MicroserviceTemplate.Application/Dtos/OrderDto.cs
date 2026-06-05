using BytLabs.Domain.Audit;
using BytLabs.MicroserviceTemplate.Domain.Aggregates.OrderAggregate;

namespace BytLabs.MicroserviceTemplate.Application.Dtos
{
    // RECIPE: DTO with audit info. 4.x exposes audit data via IAuditable's nested AuditInfo
    // (CreatedAt/CreatedBy/LastModifiedAt/...), matching how aggregates store it.
    public class OrderDto : IAuditable
    {
        public Guid Id { get; set; }
        public DateTime OrderDate { get; set; }
        public OrderStatus Status { get; set; }
        public ICollection<OrderItemDto> Items { get; set; } = new List<OrderItemDto>();
        public AuditInfo AuditInfo { get; set; } = new AuditInfo();
    }
}
