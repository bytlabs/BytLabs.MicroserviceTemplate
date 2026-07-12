# Typed domain events

**What it is.** Events that record something meaningful that happened to an aggregate, carrying the
data that caused it. They derive from `DomainEventBase<TId, TData>` (or `DomainEventBase` when there
is no payload) and are dispatched in-process after the aggregate is saved.

**When to use it.** To decouple side effects (emails, read-model updates, integration messages) from
the write that triggered them.

**How it works.** The aggregate calls `AddDomainEvent(new XHappened(id, data))`; a
`DomainEventHandler<T>` reacts. `IDomainEvent` requires `CreatedAt`/`CreatedBy`, which the base class
supplies — so a payload-less event must be a class deriving from `DomainEventBase`, not a record.

```csharp
public class ProductCreated(Guid id, CreateProduct data) : DomainEventBase<Guid, CreateProduct>(id, data);
public class ProductRemoved(Guid productId) : DomainEventBase { public Guid ProductId { get; } = productId; }
```

**Sample code in this template.**
- [`ProductCreated.cs`](../../src/BytLabs.MicroserviceTemplate.Domain/Products/Events/ProductCreated.cs) — typed event with payload
- [`ProductRemoved.cs`](../../src/BytLabs.MicroserviceTemplate.Domain/Products/Events/ProductRemoved.cs) — payload-less event
- [`OrderCreatedEvent.cs`](../../src/BytLabs.MicroserviceTemplate.Domain/Orders/Events/OrderCreatedEvent.cs)

**Reference (BytLabs.BackendPackages).** `BytLabs.Domain.DomainEvents.DomainEventBase` / `DomainEventBase<TId,TData>` / `IDomainEvent`.

**Related recipes.** [Domain event handler](domain-event-handler.md), [Aggregate root](aggregate-root.md).
