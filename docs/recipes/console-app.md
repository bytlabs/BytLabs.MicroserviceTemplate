# Bundled console app (static export, single-port)

**What it is.** A Next.js admin **console** (`src/BytLabs.MicroserviceTemplate.Console`) that is built to
a static export and served by the API under `/console` — one container, one port. It manages entities
(list/pagination/view/create/edit) and authors their schema, consuming the
[shadcn UI registry](ui-registry-integration.md).

**When to use it.** As the out-of-the-box admin UI for a service's dynamic entities, or as a starting
point for a bespoke console.

**How it works.**
- The console is `output: 'export'` with `basePath: '/console'` (SPA-only, no RSC). Its build (`out/`)
  is copied to the API's `wwwroot/console`; the API serves it via `UseStaticFiles()` +
  `MapFallbackToFile("/console/{*path:nonfile}", "console/index.html")` so client routes resolve.
- On boot the SPA fetches `GET /console/config` → `{ authMode, oidc, graphqlEndpoint }` from the
  `ConsoleAuth` appsettings section (change auth/endpoint without rebuilding the static app).
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
- Local: `bash src/BytLabs.MicroserviceTemplate.Console/build-console.sh` (builds + copies into
  `wwwroot/console`), then run the API and open `/console`.
- Docker: the `Dockerfile` has a `node` stage that runs `npm ci && npm run build` and copies `out/`
  into the final image's `wwwroot/console` (the console's registry components are vendored under
  `components/dynamic`, so no registry server is needed at build time).

**Sample code in this template.**
- [`src/BytLabs.MicroserviceTemplate.Console/`](../../src/BytLabs.MicroserviceTemplate.Console) — the Next.js app (`app/`, `components/`, `lib/`)
- [`Api/ConsoleApp/ConsoleEndpoints.cs`](../../src/BytLabs.MicroserviceTemplate.Api/ConsoleApp/ConsoleEndpoints.cs) — `/console/config`, `/auth/login`
- [`Api/ConsoleApp/ConsoleAuthConfiguration.cs`](../../src/BytLabs.MicroserviceTemplate.Api/ConsoleApp/ConsoleAuthConfiguration.cs), [`Api/Extensions/AuthenticationExtensions.cs`](../../src/BytLabs.MicroserviceTemplate.Api/Extensions/AuthenticationExtensions.cs) — JWT accepts the console key
- [`Api/Program.cs`](../../src/BytLabs.MicroserviceTemplate.Api/Program.cs) — static serving + SPA fallback

**Reference (BytLabs.BackendPackages).** `Microsoft.AspNetCore.Authentication.JwtBearer` (JWT), ASP.NET static files.

**Related recipes.** [shadcn UI registry integration](ui-registry-integration.md), [EntityDef aggregate](entity-def.md), [Authorization](authorization.md).
