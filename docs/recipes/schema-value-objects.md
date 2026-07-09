# Schema value objects

**What it is.** Immutable value objects (`DataSchema`, `FormDataSchema`, `TableDataSchema`) that
describe dynamic form/table UI schemas, compared by value rather than identity.

**When to use it.** To attach UI/validation schema (a JSON schema + UI hints + sample payload) to a
dynamic-data aggregate so clients can render forms/tables for the schema-less `Data`.

**How it works.** Each derives from `ValueObject` and implements `GetEqualityComponents()`. They are
composed (`FormDataSchema` holds three `DataSchema` parts) and require a BSON class map with a
creator + defaults to persist — see [MongoDB BSON class maps](bson-class-maps.md).

```csharp
public class DataSchema : ValueObject
{
    public string Type { get; private set; }
    public string Data { get; private set; }
    protected override IEnumerable<object> GetEqualityComponents() => [Type, Data];
}
```

**Sample code in this template.**
- [`Shared/DynamicData/`](../../src/BytLabs.MicroserviceTemplate.Domain/Shared/DynamicData) — `DataSchema`, `FormDataSchema`, `TableDataSchema`
- [`EntityDef.cs`](../../src/BytLabs.MicroserviceTemplate.Domain/Aggregates/EntityDefAggregate/EntityDef.cs) — holds `Form` (`FormDataSchema`) + `Table` (`TableDataSchema`) on a definition aggregate, with create/update/remove (see [GraphQL EntityDef contract](graphql-entity-def.md))

> Note: the render schema lives on the `EntityDef` aggregate (keyed by `EntityType`), not on the
> entity being described. Flat entities like `Product` carry only their `Data`.

**Reference (BytLabs.BackendPackages).** `BytLabs.Domain.ValueObjects.ValueObject`.

**Related recipes.** [EntityDef schema flow (define → store → render)](entity-def-schema-flow.md), [Dynamic data](dynamic-data.md), [Dynamic table](dynamic-table.md), [EntityDef aggregate](entity-def.md), [BSON class maps](bson-class-maps.md), [Specialized update methods](specialized-update-methods.md).
