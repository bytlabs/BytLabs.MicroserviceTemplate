# EntityDef schema flow (define → store → render)

**What it is.** The end-to-end path of a dynamic entity's UI schema: how it is **authored**, **where it
is stored**, and how the app **renders forms/tables/views** from it. This ties together the pieces
covered separately in [EntityDef aggregate](entity-def.md), [Schema value objects](schema-value-objects.md),
[Dynamic data](dynamic-data.md), and [UI registry integration](ui-registry-integration.md).

**The core idea — two separate stores, joined by `EntityType`.**

| | Where it lives | Keyed by | Cardinality |
|---|---|---|---|
| **Shape** (form/table/view *schema*) | `EntityDef` aggregate (`Form`, `Table`) | `EntityType` (e.g. `"Product"`) | **one** per entity type |
| **Values** (a record's actual data) | the entity aggregate's `JsonElement Data` (`Product`, `Order`) | the record's `Id` | **one per record** |

The schema is **not** duplicated onto each record, and the record's values are **not** stored on the
`EntityDef`. The two are joined **client-side at render time** by `EntityType` — the console fetches the
one `EntityDef` for the type, then lists the many entity records and renders each with that schema.

## What an EntityDef holds

`EntityDef.Form` (`FormDataSchema`) and `EntityDef.Table` (`TableDataSchema`) are composite value
objects, each a bag of tagged `DataSchema { Type, Data }` parts (`Data` is a JSON string):

```
EntityDef(EntityType)
├─ Form  (FormDataSchema)
│  ├─ Key         logical form name
│  ├─ SampleData  example formData payload
│  ├─ FormSchema  ── DataSchema.Type "rjsf/formSchema"   → RJSF JSON schema
│  └─ FormUi      ── DataSchema.Type "rjsf/uiSchema"     → RJSF UI hints
└─ Table (TableDataSchema)
   ├─ SampleData  example rows
   ├─ Columns     ── DataSchema.Type "tanstack/columnDef" → table columns
   ├─ Filter      filter config
   └─ Details     ── DataSchema.Type "cms/view"           → read-only detail view
```

The `DataSchema.Type` tag is the contract that routes each part to a renderer (see
[UI registry integration](ui-registry-integration.md)).

## 1. Define — author the schema

- **In the console:** the Monaco-based **Schema Editor** (`SchemaAuthoring`) edits the JSON for each
  part and saves via the `updateEntityDef` mutation. Importable starter configs live in
  [`docs/console-samples/`](../console-samples) (`product.schema.json`, `order.schema.json`).
- **Over GraphQL:** `createEntityDef` / `updateEntityDef` carry the `FormDataSchema`/`TableDataSchema`
  value objects **directly** as inputs (`Update` replaces `Form` + `Table` wholesale). See
  [GraphQL EntityDef contract](graphql-entity-def.md).

## 2. Store — persist on the aggregate

`updateEntityDef` → `EntityDef.Update(...)` sets `Form`/`Table` and raises `EntityDefUpdated`. MongoDB
persists the value objects via their BSON class map. That's the **only** place the schema lives — one
document per `EntityType`.

## 3. Render — the app consumes it

The generic `EntityManager` fetches the def once and maps each schema part to a registry component; the
record data comes from the entity's own GraphQL operations (registered in
[`src/lib/entities.ts`](../../src/BytLabs.MicroserviceTemplate.Console/src/lib/entities.ts)):

```ts
const { def } = useEntityDef(entityType);                        // the one EntityDef for the type
const entity  = useDynamicEntity(reg);                           // the many records (list/CRUD)

const formSchema = parse(def?.form?.formSchema?.data);           // → <DynamicForm schema>
const uiSchema   = parse(def?.form?.formUi?.data);               // → <DynamicForm uiSchema>
const columns    = parse(def?.table?.columns?.data);             // → <DataTable columns>
const viewSchema = parse(def?.table?.details?.data);             // → <ViewEntity schema>
```

- **List** → `DataTable` uses `table.columns`.
- **Create/Edit** → `DynamicForm` uses `form.formSchema` + `form.formUi`.
- **View** → `ViewEntity` uses `table.details`.

## 4. Where the record's values go

Submitting a form does **not** write to the `EntityDef` — it writes to the entity's schema-less
`JsonElement Data`:

- On submit, `reg.toCreateInput(formData)` / `reg.toUpdateInput(id, formData)` map the RJSF `formData`
  into the entity's create/update GraphQL input. `Update` **merges** into existing `Data` (only supplied
  keys change) — see [Dynamic data](dynamic-data.md).
- To pre-fill the edit form, `reg.rowToFormData(row)` maps the record's `Data` back to form values.

