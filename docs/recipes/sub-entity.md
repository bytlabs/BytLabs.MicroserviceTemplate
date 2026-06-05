# Sub-entity inside an aggregate

**What it is.** A child entity owned by an aggregate root, with no independent lifecycle or
repository. It derives from `Entity<TId>` and is created through a static factory.

**When to use it.** For parts that only exist within a root (an order's line items, a product's
variants) and must be saved/loaded together with the root.

**How it works.** The root owns a collection of the sub-entity and exposes methods to add/remove
members; the sub-entity has private setters and a `Create` factory.

```csharp
public sealed class ProductVariant : Entity<Guid>
{
    public string Sku { get; private set; }
    public decimal Price { get; private set; }
    private ProductVariant(Guid id, string sku, decimal price) : base(id) { Sku = sku; Price = price; }
    public static ProductVariant Create(CreateVariant d) => new(d.Id, d.Sku, d.Price);
}
```

**Sample code in this template.**
- [`ProductVariant.cs`](../../src/BytLabs.MicroserviceTemplate.Domain/Aggregates/ProductAggregate/ProductVariant.cs) — sub-entity
- [`OrderItem.cs`](../../src/BytLabs.MicroserviceTemplate.Domain/Aggregates/OrderAggregate/OrderItem.cs) — sub-entity
- [`Product.cs`](../../src/BytLabs.MicroserviceTemplate.Domain/Aggregates/ProductAggregate/Product.cs) — owns the `Variants` set

**Reference (BytLabs.BackendPackages).** `BytLabs.Domain.Entities.Entity<TId>`.

**Related recipes.** [Sub-entity add/remove commands](sub-entity-commands.md), [Custom BSON serializer](custom-bson-serializer.md).
