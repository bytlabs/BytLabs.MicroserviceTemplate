# Multitenancy (database-per-tenant)

**What it is.** Serving many tenants from one deployment by giving each tenant its **own MongoDB
database**, resolved per request. It is transparent to domain code — there is no tenant field on
entities and no per-query tenant filter.

**When to use it.** Whenever the service is shared by multiple organizations/customers that must be
isolated from each other.

**How it works.**
1. `ITenantIdResolver`(s) resolve a `TenantId` from the request — the template uses
   `FromHeaderTenantIdResolver` (reads a `Tenant` header). `ITenantIdProvider` aggregates resolvers.
2. `BytLabs.DataAccess.MongoDB` resolves the `IMongoDatabase` per request via
   `MongoDatabaseFactory.GetDatabaseForTenant(tenantId)`, selecting a database named
   `"{baseName}-{tenantId}"` and caching one connection per tenant.
3. Repositories operate on that tenant-scoped database — isolation is **physical**.

```csharp
.WithMultiTenantContext(mt => mt.AddResolver<FromHeaderTenantIdResolver>())
```

> This is a configuration recipe — no aggregate changes. To add another resolution strategy (JWT
> claim, subdomain), register an additional `ITenantIdResolver`.

**Sample code in this template.**
- [`Api/Program.cs`](../../src/BytLabs.MicroserviceTemplate.Api/Program.cs) — `WithMultiTenantContext(...)`
- [`Tests.Accpetance/Support/CustomWebApplicationFactory.cs`](../../src/BytLabs.MicroserviceTemplate.Tests.Accpetance/Support/CustomWebApplicationFactory.cs) — tests send a `Tenant` header

**Reference (BytLabs.BackendPackages).** `BytLabs.Multitenancy` (`ITenantIdResolver`, `ITenantIdProvider`, `FromHeaderTenantIdResolver`), `BytLabs.DataAccess.MongoDB.MongoDatabaseFactory`.

**Related recipes.** [Service registration](service-registration.md), [Acceptance testing with xUnit](acceptance-testing-xunit.md).
