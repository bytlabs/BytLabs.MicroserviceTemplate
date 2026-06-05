using BytLabs.Domain.Entities;
using BytLabs.MicroserviceTemplate.Domain.Aggregates.ProductAggregate.DataObjects;

namespace BytLabs.MicroserviceTemplate.Domain.Aggregates.ProductAggregate
{
    // RECIPE: Sub-entity inside an aggregate. ProductVariant is owned by Product (it is not an
    // aggregate root). Created via a static factory from a CreateVariant data object.
    public sealed class ProductVariant : Entity<Guid>
    {
        public string Sku { get; private set; }
        public decimal Price { get; private set; }

        private ProductVariant(Guid id, string sku, decimal price) : base(id)
        {
            Sku = sku;
            Price = price;
        }

        public static ProductVariant Create(CreateVariant details)
            => new(details.Id, details.Sku, details.Price);
    }
}
