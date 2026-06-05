# Dynamic data (schema-less JSON)

**What it is.** A way for an aggregate to carry arbitrary, caller-defined fields in a single JSON
property (`JsonElement Data`) without changing the C# model or the database schema. The aggregate
implements `IHaveDynamicData`.

**When to use it.** When tenants/clients need custom attributes you cannot model up front (e.g.
product specs that differ per category), or when you want a stable core schema plus an extensible bag.

**How it works.**
- The aggregate exposes `JsonElement Data` and implements `BytLabs.Domain.DynamicData.IHaveDynamicData`.
- On update, new data is **merged** into the existing data (only supplied keys change) via the
  `JsonElement.Merge` helper, instead of being overwritten wholesale.
- GraphQL can filter on the dynamic field — see [Dynamic-data query](graphql-dynamic-data-query.md).
- MongoDB stores it as a native sub-document, so it is queryable.

```csharp
public void Update(UpdateProduct value)
{
    Name = value.Name;
    Data = Data.Merge(value.Data); // merge, not replace
    AddDomainEvent(new ProductUpdated(Id, value));
}
```

**Sample code in this template.**
- [`Product.cs`](../../src/BytLabs.MicroserviceTemplate.Domain/Aggregates/ProductAggregate/Product.cs) — `Data` property + merge on update
- [`JsonElementExtensions.cs`](../../src/BytLabs.MicroserviceTemplate.Domain/Utils/JsonElementExtensions.cs) — the `Merge` helper

**Reference (BytLabs.BackendPackages).** `BytLabs.Domain.DynamicData.IHaveDynamicData`.

**Related recipes.** [Soft delete](soft-delete.md), [Schema value objects](schema-value-objects.md), [Dynamic-data query](graphql-dynamic-data-query.md).
