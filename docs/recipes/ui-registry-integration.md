# shadcn UI registry integration

**What it is.** The cross-repo bridge between the backend `EntityDef` and the frontend. A standalone
shadcn **registry** (`bytlabs-ui-registry`) publishes reusable dynamic-UI components that any app can
install with `npx shadcn add <url>`. The bundled [console](console-app.md) is its first consumer.

**When to use it.** Whenever a frontend needs to render/author GraphQL-driven dynamic forms, tables,
and detail views for entities described by an `EntityDef`.

**The `DataSchema.Type` contract.** Each `DataSchema` payload carries a `type` tag telling the frontend
which renderer receives it:

| Payload | Tag | Renderer (registry item) |
|---|---|---|
| `form.formSchema` | `rjsf/formSchema` | `dynamic-form` (RJSF) |
| `form.formUi` | `rjsf/uiSchema` | `dynamic-form` (RJSF) |
| `table.columns` | `tanstack/columnDef` | `dynamic-table` (TanStack) |
| `table.details` | `cms/view` | `dynamic-view` |

**Registry items** (install into any shadcn project):

```bash
# one bundle pulls form + table + view + schema-editor + graphql hooks
npx shadcn add https://<registry-host>/r/dynamic-entity.json
# or individually, e.g.
npx shadcn add https://<registry-host>/r/dynamic-form.json
```

Items: `dynamic-form`, `dynamic-table`, `dynamic-view`, `schema-editor`, `dynamic-entity-graphql`, and
the `dynamic-entity` bundle. The components are unopinionated (e.g. `DynamicForm` is just a themed RJSF
`<Form>` with an overridable `className`; `ViewEntity` renders content only) — the consumer supplies
layout chrome and an `ApolloProvider`. They install under `components/dynamic/**`.

> The registry components use the Radix `asChild` idiom in source; the shadcn CLI auto-converts these to
> the Base UI `render` prop when installing into a `base-nova` project, so they work on either base.

**Plugging in a flat entity (Product / Order).** Register the entity's GraphQL operations once in the
console's `lib/entities.ts`; the generic `EntityManager` + `useEntityDef`/`useDynamicEntity` hooks do
the rest (list, paginate, view, create, edit). No new UI code per entity.

**Sample code in this template.**
- [`Console/lib/entities.ts`](../../src/BytLabs.MicroserviceTemplate.Console/lib/entities.ts) — per-entity registration (Product)
- [`Console/components/EntityManager.tsx`](../../src/BytLabs.MicroserviceTemplate.Console/components/EntityManager.tsx) — generic CRUD page composing the registry components
- [`Console/components/SchemaAuthoring.tsx`](../../src/BytLabs.MicroserviceTemplate.Console/components/SchemaAuthoring.tsx) — Monaco schema editor backed by `EntityDef`

**Reference.** [shadcn registry docs](https://ui.shadcn.com/docs/registry), [shadcn components](https://ui.shadcn.com/docs/components).

**Related recipes.** [Bundled console app](console-app.md), [EntityDef aggregate](entity-def.md), [GraphQL EntityDef contract](graphql-entity-def.md), [Dynamic table](dynamic-table.md).
