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

**Authoring → build → publish → consume (the flow).** In the `bytlabs-ui-registry` repo:

1. **Author** a component under `registry/<item>/…` and declare it (files, `registryDependencies`, npm
   `dependencies`) in **`registry.json`** — the manifest and source of truth.
2. **Build** with `npm run registry:build` (`shadcn build`) → emits **`public/r/*.json`**. This is the
   step people forget: **`public/r/*.json` is what `npx shadcn add` downloads, not the `.tsx` source.**
   Editing a component without rebuilding ships nothing.
3. **Publish** by hosting `public/` so items resolve at `https://<host>/r/<item>.json`.
4. **Consume** with `npx shadcn add https://<host>/r/dynamic-entity.json`.

**How the console consumes it (vendored, not live).** The bundled console does **not** fetch from a
registry server at build time — the registry components are **vendored** (copied) into
`src/components/dynamic/**`, so it builds offline. When a shared component changes, fix it in the
registry first (rebuild `public/r`), then sync the change into the console's vendored copy. Both copies
must stay in step.

**Plugging in a flat entity (Product / Order).** Register the entity's GraphQL operations once in the
console's `src/lib/entities.ts`; the generic `EntityManager` + `useEntityDef`/`useDynamicEntity` hooks do
the rest (list, paginate, view, create, edit). No new UI code per entity.

**Gotcha — date widgets & AJV formats.** `DatePickerWidget` must emit a value matching the schema's
declared `format`. A `date` field (JSON Schema full-date) requires `"YYYY-MM-DD"`; emitting a full ISO
datetime (`toISOString()`) fails AJV's `date` format check with *`must match format "date"`*. Only
`date-time` fields keep the ISO string. Parse stored `"YYYY-MM-DD"` values as **local** dates (not UTC)
so the calendar doesn't drift a day in negative-offset timezones. This fix lives in **both** the registry
source and the console's vendored copy.

**Sample code in this template.**
- [`Console/src/lib/entities.ts`](../../src/BytLabs.MicroserviceTemplate.Console/src/lib/entities.ts) — per-entity registration (Product, Order)
- [`Console/src/components/EntityManager.tsx`](../../src/BytLabs.MicroserviceTemplate.Console/src/components/EntityManager.tsx) — generic CRUD page composing the registry components
- [`Console/src/components/SchemaAuthoring.tsx`](../../src/BytLabs.MicroserviceTemplate.Console/src/components/SchemaAuthoring.tsx) — Monaco schema editor backed by `EntityDef`
- [`Console/src/components/dynamic/**`](../../src/BytLabs.MicroserviceTemplate.Console/src/components/dynamic) — vendored registry components

**Reference.** [shadcn registry docs](https://ui.shadcn.com/docs/registry), [shadcn components](https://ui.shadcn.com/docs/components).

**Related recipes.** [Bundled console app](console-app.md), [EntityDef aggregate](entity-def.md), [GraphQL EntityDef contract](graphql-entity-def.md), [Dynamic table](dynamic-table.md).
