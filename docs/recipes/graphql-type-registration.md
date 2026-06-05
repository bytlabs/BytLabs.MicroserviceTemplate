# GraphQL type registration

**What it is.** Builder extensions that register command input types, DTO object types (including a
custom `DtoType` with a computed field), and aggregate filter/sort input types with the GraphQL schema.

**When to use it.** Whenever you add an aggregate/command/DTO that must appear in the schema.

**How it works.** `AddCommandTypes`/`AddDtoTypes`/`AddAggregateTypes` group the registrations and are
chained in `Program.cs` after `AddGraphQLService()`. A custom `DtoType<T>` can add resolved fields;
`AddAggregateFilterType`/`AddAggregateSortType` provide the dynamic-data-aware filter/sort inputs used
by the `Product` query.

```csharp
services.AddGraphQLService()
    .AddMongoDbQuerySettings()
    .AddDynamicDataTypes()
    .AddCommandTypes()
    .AddDtoTypes()
    .AddAggregateTypes()
    .AddMutationType<Mutation>()
    .AddQueryType<Query>();
```

**Sample code in this template.**
- [`Infrastructure/RequestExecutorBuilderExtensions.cs`](../../src/BytLabs.MicroserviceTemplate.Infrastructure/RequestExecutorBuilderExtensions.cs)
- [`Infrastructure/DtoTypes/ProductDtoType.cs`](../../src/BytLabs.MicroserviceTemplate.Infrastructure/DtoTypes/ProductDtoType.cs) — computed `variantCount` field
- [`Api/Program.cs`](../../src/BytLabs.MicroserviceTemplate.Api/Program.cs)

**Reference (BytLabs.BackendPackages).** `BytLabs.Api.Graphql` (`AddCommandType`, `AddDtoType`, `AddAggregateFilterType`, `AddAggregateSortType`, `AddDynamicDataTypes`), `DtoType<T>`.

**Related recipes.** [GraphQL query](graphql-query.md), [Dynamic-data query](graphql-dynamic-data-query.md), [DTOs](dtos.md).
