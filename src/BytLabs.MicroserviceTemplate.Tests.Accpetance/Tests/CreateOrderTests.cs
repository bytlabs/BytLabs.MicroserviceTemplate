using BytLabs.MicroserviceTemplate.Client;
using BytLabs.MicroserviceTemplate.Tests.Accpetance.Support;
using StrawberryShake;

namespace BytLabs.MicroserviceTemplate.Tests.Accpetance.Tests;

public class CreateOrderTests
{
    private readonly IMicroserviceTemplateGraphQLClient _client;

    public CreateOrderTests()
    {
        _client = new CustomWebApplicationFactory<Program>().GetGraphQLClient();
    }

    [Fact]
    public async Task Valid_input_creates_an_order()
    {
        var orderId = Guid.NewGuid();
        var input = new CreateOrderInputBuilder()
            .WithOrderId(orderId.ToString())
            .WithItems(new List<OrderItemInput>
            {
                new OrderItemInputBuilder()
                    .WithProductId(GuidExtensions.GUID_0002)
                    // Unique per run: on Postgres, OrderItems is a relational table whose PK is the item
                    // id, so a fixed id collides across runs / parallel tests sharing the same database.
                    .WithQuantity(1).WithPrice(100).WithId(Guid.NewGuid().ToString()).Build()
            })
            .WithOrderDate(DateTimeExtensions.DDMMYYYY_01_01_2000)
            .Build();

        var result = await _client.CreateOrder.ExecuteAsync(input, CancellationToken.None);

        result.Data.Should().NotBeNull();
        result.Data!.CreateOrder.Errors.Should().BeNullOrEmpty();
        Guid.Parse(result.Data.CreateOrder.CreateOrderResult!.OrderId).Should().Be(orderId);
    }

    [Fact]
    public async Task Order_without_items_is_allowed()
    {
        // Order is now a flexible dynamic entity: an order can be created with no line items (its
        // dynamic fields live in `data`), so an empty items list is valid input.
        var orderId = Guid.NewGuid();
        var input = new CreateOrderInputBuilder()
            .WithOrderId(orderId.ToString())
            .WithItems(new List<OrderItemInput>())
            .WithOrderDate(DateTimeExtensions.DDMMYYYY_01_01_2000)
            .Build();

        var result = await _client.CreateOrder.ExecuteAsync(input, CancellationToken.None);

        result.Data!.CreateOrder.Errors.Should().BeNullOrEmpty();
        Guid.Parse(result.Data.CreateOrder.CreateOrderResult!.OrderId).Should().Be(orderId);
    }
}
