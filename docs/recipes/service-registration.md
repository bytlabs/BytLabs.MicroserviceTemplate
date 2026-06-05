# Service registration

**What it is.** A single `AddInfrastructure(...)` extension that composes all dependency injection:
CQS/MediatR, AutoMapper, the MongoDB database + repositories, class maps, and custom services.

**When to use it.** As the one wiring entry point called from `Program.cs`; add new aggregates and
services here.

**How it works.** `AddCQS(assembly)` registers commands/handlers/event-handlers; `AddAutoMapper`
registers profiles; `AddMongoDatabase(config).AddMongoRepository<TAgg,TId>()` wires persistence;
class-map helpers register value-object/serializer mappings.

```csharp
services.AddCQS(new[] { typeof(CreateOrderCommand).Assembly });
services.AddAutoMapper(typeof(OrderMappingProfile), typeof(ProductMappingProfile));
services.AddMongoDatabase(mongoConfig)
    .RegisterMongoDBClassMaps()
    .RegisterDynamicDataClassMaps()
    .AddMongoRepository<Order, Guid>()
    .AddMongoRepository<Product, Guid>();
```

**Sample code in this template.**
- [`Infrastructure/ServiceExtensions.cs`](../../src/BytLabs.MicroserviceTemplate.Infrastructure/ServiceExtensions.cs)

**Reference (BytLabs.BackendPackages).** `BytLabs.Application.AddCQS`, `BytLabs.DataAccess.MongoDB` (`AddMongoDatabase`, `AddMongoRepository`).

**Related recipes.** [BSON class maps](bson-class-maps.md), [Custom BSON serializer](custom-bson-serializer.md), [AutoMapper profiles](automapper-profiles.md).
