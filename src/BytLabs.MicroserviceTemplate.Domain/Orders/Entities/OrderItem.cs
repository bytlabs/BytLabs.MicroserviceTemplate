using BytLabs.Domain.Entities;

namespace BytLabs.MicroserviceTemplate.Domain.Orders.Entities
{
    public class OrderItem : Entity<Guid>
    {
        public Guid ProductId { get; private set; }
        public int Quantity { get; private set; }
        public decimal Price { get; private set; }

        // Id-accepting ctor: used by GraphQL input binding and by the EF jsonb round-trip so the
        // item's identity is preserved. Id generation lives in Create (matches ProductVariant/aggregates).
        public OrderItem(Guid id, Guid productId, int quantity, decimal price) : base(id)
        {
            ProductId = productId;
            Quantity = quantity;
            Price = price;
        }

        public static OrderItem Create(Guid productId, int quantity, decimal price)
            => new(Guid.NewGuid(), productId, quantity, price);
    }

}
