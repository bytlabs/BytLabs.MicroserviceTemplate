# Acceptance testing with xUnit

**What it is.** End-to-end tests that boot the API in-process with `WebApplicationFactory<Program>`
and drive it through the generated StrawberryShake GraphQL client — plain xUnit, **no SpecFlow**.

**When to use it.** To verify real API flows (mutations + queries) against a running MongoDB, the way
a client would call them.

**How it works.** `CustomWebApplicationFactory` boots the app, registers a `TestAuthHandler` so
`[Authorize]` resolvers are reachable, and sends a `Tenant` header for [multitenancy](multitenancy.md).
Tests resolve the typed client and assert on payloads/errors.

```csharp
var created = await _client.CreateProduct.ExecuteAsync(createInput, CancellationToken.None);
created.Data!.CreateProduct.Errors.Should().BeNullOrEmpty();
created.Data.CreateProduct.Product!.Name.Should().Be("Widget");
```

**Sample code in this template.**
- [`Tests.Accpetance/Tests/ProductTests.cs`](../../src/BytLabs.MicroserviceTemplate.Tests.Accpetance/Tests/ProductTests.cs) — full lifecycle
- [`Tests.Accpetance/Tests/CreateOrderTests.cs`](../../src/BytLabs.MicroserviceTemplate.Tests.Accpetance/Tests/CreateOrderTests.cs)
- [`Support/CustomWebApplicationFactory.cs`](../../src/BytLabs.MicroserviceTemplate.Tests.Accpetance/Support/CustomWebApplicationFactory.cs)
- [`Support/TestAuthHandler.cs`](../../src/BytLabs.MicroserviceTemplate.Tests.Accpetance/Support/TestAuthHandler.cs)

Run (needs MongoDB): `docker-compose up -d` then `dotnet test src/BytLabs.MicroserviceTemplate.Tests.Accpetance`

**Reference (BytLabs.BackendPackages).** `Microsoft.AspNetCore.Mvc.Testing` (`WebApplicationFactory`).

**Related recipes.** [StrawberryShake typed client](strawberry-shake-client.md), [Authorization](authorization.md), [Multitenancy](multitenancy.md).
