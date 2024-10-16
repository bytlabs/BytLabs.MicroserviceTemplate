using BytLabs.Domain.Entities;

namespace BytLabs.MicroserviceTemplate.Domain.Aggregates.OrderAggregate
{
    public class OrderItem : Entity<Guid>
    {
        public Guid ProductId { get; private set; }
        public int Quantity { get; private set; }
        public decimal Price { get; private set; }

        public OrderItem(Guid productId, int quantity, decimal price) : base(Guid.NewGuid())
        {
            ProductId = productId;
            Quantity = quantity;
            Price = price;
        }
    }

}
