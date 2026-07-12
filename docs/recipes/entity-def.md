# EntityDef aggregate (dynamic form + table definition)

**What it is.** A soft-deletable aggregate (`EntityDef`) that stores, per `EntityType`, how a flat
entity's dynamic **form** (`FormDataSchema`) and **table** (`TableDataSchema`) render. It holds only
schemas — the entity's actual values live as `JsonElement Data` on the target aggregate. Flat entities
(e.g. `Product`) do **not** carry their own render schema; `EntityDef` is the single source of truth.

**When to use it.** When a client (the bundled console or a shadcn UI) must render create/edit forms
and list tables for an entity whose fields are configured at runtime rather than compiled in.

**How it works.** `EntityDef.Create/Update/Remove` mirror the definition lifecycle. `Update` replaces
`Form` + `Table` wholesale; `Remove` is a soft delete. Each transition raises a typed domain event.

```csharp
public sealed class EntityDef : AggregateRootBase<Guid>, ISoftDeletable
{
    public string EntityType { get; private set; }
    public FormDataSchema Form { get; private set; }
    public TableDataSchema Table { get; private set; }
    public bool IsDeleted { get; private set; }

    public void Update(UpdateEntityDef value)  // replace form + table
    {
        Form = value.Form; Table = value.Table;
        AddDomainEvent(new EntityDefUpdated(Id, value));
    }
}
```

**Sample code in this template.**
- [`EntityDef.cs`](../../src/BytLabs.MicroserviceTemplate.Domain/EntityDefs/EntityDef.cs) — aggregate + Create/Update/Remove
- [`DataObjects/`](../../src/BytLabs.MicroserviceTemplate.Domain/EntityDefs/DataObjects) — `CreateEntityDef`, `UpdateEntityDef`
- [`EntityDefs/Commands/`](../../src/BytLabs.MicroserviceTemplate.Application/EntityDefs/Commands) — `CreateEntityDef`, `UpdateEntityDef`, `RemoveEntityDef`

**Reference (BytLabs.BackendPackages).** `BytLabs.Domain.Entities.AggregateRootBase<TId>`, `ISoftDeletable`.

**Related recipes.** [EntityDef schema flow (define → store → render)](entity-def-schema-flow.md), [Schema value objects](schema-value-objects.md), [Dynamic table](dynamic-table.md), [GraphQL EntityDef contract](graphql-entity-def.md), [Soft delete](soft-delete.md).
