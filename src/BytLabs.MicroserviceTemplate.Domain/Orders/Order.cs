using System.Text.Json;
using BytLabs.Domain.DynamicData;
using BytLabs.Domain.Entities;
using BytLabs.MicroserviceTemplate.Domain.Orders.DataObjects;
using BytLabs.MicroserviceTemplate.Domain.Orders.Events;
using BytLabs.MicroserviceTemplate.Domain.Common.Utils;

namespace BytLabs.MicroserviceTemplate.Domain.Orders
{
    // A flat dynamic entity: structured fields (OrderDate/Status/Items) plus a schema-less `Data`
    // JSON (IHaveDynamicData) rendered by an EntityDef. Soft-deletable, with create/update/remove.
    public class Order : AggregateRootBase<Guid>, IHaveDynamicData, ISoftDeletable
    {
        public DateTime OrderDate { get; private set; }
        public OrderStatus Status { get; private set; }
        public IReadOnlyCollection<OrderItem> Items { get; private set; }
        public bool IsEmailSent { get; private set; }
        public JsonElement Data { get; private set; }
        public bool IsDeleted { get; private set; }

        // IMPORTANT: the constructor MUST stay side-effect free. MongoDB reconstructs the aggregate by
        // calling this constructor on every load, so raising a domain event here would re-fire
        // OrderCreatedEvent on every read (see Order.Create). Parameter names/types match the members
        // so the Mongo creator can be configured automatically.
        public Order(Guid id, DateTime orderDate, IReadOnlyCollection<OrderItem> items, JsonElement data) : base(id)
        {
            Id = id;
            OrderDate = orderDate;
            Status = OrderStatus.Pending;
            Items = items.ToList();
            Data = data;
        }

        // Factory: the ONLY place OrderCreatedEvent is raised (not the constructor).
        public static Order Create(Guid id, DateTime orderDate, IReadOnlyCollection<OrderItem> items, JsonElement data)
        {
            var order = new Order(id, orderDate, items, data);
            order.AddDomainEvent(new OrderCreatedEvent(id));
            return order;
        }

        public void MarkAsShipped()
        {
            if (Status != OrderStatus.Pending)
                throw new BytLabs.Domain.Exceptions.DomainException("Only pending orders can be marked as shipped.");

            Status = OrderStatus.Shipped;
            AddDomainEvent(new OrderShippedEvent(Id));
        }

        public void MarkAsEmailSent()
        {
            IsEmailSent = true;
        }

        // Update: merge dynamic data (not replace), matching Product's convention.
        public void Update(UpdateOrder value)
        {
            Data = Data.Merge(value.Data);
            if (value.Items is not null) Items = value.Items.ToList();
            AddDomainEvent(new OrderUpdated(Id, value));
        }

        // Soft delete: flag, don't remove. Queries exclude these via ExcludeSoftDeletedEntites().
        public void Remove()
        {
            IsDeleted = true;
            AddDomainEvent(new OrderRemoved(Id));
        }
    }
}
