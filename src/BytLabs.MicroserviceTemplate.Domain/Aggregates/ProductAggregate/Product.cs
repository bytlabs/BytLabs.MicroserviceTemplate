using System.Text.Json;
using BytLabs.Domain.DynamicData;
using BytLabs.Domain.Entities;
using BytLabs.MicroserviceTemplate.Domain.Aggregates.ProductAggregate.DataObjects;
using BytLabs.MicroserviceTemplate.Domain.Aggregates.ProductAggregate.Events;
using BytLabs.MicroserviceTemplate.Domain.Utils;

namespace BytLabs.MicroserviceTemplate.Domain.Aggregates.ProductAggregate
{
    // RECIPE HUB: dynamic data (IHaveDynamicData) + soft-delete (ISoftDeletable) +
    // schema value object + sub-entity set + specialized update methods + domain events.
    public sealed class Product : AggregateRootBase<Guid>, IHaveDynamicData, ISoftDeletable
    {
        public string Name { get; private set; }
        public JsonElement Data { get; private set; }
        public IReadOnlySet<ProductVariant> Variants { get; private set; }
        public bool IsDeleted { get; private set; }

        private Product(Guid id, string name, JsonElement data) : base(id)
        {
            Name = name;
            Data = data;
            Variants = new HashSet<ProductVariant>();
        }

        public static Product Create(CreateProduct details)
        {
            var product = new Product(details.Id, details.Name, details.Data);
            product.AddDomainEvent(new ProductCreated(product.Id, details));
            return product;
        }

        // Specialized update: details + dynamic data (merge, not replace).
        public void Update(UpdateProduct value)
        {
            Name = value.Name;
            Data = Data.Merge(value.Data);
            AddDomainEvent(new ProductUpdated(Id, value));
        }

        public void AddVariant(AddVariant value)
        {
            Variants = Variants.Concat([value.Variant]).ToHashSet();
            AddDomainEvent(new ProductVariantAdded(Id, value));
        }

        public void RemoveVariant(RemoveVariant value)
        {
            Variants = Variants.Where(v => v.Id != value.VariantId).ToHashSet();
            AddDomainEvent(new ProductVariantRemoved(Id, value));
        }

        // Soft delete: flag, don't remove. Queries exclude these via ExcludeSoftDeletedEntites().
        public void Remove()
        {
            IsDeleted = true;
            AddDomainEvent(new ProductRemoved(Id));
        }
    }
}
