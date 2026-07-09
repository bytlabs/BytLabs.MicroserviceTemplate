# Dynamic table (TableDataSchema)

**What it is.** The table half of a dynamic entity definition. `TableDataSchema` carries four
tagged-JSON `DataSchema` payloads — `Columns` (`tanstack/columnDef`), `Filter`, `Details`
(`cms/view`), and `SampleData` — that a frontend uses to render a list table and detail view.

**When to use it.** Alongside [`FormDataSchema`](schema-value-objects.md) on an [`EntityDef`](entity-def.md)
so a client can render both the form and the table for a runtime-configured entity.

**How it works.** `TableDataSchema` is a `ValueObject` persisted via a BSON class map (creator +
defaults). It was previously defined but unwired; the `EntityDef.Table` property is its first real
consumer. The `DataSchema.Type` tags tell the frontend which renderer receives each payload.

| Payload | Tag | Frontend renderer |
|---|---|---|
| `Columns` | `tanstack/columnDef` | TanStack + shadcn data-table |
| `Details` | `cms/view` | recursive view/detail renderer |
| `Filter` | (app-defined) | column filters |

**Sample code in this template.**
- [`Shared/DynamicData/TableDataSchema.cs`](../../src/BytLabs.MicroserviceTemplate.Domain/Shared/DynamicData/TableDataSchema.cs)
- [`EntityDef.cs`](../../src/BytLabs.MicroserviceTemplate.Domain/Aggregates/EntityDefAggregate/EntityDef.cs) — `Table`
- [`Infrastructure/Shared/DynamicData/DynamicDataExtensions.cs`](../../src/BytLabs.MicroserviceTemplate.Infrastructure/Shared/DynamicData/DynamicDataExtensions.cs) — BSON class map

**Reference (BytLabs.BackendPackages).** `BytLabs.Domain.ValueObjects.ValueObject`.

**Related recipes.** [Schema value objects](schema-value-objects.md), [EntityDef aggregate](entity-def.md), [BSON class maps](bson-class-maps.md).
