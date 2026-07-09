# DTOs

**What it is.** Read-side data shapes returned by the API, decoupled from the domain aggregate. They
may implement `IAuditable` (nested `AuditInfo`) and `IHaveDynamicData` (a `JsonElement Data`).

**When to use it.** For every API response. Never return aggregates directly — DTOs keep the contract
stable and projection-friendly.

**How it works.** A DTO is a `record` (or class) whose property names match the stored document, so it
works both with AutoMapper (command results) and MongoDB `As<TDto>` projection (queries).

```csharp
public record ProductDto(
    Guid Id, string Name, JsonElement Data,
    IReadOnlyCollection<ProductVariantDto> Variants, AuditInfo AuditInfo)
    : IAuditable, IHaveDynamicData;
```

**Sample code in this template.**
- [`Dtos/ProductDto.cs`](../../src/BytLabs.MicroserviceTemplate.Application/Dtos/ProductDto.cs)
- [`Dtos/OrderDto.cs`](../../src/BytLabs.MicroserviceTemplate.Application/Dtos/OrderDto.cs)
- [`Dtos/ProductVariantDto.cs`](../../src/BytLabs.MicroserviceTemplate.Application/Dtos/ProductVariantDto.cs)

**Reference (BytLabs.BackendPackages).** `BytLabs.Domain.Audit.IAuditable` / `AuditInfo`, `BytLabs.Domain.DynamicData.IHaveDynamicData`.

**Related recipes.** [AutoMapper profiles](automapper-profiles.md), [GraphQL query](graphql-query.md).
