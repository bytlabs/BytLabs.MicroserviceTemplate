# Custom BSON serializer

**What it is.** A `SerializerBase<T>` implementation for a type MongoDB can't map out of the box —
here `IReadOnlySet<T>`, used for a product's variant collection.

**When to use it.** For unusual collection or value types whose default serialization is missing or
wrong. Register it once with `BsonSerializer.TryRegisterSerializer`.

**How it works.** Implement `Serialize`/`Deserialize` to read/write a BSON array, then register the
serializer during infrastructure setup before any document is mapped.

```csharp
public class IReadOnlySetSerializer<T> : SerializerBase<IReadOnlySet<T>>
{
    public override void Serialize(BsonSerializationContext ctx, BsonSerializationArgs args, IReadOnlySet<T> value)
    { ctx.Writer.WriteStartArray(); foreach (var i in value) BsonSerializer.Serialize(ctx.Writer, i); ctx.Writer.WriteEndArray(); }

    public override IReadOnlySet<T> Deserialize(BsonDeserializationContext ctx, BsonDeserializationArgs args)
        => BsonSerializer.Deserialize<IEnumerable<T>>(ctx.Reader).ToHashSet();
}
```

**Sample code in this template.**
- [`Infrastructure/MongoDB/IReadOnlySetSerializer.cs`](../../src/BytLabs.MicroserviceTemplate.Infrastructure/MongoDB/IReadOnlySetSerializer.cs)
- [`Infrastructure/ServiceExtensions.cs`](../../src/BytLabs.MicroserviceTemplate.Infrastructure/ServiceExtensions.cs) — `BsonSerializer.TryRegisterSerializer(new IReadOnlySetSerializer<ProductVariant>())`

**Reference (BytLabs.BackendPackages).** `MongoDB.Bson.Serialization.SerializerBase<T>`.

**Related recipes.** [Sub-entity](sub-entity.md), [BSON class maps](bson-class-maps.md).
