using System.Text.Json;
using BytLabs.Domain.DynamicData;
using BytLabs.Domain.Entities;
using BytLabs.MicroserviceTemplate.Domain.Products.DataObjects;
using BytLabs.MicroserviceTemplate.Domain.Products.Events;
using BytLabs.MicroserviceTemplate.Domain.Common.Utils;

namespace BytLabs.MicroserviceTemplate.Domain.Products
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
            if (details.Variants is not null) product.Variants = details.Variants.Select(ToVariant).ToHashSet();
            return product;
        }

        // Specialized update: details + dynamic data (merge, not replace).
        public void Update(UpdateProduct value)
        {
            Name = value.Name;
            Data = Data.Merge(value.Data);
            if (value.Variants is not null) Variants = value.Variants.Select(ToVariant).ToHashSet();
            AddDomainEvent(new ProductUpdated(Id, value));
        }

        private static ProductVariant ToVariant(VariantData v) => ProductVariant.Create(new CreateVariant(Guid.NewGuid(), v.Sku, v.Price));


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
