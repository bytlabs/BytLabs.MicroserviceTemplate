# Mutation conventions

**What it is.** HotChocolate's input/payload/error naming conventions that give every mutation a
consistent `{Name}Input` argument, `{Name}Payload` result, and `{Name}Error` union.

**When to use it.** Always — it standardizes the GraphQL contract so clients and codegen are uniform.

**How it works.** The package default `AddGraphQLService()` enables mutation conventions (input
argument named `input`, `{MutationName}Input`/`Payload`/`Error` patterns, `errors` field). You do not
re-add them per service. A mutation returning `ProductDto` therefore exposes
`createProduct(input: CreateProductInput!): CreateProductPayload!` with a `product` field and an
`errors` union of the declared `[Error(...)]` types.

**Sample code in this template.**
- [`Api/Program.cs`](../../src/BytLabs.MicroserviceTemplate.Api/Program.cs) — relies on `AddGraphQLService()` defaults (no manual convention setup)
- [`Graphql/Mutations/ProductMutations.cs`](../../src/BytLabs.MicroserviceTemplate.Api/Graphql/Mutations/ProductMutations.cs) — declares `[Error(...)]` types

**Reference (BytLabs.BackendPackages).** `BytLabs.Api.Graphql.ServiceExtensions.AddGraphQLService` → `AddBytLabsDefaults` (`AddMutationConventions`).

**Related recipes.** [Mutation with error types](graphql-mutation.md), [StrawberryShake typed client](strawberry-shake-client.md).
