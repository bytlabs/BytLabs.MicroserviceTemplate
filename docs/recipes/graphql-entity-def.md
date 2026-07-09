# GraphQL EntityDef contract (create / update / remove)

**What it is.** The full GraphQL surface for the `EntityDef` aggregate: a paged, filterable
`entityDefs` query plus `createEntityDef` / `updateEntityDef` / `removeEntityDef` mutations. This is
the create/**update**/remove pattern the older recipes omitted.

**When to use it.** Whenever a client authors or reads dynamic form/table definitions.

**How it works.** Commands carry the `FormDataSchema`/`TableDataSchema` value objects **directly** as
inputs — `AddCommandType<T>()` generates `CreateEntityDefInput`, `FormDataSchemaInput`,
`DataSchemaInput`, etc., so there is no hand-written `...Input` duplication. Mutations dispatch via
MediatR and surface `BusinessError | ValidationError`. The query excludes soft-deleted rows and
projects to `EntityDefDto`.

```csharp
[Authorize]
[Error(typeof(BusinessError))]
[Error(typeof(ValidationError))]
public async Task<EntityDefDto> UpdateEntityDef(UpdateEntityDefCommand input, [Service] IMediator mediator, CancellationToken ct)
    => await mediator.Send(input, ct);
```

> Registration note: the query uses `[UseSorting(Type = typeof(EntityDef))]`, which generates the sort
> input itself — register only `AddAggregateFilterType<EntityDef, Guid>()`. Also register the value
> objects once (via any command that uses them); a second explicit `FormDataSchemaInput` record would
> collide on the type name.

**Sample code in this template.**
- [`Graphql/Queries/EntityDefQueries.cs`](../../src/BytLabs.MicroserviceTemplate.Api/Graphql/Queries/EntityDefQueries.cs)
- [`Graphql/Mutations/EntityDefMutations.cs`](../../src/BytLabs.MicroserviceTemplate.Api/Graphql/Mutations/EntityDefMutations.cs)
- [`Infrastructure/RequestExecutorBuilderExtensions.cs`](../../src/BytLabs.MicroserviceTemplate.Infrastructure/RequestExecutorBuilderExtensions.cs) — `AddCommandType`/`AddDtoType`/`AddAggregateFilterType`

**Reference (BytLabs.BackendPackages).** `BytLabs.Api.Graphql` (`AddCommandType`, `AddDtoType`, `AddAggregateFilterType`, error types).

**Related recipes.** [EntityDef aggregate](entity-def.md), [Mutation conventions](mutation-conventions.md), [GraphQL type registration](graphql-type-registration.md).
