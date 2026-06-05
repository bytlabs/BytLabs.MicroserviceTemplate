# Query with paging/projection/filtering/sorting

**What it is.** A GraphQL read endpoint defined as a resolver on the partial `Query` class that
returns an `IExecutable<TDto>` from a MongoDB aggregate, decorated with HotChocolate middleware
attributes for cursor paging, projection, filtering and sorting.

**When to use it.** For list/collection endpoints. Single-item reads can use a filtered version of
the same query.

**How it works.** `[UsePaging][UseProjection][UseFiltering(Type=typeof(Order))][UseSorting(Type=typeof(Order))]`
wrap a Mongo aggregate that projects to the DTO with `As<OrderDto>`. The Mongo-aware query
conventions are enabled once via `AddMongoDbQuerySettings()`.

```csharp
[UsePaging, UseProjection, UseSorting(Type = typeof(Order)), UseFiltering(Type = typeof(Order))]
public IExecutable<OrderDto> GetOrders([Service] IMongoDatabase db)
    => db.GetCollection<Order>().Aggregate()
         .Project(Builders<Order>.Projection.As<OrderDto>())
         .AsExecutable();
```

**Sample code in this template.**
- [`Graphql/Queries/OrderQueries.cs`](../../src/BytLabs.MicroserviceTemplate.Api/Graphql/Queries/OrderQueries.cs)
- [`Api/Utils/IRequestExecutorBuilderExtensions.cs`](../../src/BytLabs.MicroserviceTemplate.Api/Utils/IRequestExecutorBuilderExtensions.cs) — `AddMongoDbQuerySettings`

**Reference (BytLabs.BackendPackages).** HotChocolate + `HotChocolate.Data.MongoDb`.

**Related recipes.** [Dynamic-data query](graphql-dynamic-data-query.md), [GraphQL type registration](graphql-type-registration.md), [DTOs](dtos.md).
