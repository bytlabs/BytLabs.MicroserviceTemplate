# Specialized update methods

**What it is.** Exposing intent-revealing methods on an aggregate (`Update`, `UpdateAttributesSchema`,
`AddVariant`, `Remove`) instead of a single generic setter or a public property soup.

**When to use it.** Always — each method names a business operation, enforces its own invariants, and
raises the matching domain event, keeping the aggregate's API meaningful.

**How it works.** The aggregate keeps private setters and offers one method per operation. Different
concerns get different methods (e.g. updating details vs. replacing the attributes schema).

```csharp
public void Update(UpdateProduct value) { /* details + dynamic data */ }
public void UpdateAttributesSchema(FormDataSchema schema) { /* schema only */ }
```

**Sample code in this template.**
- [`Product.cs`](../../src/BytLabs.MicroserviceTemplate.Domain/Aggregates/ProductAggregate/Product.cs) — `Update`, `UpdateAttributesSchema`, `AddVariant`, `RemoveVariant`, `Remove`

**Reference (BytLabs.BackendPackages).** n/a (a DDD aggregate-design convention).

**Related recipes.** [Aggregate root](aggregate-root.md), [Sub-entity add/remove commands](sub-entity-commands.md).
