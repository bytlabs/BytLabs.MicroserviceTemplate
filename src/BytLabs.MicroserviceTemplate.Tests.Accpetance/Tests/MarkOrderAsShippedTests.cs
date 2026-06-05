using BytLabs.MicroserviceTemplate.Client;
using BytLabs.MicroserviceTemplate.Tests.Accpetance.Support;
using StrawberryShake;

namespace BytLabs.MicroserviceTemplate.Tests.Accpetance.Tests;

public class MarkOrderAsShippedTests
{
    private readonly IMicroserviceTemplateGraphQLClient _client;

    public MarkOrderAsShippedTests()
    {
        _client = new CustomWebApplicationFactory<Program>().GetGraphQLClient();
    }

    [Fact]
    public async Task Pending_order_can_be_marked_shipped()
    {
        var orderId = Guid.NewGuid();
        var createInput = new CreateOrderInputBuilder()
            .WithOrderId(orderId.ToString())
            .WithItems(new List<OrderItemInput>
            {
                new OrderItemInputBuilder()
                    .WithProductId(GuidExtensions.GUID_0002)
                    .WithQuantity(1).WithPrice(100).WithId(GuidExtensions.GUID_0003).Build()
            })
            .WithOrderDate(DateTimeExtensions.DDMMYYYY_01_01_2000)
            .Build();
        await _client.CreateOrder.ExecuteAsync(createInput, CancellationToken.None);

        var shipInput = new ShipOrderInput { OrderId = orderId.ToString() };
        var result = await _client.MarkOrderAsShipped.ExecuteAsync(shipInput, CancellationToken.None);

        result.Data.Should().NotBeNull();
        result.Data!.MarkOrderAsShipped.Errors.Should().BeNullOrEmpty();
        Guid.Parse(result.Data.MarkOrderAsShipped.ShipOrderResult!.OrderId).Should().Be(orderId);
    }
}
