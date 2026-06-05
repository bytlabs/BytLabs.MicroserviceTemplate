# Unit testing an aggregate

**What it is.** Fast, infrastructure-free xUnit + FluentAssertions tests that exercise aggregate
behavior and assert the domain events it raises.

**When to use it.** For all domain logic — invariants, state transitions, dynamic-data merge,
soft-delete, sub-entity add/remove. No database or host required.

**How it works.** Construct the aggregate via its factory, invoke a method, and assert resulting state
plus the expected domain event.

```csharp
[Fact]
public void Update_merges_dynamic_data_and_overwrites_name()
{
    var product = Product.Create(NewCreate(data: "{\"color\":\"red\",\"size\":\"M\"}"));
    product.Update(new UpdateProduct("Widget v2", Json("{\"color\":\"blue\"}")));

    product.Data.GetProperty("color").GetString().Should().Be("blue"); // overwritten
    product.Data.GetProperty("size").GetString().Should().Be("M");     // preserved
    product.DomainEvents.Should().Contain(e => e is ProductUpdated);
}
```

**Sample code in this template.**
- [`Tests.Unit/Aggregates/ProductTests.cs`](../../src/BytLabs.MicroserviceTemplate.Tests.Unit/Aggregates/ProductTests.cs)

Run: `dotnet test src/BytLabs.MicroserviceTemplate.Tests.Unit`

**Reference (BytLabs.BackendPackages).** xUnit, FluentAssertions (third-party).

**Related recipes.** [Aggregate root](aggregate-root.md), [Acceptance testing with xUnit](acceptance-testing-xunit.md).
