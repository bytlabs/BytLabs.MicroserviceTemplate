# Mutation with error types

**What it is.** A GraphQL write endpoint on the partial `Mutation` class that forwards a command via
`IMediator` and declares typed errors with `[Error(typeof(...))]`.

**When to use it.** For every write operation exposed over GraphQL.

**How it works.** The resolver takes the command as `input`, sends it through MediatR, and returns the
command result. `[Error(typeof(BusinessError))]` / `[Error(typeof(ValidationError))]` make domain and
validation failures show up as structured `errors` on the mutation payload (see
[mutation conventions](mutation-conventions.md)).

```csharp
[Authorize]
[Error(typeof(BusinessError))]
[Error(typeof(ValidationError))]
public async Task<ProductDto> CreateProduct(CreateProductCommand input, [Service] IMediator mediator, CancellationToken ct)
    => await mediator.Send(input, ct);
```

**Sample code in this template.**
- [`Graphql/Mutations/ProductMutations.cs`](../../src/BytLabs.MicroserviceTemplate.Api/Graphql/Mutations/ProductMutations.cs)
- [`Graphql/Mutations/OrderMutations.cs`](../../src/BytLabs.MicroserviceTemplate.Api/Graphql/Mutations/OrderMutations.cs)

**Reference (BytLabs.BackendPackages).** `BytLabs.Api.Graphql.ErrorTypes.Business.BusinessError`, `BytLabs.Api.Graphql.ErrorTypes.Validation.ValidationError`.

**Related recipes.** [Mutation conventions](mutation-conventions.md), [Authorization](authorization.md), [Command + handler](cqrs-command-handler.md).
