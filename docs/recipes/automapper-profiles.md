# AutoMapper profiles

**What it is.** `Profile` classes that configure how domain aggregates map to DTOs, registered with
`AddAutoMapper`.

**When to use it.** For command handlers that return a DTO mapped from the mutated aggregate. (Query
resolvers use MongoDB `As<TDto>` projection instead, so they don't need AutoMapper.)

**How it works.** Each profile declares `CreateMap<Aggregate, Dto>()`. Profiles are registered by
marker type in `AddInfrastructure`.

```csharp
public class ProductMappingProfile : Profile
{
    public ProductMappingProfile()
    {
        CreateMap<Product, ProductDto>();
        CreateMap<ProductVariant, ProductVariantDto>();
    }
}
```

**Sample code in this template.**
- [`Products/Mapping/ProductMappingProfile.cs`](../../src/BytLabs.MicroserviceTemplate.Application/Products/Mapping/ProductMappingProfile.cs)
- [`Orders/Mapping/OrderMappingProfile.cs`](../../src/BytLabs.MicroserviceTemplate.Application/Orders/Mapping/OrderMappingProfile.cs)
- [`ServiceExtensions.cs`](../../src/BytLabs.MicroserviceTemplate.Infrastructure/ServiceExtensions.cs) — `AddAutoMapper(...)`

**Reference (BytLabs.BackendPackages).** AutoMapper (third-party).

**Related recipes.** [DTOs](dtos.md), [Service registration](service-registration.md).