So a `date` field authored in `form.formSchema` (format `"date"`) drives the `DatePickerWidget`, whose
value lands in the record's `Data` as `"YYYY-MM-DD"` — the schema on `EntityDef`, the value on the
record. (Emitting a full ISO datetime here fails AJV's `date` format check; see the widget gotcha in
[UI registry integration](ui-registry-integration.md).)

## Import / export the schema (portable config)

The Schema Editor treats the whole schema as a **portable JSON file**, so definitions can be shared,
versioned in git, and promoted between environments without rebuilding anything.

- **Export** (`Export` button) downloads `schema-config.json` — a single object with all six authored
  parts:

  ```jsonc
  {
    "formSchema": { /* rjsf/formSchema */ },
    "uiSchema":   { /* rjsf/uiSchema   */ },
    "formData":   { /* sample form payload */ },
    "columns":    [ /* tanstack/columnDef */ ],
    "details":    { /* cms/view */ },
    "tableData":  [ /* sample rows */ ]
  }
  ```

- **Import** (`Import` button) reads such a file and hydrates the editors. Import is **partial and
  tolerant**: only the keys present are applied, and an unparseable file is rejected with an alert.
- **Import hydrates the editor only — it does not persist.** You must click **Save** to write the
  config to the `EntityDef` (`updateEntityDef`/`createEntityDef`). Export, conversely, captures the
  current editor state, including unsaved edits.
- Ready-made starter configs ship in [`docs/console-samples/`](../console-samples)
  (`product.schema.json`, `order.schema.json`) — import one, tweak, Save.

The file's keys map 1:1 to the stored parts: `formSchema`/`uiSchema`/`formData` → `EntityDef.Form`
(`FormSchema`/`FormUi`/`SampleData`); `columns`/`details`/`tableData` → `EntityDef.Table`
(`Columns`/`Details`/`SampleData`). So export → edit → import is the same JSON that lives on the
aggregate, minus the wrapper.

## Why this is useful — a low-code mechanism

Because the shape is **data, not code**, adding or changing an entity's UI needs no C# changes, no
frontend build, and no redeploy:

- **Runtime configuration.** Author/adjust fields, columns, and detail views live in the console and
  Save — the generic `EntityManager` re-renders from the new `EntityDef` on next load. One generic UI
  serves every entity type.
- **Schema as a shareable artifact.** Export the JSON to version it in git, review it in a PR, or seed
  it into another environment (dev → staging → prod) via import or the `updateEntityDef` mutation.
- **Fast iteration & handoff.** A non-developer can shape the form/table by editing JSON (with live
  preview) instead of waiting on an app release; developers keep the stable core model in C# while the
  variable/tenant-specific surface stays in `EntityDef` + the record's `Data`.
- **Consistent, reusable rendering.** Every entity gets the same validated forms (RJSF + AJV), paginated
  tables (TanStack), and detail views (cms/view) from the [UI registry](ui-registry-integration.md) — no
  bespoke screens to build or maintain per entity.

**Sample code in this template.**
- [`SchemaEditor.tsx`](../../src/BytLabs.MicroserviceTemplate.Console/src/components/dynamic/SchemaEditor.tsx) — Monaco editors + Import/Export buttons (`schema-config.json`)
- [`docs/console-samples/`](../console-samples) — importable `product.schema.json`, `order.schema.json`
- [`EntityManager.tsx`](../../src/BytLabs.MicroserviceTemplate.Console/src/components/EntityManager.tsx) — fetches the def, maps parts → components, wires CRUD
- [`SchemaAuthoring.tsx`](../../src/BytLabs.MicroserviceTemplate.Console/src/components/SchemaAuthoring.tsx) — Monaco editor → `updateEntityDef`
- [`entities.ts`](../../src/BytLabs.MicroserviceTemplate.Console/src/lib/entities.ts) — per-entity ops + `toCreateInput`/`toUpdateInput`/`rowToFormData`
- [`FormDataSchema.cs`](../../src/BytLabs.MicroserviceTemplate.Domain/Common/DynamicData/FormDataSchema.cs), [`TableDataSchema.cs`](../../src/BytLabs.MicroserviceTemplate.Domain/Common/DynamicData/TableDataSchema.cs) — the schema value objects

**Related recipes.** [Entity associations & outer-field integration](entity-associations.md), [EntityDef aggregate](entity-def.md), [Schema value objects](schema-value-objects.md), [Dynamic data](dynamic-data.md), [GraphQL EntityDef contract](graphql-entity-def.md), [Dynamic table](dynamic-table.md), [UI registry integration](ui-registry-integration.md), [Bundled console app](console-app.md).
