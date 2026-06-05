# Data objects (Create/Update)

**What it is.** Small `record` types that carry parameters into aggregate factories and mutators,
decoupling the domain API from command/GraphQL input shapes.

**When to use it.** Whenever an aggregate factory or method needs several values — pass a data object
instead of a long parameter list, keeping the signature stable as fields evolve.

**How it works.** The Domain layer defines `CreateX`/`UpdateX`/`AddY`/`RemoveY` records; aggregate
methods accept them and (for create/update) embed them in the corresponding domain event.

```csharp
public record CreateProduct(Guid Id, string Name, JsonElement Data, FormDataSchema AttributesSchema);
```

**Sample code in this template.**
- [`DataObjects/`](../../src/BytLabs.MicroserviceTemplate.Domain/Aggregates/ProductAggregate/DataObjects) — `CreateProduct`, `UpdateProduct`, `AddVariant`, `RemoveVariant`, `CreateVariant`

**Reference (BytLabs.BackendPackages).** n/a (a plain DDD convention).

**Related recipes.** [Command + handler](cqrs-command-handler.md), [Typed domain events](domain-events.md).
