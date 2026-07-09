using System.Text.Json;
using BytLabs.Domain.DynamicData;
using BytLabs.Domain.Entities;
using BytLabs.MicroserviceTemplate.Domain.Aggregates.OrderAggregate.DataObjects;
using BytLabs.MicroserviceTemplate.Domain.Aggregates.OrderAggregate.Events;
using BytLabs.MicroserviceTemplate.Domain.Utils;

namespace BytLabs.MicroserviceTemplate.Domain.Aggregates.OrderAggregate
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

        // Constructor parameter names/types match the members so MongoDB can configure the
        // creator automatically when deserializing (see Product for the same convention).
        public Order(Guid id, DateTime orderDate, IReadOnlyCollection<OrderItem> items, JsonElement data) : base(id)
        {
            Id = id;
            OrderDate = orderDate;
            Status = OrderStatus.Pending;
            Items = items.ToList();
            Data = data;

            AddDomainEvent(new OrderCreatedEvent(Id));
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
