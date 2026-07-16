using BytLabs.MicroserviceTemplate.Client;
using BytLabs.MicroserviceTemplate.Tests.Accpetance.Support;
using FluentAssertions;
using StrawberryShake;
using Xunit;

namespace BytLabs.MicroserviceTemplate.Tests.Accpetance.Graphql;

// GraphQL acceptance behaviours over /graphql via the StrawberryShake client, run against each store via
// the derived classes below. Validates the store-selected query resolvers (Mongo vs EF) behind an
// identical schema, plus the shared mutation → command path.
public abstract class GraphqlTestsBase
{
    private readonly MatrixApiFactory _factory;
    protected GraphqlTestsBase(MatrixApiFactory factory) => _factory = factory;

    private IMicroserviceTemplateGraphQLClient Client => _factory.GraphQLClient;

    [Fact]
    public async Task Create_order_via_mutation_succeeds()
    {
        var orderId = Guid.NewGuid();
        var input = new CreateOrderInputBuilder()
            .WithOrderId(orderId.ToString())
            .WithItems(new List<OrderItemInput>
            {
                new OrderItemInputBuilder()
                    .WithProductId(Guid.NewGuid().ToString())
                    .WithQuantity(3).WithPrice(50).WithId(Guid.NewGuid().ToString()).Build()
            })
            .WithOrderDate(new DateTimeOffset(2000, 1, 1, 0, 0, 0, TimeSpan.Zero))
            .Build();

        var result = await Client.CreateOrder.ExecuteAsync(input, CancellationToken.None);

        result.Data.Should().NotBeNull();
        result.Data!.CreateOrder.Errors.Should().BeNullOrEmpty();
        Guid.Parse(result.Data.CreateOrder.CreateOrderResult!.OrderId).Should().Be(orderId);
    }

    [Fact]
    public async Task Created_order_is_returned_by_orders_query()
    {
        var orderId = Guid.NewGuid();
        var input = new CreateOrderInputBuilder()
            .WithOrderId(orderId.ToString())
            .WithItems(new List<OrderItemInput>())
            .WithOrderDate(new DateTimeOffset(2003, 3, 3, 0, 0, 0, TimeSpan.Zero))
            .Build();
        var create = await Client.CreateOrder.ExecuteAsync(input, CancellationToken.None);
        create.Data!.CreateOrder.Errors.Should().BeNullOrEmpty();

        var query = await Client.GetOrders.ExecuteAsync(
            first: 20, after: null, last: null, before: null, order: null, where: null,
            cancellationToken: CancellationToken.None);

        query.EnsureNoErrors();
        query.Data!.Orders!.TotalCount.Should().BeGreaterThan(0);
        query.Data!.Orders!.Nodes.Should().NotBeNull();
    }
}

[Collection("Mongo")]
public sealed class GraphqlTestsMongo : GraphqlTestsBase
{
    public GraphqlTestsMongo(MongoStoreFixture fixture) : base(fixture.Factory) { }
}

[Collection("Postgres")]
public sealed class GraphqlTestsPostgres : GraphqlTestsBase
{
    public GraphqlTestsPostgres(PostgresStoreFixture fixture) : base(fixture.Factory) { }
}
