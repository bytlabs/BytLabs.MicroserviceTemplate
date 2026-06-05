# Domain event handler (side effects)

**What it is.** A `DomainEventHandler<TEvent>` that runs a side effect (send email, update a read
model, publish an integration event) after a domain event is dispatched.

**When to use it.** When a write should trigger follow-on work that you don't want coupled into the
command handler.

**How it works.** Implement `DomainEventHandler<TEvent>` and override `HandleDomainEvent`. It is
discovered/registered by `AddCQS` and invoked after the aggregate that raised the event is saved.

```csharp
public class SendEmailOrderCreatedEventHandler : DomainEventHandler<OrderCreatedEvent>
{
    protected override async Task HandleDomainEvent(OrderCreatedEvent e, CancellationToken ct)
    {
        var order = await orderRepository.GetByIdAsync(e.OrderId, ct);
        order.MarkAsEmailSent();
        await emailService.SendEmail("test@test.com", $"#{e.OrderId} order is placed.", "Welcome...");
        await orderRepository.UpdateAsync(order, ct);
    }
}
```

**Sample code in this template.**
- [`Events/OrderCreatedEvents/SendEmailOrderCreatedEventHandler.cs`](../../src/BytLabs.MicroserviceTemplate.Application/Events/OrderCreatedEvents/SendEmailOrderCreatedEventHandler.cs)

**Reference (BytLabs.BackendPackages).** `BytLabs.Application.DomainEvents.DomainEventHandler<T>`.

**Related recipes.** [Typed domain events](domain-events.md).
