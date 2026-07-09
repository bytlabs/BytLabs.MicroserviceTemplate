# BytLabs Microservice Recipes

This template is a **recipe catalog**. `Order` is the minimal example; `Product` demonstrates the
advanced patterns. Each recipe below has its own page with an explanation and links to the real
sample code in this repository.

> Built on [BytLabs.BackendPackages](https://github.com/bytlabs/BytLabs.BackendPackages). Recipes that
> reuse a package building block link to the relevant base type.

## Domain
- [Aggregate root & domain events](aggregate-root.md)
- [Sub-entity inside an aggregate](sub-entity.md)
- [Data objects (Create/Update)](data-objects.md)
- [Typed domain events](domain-events.md)
- [Dynamic data (schema-less JSON)](dynamic-data.md)
- [Soft delete](soft-delete.md)
- [Schema value objects](schema-value-objects.md)
- [EntityDef aggregate (dynamic form + table)](entity-def.md)
- [Dynamic table (TableDataSchema)](dynamic-table.md)
- [Specialized update methods](specialized-update-methods.md)

## Application (CQRS)
- [Command + handler](cqrs-command-handler.md)
- [Sub-entity add/remove commands](sub-entity-commands.md)
- [DTOs](dtos.md)
- [AutoMapper profiles](automapper-profiles.md)
- [Domain event handler (side effects)](domain-event-handler.md)

## API (GraphQL)
- [Query with paging/projection/filtering/sorting](graphql-query.md)
- [Dynamic-data query + soft-delete filter](graphql-dynamic-data-query.md)
- [GraphQL EntityDef contract (create/update/remove)](graphql-entity-def.md)
- [Mutation with error types](graphql-mutation.md)
- [Authorization](authorization.md)
- [Mutation conventions](mutation-conventions.md)
- [GraphQL type registration](graphql-type-registration.md)

## Infrastructure
- [Service registration](service-registration.md)
- [MongoDB BSON class maps](bson-class-maps.md)
- [Custom BSON serializer](custom-bson-serializer.md)

## Cross-cutting
- [Multitenancy (database-per-tenant)](multitenancy.md)

## Testing
- [Unit testing an aggregate](unit-testing.md)
- [Acceptance testing with xUnit](acceptance-testing-xunit.md)
- [StrawberryShake typed client](strawberry-shake-client.md)
