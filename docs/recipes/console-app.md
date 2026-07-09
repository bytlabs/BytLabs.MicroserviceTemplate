# Bundled console app (client SPA, single-port)

**What it is.** A **Vite + React** admin **console** (`src/BytLabs.MicroserviceTemplate.Console`) built to
a static client SPA and served by the API under `/console` — one container, one port. It manages entities
(list/pagination/view/create/edit) and authors their schema, consuming the
[shadcn UI registry](ui-registry-integration.md).

**When to use it.** As the out-of-the-box admin UI for a service's dynamic entities, or as a starting
point for a bespoke console.

**Why a client SPA (not Next.js SSR).** The console is client-only — it talks to GraphQL from the
browser and has no server-rendering needs. A Vite SPA is the natural fit: one `index.html` + assets,
client-side routing with **no per-route build** and none of the static-export/RSC edge cases.

**How it works.**
- Vite is configured with `base: '/console/'` and builds to `dist/`, which is copied to the API's
  `wwwroot/console`. The API serves it via `UseStaticFiles()` +
  `MapFallbackToFile("/console/{*path:nonfile}", "console/index.html")`, so any client route resolves.
- Routing is **React Router** with `basename="/console"` — deep links (e.g. `/console/entities/Product`)
  hit the SPA-fallback `index.html` and render client-side.
- On boot the SPA fetches `GET /console/config` → `{ authMode, oidc, graphqlEndpoint }` from the
  `ConsoleAuth` appsettings section (change auth/endpoint without rebuilding the app).
- **Auth** (`ConsoleAuth:Mode`): `Basic` shows a login form → `POST /auth/login` → the API validates
  the configured credentials and issues an HS256 JWT that its own JwtBearer accepts; `Oidc` uses an
  IdP; `None` is open (default, runs out of the box).

```jsonc
"ConsoleAuth": {
  "Mode": "Basic",                 // Basic | Oidc | None
  "Basic": { "Username": "admin", "Password": "…", "JwtSigningKey": "<32+ chars>" },
  "Oidc":  { "Authority": "", "ClientId": "" }
}
```

**Build & run.**
- **Automatic:** building/running the **API** triggers the `BuildConsoleUi` MSBuild target, which runs
  `npm run build` (Vite) and publishes `dist/` into `wwwroot/console`. Incremental (only when console
  sources change); disable with `-p:BuildConsoleUi=false`.
- **Hot-reload dev:** run `npm run dev` in the console (Vite dev server on :5173) — it proxies
  `/graphql`, `/console/config`, and `/auth` to the API at `localhost:5024`. In Visual Studio, F5 on the
  Console `.esproj` runs this.
- **Manual/CI:** `bash build-console.sh`, or the `Dockerfile`'s Node stage (`npm ci && npm run build`)
  copies `dist/` into the image's `wwwroot/console`. Registry components are vendored under
  `src/components/dynamic`, so no registry server is needed at build time (see
  [ui-registry-integration.md](ui-registry-integration.md) for how the vendored copy stays in sync).

**Sample code in this template.**
- [`src/BytLabs.MicroserviceTemplate.Console/`](../../src/BytLabs.MicroserviceTemplate.Console) — the Vite SPA (`main.tsx`, `App.tsx`, `components/`, `lib/`)
- [`Api/ConsoleApp/ConsoleEndpoints.cs`](../../src/BytLabs.MicroserviceTemplate.Api/ConsoleApp/ConsoleEndpoints.cs) — `/console/config`, `/auth/login`
- [`Api/ConsoleApp/ConsoleAuthConfiguration.cs`](../../src/BytLabs.MicroserviceTemplate.Api/ConsoleApp/ConsoleAuthConfiguration.cs), [`Api/Extensions/AuthenticationExtensions.cs`](../../src/BytLabs.MicroserviceTemplate.Api/Extensions/AuthenticationExtensions.cs) — JWT accepts the console key
- [`Api/Program.cs`](../../src/BytLabs.MicroserviceTemplate.Api/Program.cs) — static serving + SPA fallback; [`Api/....csproj`](../../src/BytLabs.MicroserviceTemplate.Api/BytLabs.MicroserviceTemplate.Api.csproj) — `BuildConsoleUi` target

**Reference (BytLabs.BackendPackages).** `Microsoft.AspNetCore.Authentication.JwtBearer` (JWT), ASP.NET static files.

**Related recipes.** [shadcn UI registry integration](ui-registry-integration.md), [EntityDef aggregate](entity-def.md), [Authorization](authorization.md).
