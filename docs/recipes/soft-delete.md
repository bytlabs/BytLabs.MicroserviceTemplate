# Soft delete

**What it is.** "Deleting" a record by flagging it (`IsDeleted = true`) instead of physically
removing it, so it can be excluded from queries while remaining recoverable and auditable. The
aggregate implements `ISoftDeletable`.

**When to use it.** Almost always for business entities — to support undo, audit history, and
referential safety. Avoid only for high-volume ephemeral data.

**How it works.**
- The aggregate implements `BytLabs.Domain.Entities.ISoftDeletable` (`bool IsDeleted`) and exposes a
  `Remove()` method that sets the flag and raises a domain event.
- The `RemoveProduct` command calls `Remove()` and persists via `UpdateAsync` (not delete).
- Queries call the package helper `ExcludeSoftDeletedEntites()` so deleted rows never surface.

```csharp
public void Remove()
{
    IsDeleted = true;
    AddDomainEvent(new ProductRemoved(Id));
}
```

**Sample code in this template.**
- [`Product.cs`](../../src/BytLabs.MicroserviceTemplate.Domain/Aggregates/ProductAggregate/Product.cs) — `IsDeleted` + `Remove()`
- [`RemoveProductCommandHandler.cs`](../../src/BytLabs.MicroserviceTemplate.Application/Commands/RemoveProduct/RemoveProductCommandHandler.cs) — soft delete via update
- [`ProductQueries.cs`](../../src/BytLabs.MicroserviceTemplate.Api/Graphql/Queries/ProductQueries.cs) — `.ExcludeSoftDeletedEntites()`

**Reference (BytLabs.BackendPackages).** `BytLabs.Domain.Entities.ISoftDeletable`, `ExcludeSoftDeletedEntites()` (in `BytLabs.DataAccess.MongoDB.DynamicData`).

**Related recipes.** [Dynamic data](dynamic-data.md), [Dynamic-data query](graphql-dynamic-data-query.md).
