# Dynamic-data query + soft-delete filter

**What it is.** An advanced GraphQL query that, before projecting, excludes soft-deleted rows and
applies filtering/sorting over the schema-less `Data` field.

**When to use it.** For querying aggregates that use [dynamic data](dynamic-data.md) and
[soft delete](soft-delete.md) — i.e. the `Product` aggregate here.

**How it works.** The resolver chains package helpers on the Mongo aggregate:
`.ExcludeSoftDeletedEntites()` → `.ApplyDynamicDataFilteration(context)` → `.AppySortingWithDynamicData(order)`
→ `.Project(... As<ProductDto>())`. Dynamic-data input types are registered via `AddDynamicDataTypes()`,
and the aggregate filter/sort types via `AddAggregateFilterType`/`AddAggregateSortType`.

```csharp
return db.GetCollection<Product>().Aggregate()
    .ExcludeSoftDeletedEntites()
    .ApplyDynamicDataFilteration(context)
    .AppySortingWithDynamicData(order)
    .Project(Builders<Product>.Projection.As<ProductDto>())
    .AsExecutable();
```

**Sample code in this template.**
- [`Graphql/Queries/ProductQueries.cs`](../../src/BytLabs.MicroserviceTemplate.Api/Graphql/Queries/ProductQueries.cs)
- [`Api/Utils/IAggregateFluentExtensions.cs`](../../src/BytLabs.MicroserviceTemplate.Api/Utils/IAggregateFluentExtensions.cs) — `ApplyDynamicDataFilteration(context)`

**Reference (BytLabs.BackendPackages).** `BytLabs.DataAccess.MongoDB.DynamicData` (`ExcludeSoftDeletedEntites`, `ApplyDynamicDataFilteration`, `AppySortingWithDynamicData`), `AddDynamicDataTypes`.

**Related recipes.** [Dynamic data](dynamic-data.md), [Soft delete](soft-delete.md), [GraphQL type registration](graphql-type-registration.md).
