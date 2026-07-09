# BytLabs Console (admin SPA)

A **Vite + React** admin console that manages an entity's records (list / paginate / view / create /
edit) and authors their form/table/view schema. It is built to a static client SPA and **served by the
API under `/console`** — one container, one port. It consumes the dynamic-UI components from the
[BytLabs UI registry](../../../bytlabs-ui-registry) (vendored under `src/components/dynamic`).

> Full architecture write-up: [`docs/recipes/console-app.md`](../../docs/recipes/console-app.md) and
> [`docs/recipes/ui-registry-integration.md`](../../docs/recipes/ui-registry-integration.md).

## Layout

```
index.html               # SPA entry (loads /src/main.tsx)
vite.config.ts           # base:'/console/', alias @→./src, dev proxy to the API (:5024)
components.json          # shadcn config (base-nova style, css: src/index.css)
src/
  main.tsx App.tsx       # bootstrap + React Router (basename="/console")
  providers.tsx          # config fetch + ApolloProvider + AuthGate
  auth-gate.tsx login-form.tsx
  home.tsx
  components/
    console-layout.tsx app-sidebar.tsx nav-user.tsx   # sidebar-07 shell + logout
    EntityManager.tsx SchemaAuthoring.tsx             # generic CRUD page + schema editor
    dynamic/**           # VENDORED registry components (form, table, view, editor)
    ui/**                # shadcn primitives (base-nova / Base UI)
  lib/{config,apollo,entities}.ts
  hooks/**
```

## Run

```bash
npm install
npm run dev      # Vite dev server on http://localhost:5173, proxying /graphql, /console/config,
                 # and /auth to the API at http://localhost:5024
npm run build    # → dist/  (copied to the API's wwwroot/console)
```

- **Via the API (recommended):** building/running the **API** triggers the `BuildConsoleUi` MSBuild
  target, which runs `npm run build` and publishes `dist/` into `wwwroot/console`. Then browse
  `http://localhost:5024/console/`. Disable with `-p:BuildConsoleUi=false`.
- **In Visual Studio:** F5 on the Console `.esproj` runs `npm run dev`.

## Config & auth (no rebuild needed)

On boot the SPA fetches `GET /console/config` → `{ authMode, oidc, graphqlEndpoint }`, driven by the
API's `ConsoleAuth` appsettings section:

- `None` — open (default, runs out of the box)
- `Basic` — login form → `POST /auth/login` → API issues an HS256 JWT its own JwtBearer accepts
- `Oidc` — external IdP

## Adding an entity

Register the entity's GraphQL operations once in [`src/lib/entities.ts`](src/lib/entities.ts) (list /
create / update / remove + input mappers). The generic `EntityManager` and the vendored dynamic
components do the rest — **no new UI code per entity**. Sidebar links are generated from the `ENTITIES`
map.

## Consuming registry updates

The `src/components/dynamic/**` components are a **vendored copy** of the registry. When a shared
component changes, fix it in the [registry](../../../bytlabs-ui-registry) first (and rebuild its
`public/r`), then sync the change here. Keep both copies in step.

**Gotcha — date widgets.** `DatePickerWidget` must emit a value matching the schema's `format`: a
`date` field wants `"YYYY-MM-DD"` (a full ISO datetime fails AJV's `date` format check); only
`date-time` fields keep the ISO string.
