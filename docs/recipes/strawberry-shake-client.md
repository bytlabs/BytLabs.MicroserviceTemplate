# StrawberryShake typed client

**What it is.** A strongly-typed C# GraphQL client generated from `.graphql` operation documents and a
local `schema.graphql`, used by tests and other services to call this API.

**When to use it.** For app-to-app calls and acceptance tests — you get compile-time-checked
operations and result types instead of hand-written queries.

**How it works.** Operation documents live under `Client/GraphQLClient/{Queries,Mutations}`; the
StrawberryShake build task generates the client from them + `schema.graphql`. Because operations are
validated against `schema.graphql`, refresh that file after changing the server schema.

```bash
# 1) run the API so its schema is available
docker-compose up -d
# 2) refresh the local schema
cd src/BytLabs.MicroserviceTemplate.Client/GraphQLClient
dotnet tool restore
dotnet graphql download http://localhost:8080/graphql/ -f schema.graphql
# 3) build to regenerate the client
dotnet build src/BytLabs.MicroserviceTemplate.Client
```

> Tip: mutation conventions name payload fields after the result type — e.g. `createProduct`'s payload
> field is `product`, `removeProduct`'s is `removeProductResult`. Check `schema.graphql` when writing
> a new operation.

**Sample code in this template.**
- [`Client/GraphQLClient/Mutations/CreateProduct.graphql`](../../src/BytLabs.MicroserviceTemplate.Client/GraphQLClient/Mutations/CreateProduct.graphql)
- [`Client/GraphQLClient/Queries/GetProducts.graphql`](../../src/BytLabs.MicroserviceTemplate.Client/GraphQLClient/Queries/GetProducts.graphql)
- [`Client/GraphQLClient/.graphqlrc.json`](../../src/BytLabs.MicroserviceTemplate.Client/GraphQLClient/.graphqlrc.json)

**Reference (BytLabs.BackendPackages).** StrawberryShake (third-party).

**Related recipes.** [Acceptance testing with xUnit](acceptance-testing-xunit.md), [Mutation conventions](mutation-conventions.md).
