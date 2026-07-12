# MongoDB BSON class maps

**What it is.** Explicit `BsonClassMap` registrations that control how entities and value objects
serialize/deserialize — configuring creators, member serializers, and defaults.

**When to use it.** For value objects and any type MongoDB can't auto-map reliably (e.g. a constructor
whose parameter types don't match the members, or a member needing a specific serializer).

**How it works.** `BsonClassMap.TryRegisterClassMap<T>(cm => { cm.AutoMap(); ... })`. Use
`MapCreator` for constructor-based immutables, `SetDefaultValue` for tolerant deserialization of older
documents, and `SetSerializer` for member-level control (e.g. store a `Guid` as a string).

```csharp
BsonClassMap.TryRegisterClassMap<FormDataSchema>(cm =>
{
    cm.AutoMap();
    cm.MapMember(c => c.Key).SetDefaultValue(() => string.Empty);
    cm.MapCreator(v => new FormDataSchema(v.Key, v.SampleData, v.FormSchema, v.FormUi));
});
```

> Tip: constructor parameter names **and types** should match the members so MongoDB can auto-configure
> the creator (see how `Order`/`Product` constructors mirror their properties).

**Sample code in this template.**
- [`Infrastructure/ServiceExtensions.cs`](../../src/BytLabs.MicroserviceTemplate.Infrastructure/ServiceExtensions.cs) — `RegisterMongoDBClassMaps` (OrderItem, ProductVariant)
- [`Infrastructure/MongoDB/DynamicDataExtensions.cs`](../../src/BytLabs.MicroserviceTemplate.Infrastructure/MongoDB/DynamicDataExtensions.cs) — schema value objects

**Reference (BytLabs.BackendPackages).** `MongoDB.Bson.Serialization.BsonClassMap`.

**Related recipes.** [Schema value objects](schema-value-objects.md), [Custom BSON serializer](custom-bson-serializer.md), [Service registration](service-registration.md).
