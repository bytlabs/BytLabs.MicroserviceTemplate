# Specialized update methods

**What it is.** Exposing intent-revealing methods on an aggregate (`Update`, `AddVariant`, `Remove`)
instead of a single generic setter or a public property soup.

**When to use it.** Always — each method names a business operation, enforces its own invariants, and
raises the matching domain event, keeping the aggregate's API meaningful.

**How it works.** The aggregate keeps private setters and offers one method per operation. Different
concerns get different methods (e.g. updating details vs. adding a sub-entity).

```csharp
public void Update(UpdateProduct value) { /* details + dynamic data */ }
public void AddVariant(AddVariant value) { /* append a sub-entity */ }
```

**Sample code in this template.**
- [`Product.cs`](../../src/BytLabs.MicroserviceTemplate.Domain/Aggregates/ProductAggregate/Product.cs) — `Update`, `AddVariant`, `RemoveVariant`, `Remove`
- [`EntityDef.cs`](../../src/BytLabs.MicroserviceTemplate.Domain/Aggregates/EntityDefAggregate/EntityDef.cs) — `Update` replaces form + table schemas

**Reference (BytLabs.BackendPackages).** n/a (a DDD aggregate-design convention).

**Related recipes.** [Aggregate root](aggregate-root.md), [Sub-entity add/remove commands](sub-entity-commands.md).
