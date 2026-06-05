# Authorization

**What it is.** Protecting GraphQL resolvers with `[Authorize]`, backed by JWT bearer authentication.

**When to use it.** On operations that require an authenticated (or specifically authorized) caller.
Here `[Authorize]` is applied at the **method level** on the `Product` query and mutations, so the
simple `Order` endpoints stay open as a contrast.

**How it works.** `AddGraphQLService()` already wires HotChocolate authorization. JWT bearer auth is
configured from `appsettings` (`Authentication:Authority`/`Audience`) and added to the pipeline with
`UseAuthentication()`/`UseAuthorization()`. Unauthenticated calls to a protected field return
`AUTH_NOT_AUTHENTICATED`. In acceptance tests a `TestAuthHandler` authenticates every request.

```csharp
[Authorize]
public IExecutable<ProductDto> GetProducts(...) { ... }
```

**Sample code in this template.**
- [`Graphql/Queries/ProductQueries.cs`](../../src/BytLabs.MicroserviceTemplate.Api/Graphql/Queries/ProductQueries.cs) — `[Authorize]`
- [`Graphql/Mutations/ProductMutations.cs`](../../src/BytLabs.MicroserviceTemplate.Api/Graphql/Mutations/ProductMutations.cs) — `[Authorize]`
- [`Api/Extensions/AuthenticationExtensions.cs`](../../src/BytLabs.MicroserviceTemplate.Api/Extensions/AuthenticationExtensions.cs) — JWT wiring
- [`Tests.Accpetance/Support/TestAuthHandler.cs`](../../src/BytLabs.MicroserviceTemplate.Tests.Accpetance/Support/TestAuthHandler.cs) — test auth

**Reference (BytLabs.BackendPackages).** `HotChocolate.Authorization`; ASP.NET Core `JwtBearer`.

**Related recipes.** [Mutation with error types](graphql-mutation.md), [Acceptance testing with xUnit](acceptance-testing-xunit.md).
