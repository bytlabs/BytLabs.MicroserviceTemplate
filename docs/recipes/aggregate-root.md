# Aggregate root & domain events

**What it is.** An aggregate root is a consistency boundary with its own identity that raises domain
events when its state changes. It derives from `AggregateRootBase<TId>` and calls `AddDomainEvent(...)`.

**When to use it.** For every top-level entity that is loaded, mutated, and saved as a unit.

**How it works.** The root extends `AggregateRootBase<Guid>`; factory methods and mutators change
state and append domain events, which are dispatched after the aggregate is persisted.

```csharp
public static Product Create(CreateProduct details)
{
    var product = new Product(details.Id, details.Name, details.Data);
    product.AddDomainEvent(new ProductCreated(product.Id, details));
    return product;
}
```

**Sample code in this template.**
- [`Order.cs`](../../src/BytLabs.MicroserviceTemplate.Domain/Aggregates/OrderAggregate/Order.cs) — minimal aggregate
- [`Product.cs`](../../src/BytLabs.MicroserviceTemplate.Domain/Aggregates/ProductAggregate/Product.cs) — advanced aggregate

**Reference (BytLabs.BackendPackages).** `BytLabs.Domain.Entities.AggregateRootBase<TId>`.

**Related recipes.** [Typed domain events](domain-events.md), [Soft delete](soft-delete.md), [Sub-entity](sub-entity.md).
