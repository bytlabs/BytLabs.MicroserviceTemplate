using System.Text.Json;
using BytLabs.MicroserviceTemplate.Domain.Orders;
using BytLabs.MicroserviceTemplate.Domain.Orders.DataObjects;
using BytLabs.MicroserviceTemplate.Domain.Orders.Events;
using FluentAssertions;
using Xunit;

namespace BytLabs.MicroserviceTemplate.Tests.Unit.Aggregates;

public class OrderTests
{
    private static JsonElement Json(string raw) => JsonDocument.Parse(raw).RootElement;

    private static Order NewOrder(params OrderItem[] items)
        => Order.Create(Guid.NewGuid(), DateTime.UtcNow, items.ToHashSet(), Json("{\"priority\":\"low\"}"));

    [Fact]
    public void Update_replaces_items_when_provided()
    {
        var order = NewOrder(new OrderItem(Guid.NewGuid(), 1, 10m));
        var newProduct = Guid.NewGuid();

        order.Update(new UpdateOrder(Json("{\"priority\":\"high\"}"), new[] { new OrderItem(newProduct, 2, 50m) }.ToHashSet()));

        order.Items.Should().ContainSingle();
        order.Items.Single().ProductId.Should().Be(newProduct);
        order.Items.Single().Quantity.Should().Be(2);
        order.Data.GetProperty("priority").GetString().Should().Be("high"); // merged
        order.DomainEvents.Should().Contain(e => e is OrderUpdated);
    }

    [Fact]
    public void Update_keeps_items_when_not_provided()
    {
        var order = NewOrder(new OrderItem(Guid.NewGuid(), 1, 10m));

        order.Update(new UpdateOrder(Json("{}")));

        order.Items.Should().ContainSingle(); // untouched because Items was null
    }
}
