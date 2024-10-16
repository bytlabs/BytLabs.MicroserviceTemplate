using BytLabs.Domain.Entities;
using BytLabs.Domain.Exceptions;
using BytLabs.MicroserviceTemplate.Domain.Aggregates.OrderAggregate.Events;

namespace BytLabs.MicroserviceTemplate.Domain.Aggregates.OrderAggregate
{

    public class Order : AggregateRootBase<Guid>
    {
        public DateTime OrderDate { get; private set; }
        public OrderStatus Status { get; private set; }
        public IReadOnlyCollection<OrderItem> Items { get; private set; }

        public Order(Guid id, DateTime orderDate, IEnumerable<OrderItem> items) : base(id)
        {
            if (!items.Any())
                throw new DomainException("An order must have at least one item.");

            Id = id;
            OrderDate = orderDate;
            Status = OrderStatus.Pending;
            Items = items.ToList();

            AddDomainEvent(new OrderCreatedEvent(Id));
        }

        public void MarkAsShipped()
        {
            if (Status != OrderStatus.Pending)
                throw new DomainException("Only pending orders can be marked as shipped.");

            Status = OrderStatus.Shipped;

            AddDomainEvent(new OrderShippedEvent(Id));
        }
    }


}
