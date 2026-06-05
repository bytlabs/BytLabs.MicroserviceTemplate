using System.Text.Json;
using BytLabs.Domain.DynamicData;
using BytLabs.Domain.Entities;
using BytLabs.MicroserviceTemplate.Domain.Aggregates.ProductAggregate.DataObjects;
using BytLabs.MicroserviceTemplate.Domain.Aggregates.ProductAggregate.Events;
using BytLabs.MicroserviceTemplate.Domain.Shared.DynamicData;
using BytLabs.MicroserviceTemplate.Domain.Utils;

namespace BytLabs.MicroserviceTemplate.Domain.Aggregates.ProductAggregate
{
    // RECIPE HUB: dynamic data (IHaveDynamicData) + soft-delete (ISoftDeletable) +
    // schema value object + sub-entity set + specialized update methods + domain events.
    public sealed class Product : AggregateRootBase<Guid>, IHaveDynamicData, ISoftDeletable
    {
        public string Name { get; private set; }
        public JsonElement Data { get; private set; }
        public FormDataSchema AttributesSchema { get; private set; }
        public IReadOnlySet<ProductVariant> Variants { get; private set; }
        public bool IsDeleted { get; private set; }

        private Product(Guid id, string name, JsonElement data, FormDataSchema attributesSchema) : base(id)
        {
            Name = name;
            Data = data;
            AttributesSchema = attributesSchema;
            Variants = new HashSet<ProductVariant>();
        }

        public static Product Create(CreateProduct details)
        {
            var product = new Product(details.Id, details.Name, details.Data, details.AttributesSchema);
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

        // Specialized update: the attributes schema only.
        public void UpdateAttributesSchema(FormDataSchema schema)
        {
            AttributesSchema = schema;
            AddDomainEvent(new ProductAttributesSchemaUpdated(Id, schema));
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
