using System.Text.Json;
using BytLabs.Domain.Audit;
using BytLabs.Domain.DynamicData;
using BytLabs.MicroserviceTemplate.Domain.Aggregates.OrderAggregate;

namespace BytLabs.MicroserviceTemplate.Application.Dtos
{
    // RECIPE: DTO with audit info + dynamic data. 4.x exposes audit data via IAuditable's nested
    // AuditInfo, and dynamic fields via IHaveDynamicData's JsonElement Data.
    public class OrderDto : IAuditable, IHaveDynamicData
    {
        public Guid Id { get; set; }
        public DateTime OrderDate { get; set; }
        public OrderStatus Status { get; set; }
        public ICollection<OrderItemDto> Items { get; set; } = new List<OrderItemDto>();
        public JsonElement Data { get; set; }
        public AuditInfo AuditInfo { get; set; } = new AuditInfo();
    }
}
