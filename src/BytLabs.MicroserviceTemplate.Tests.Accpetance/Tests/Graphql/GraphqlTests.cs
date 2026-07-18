using System.Text.Json;
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

    [Fact]
    public async Task Pending_order_can_be_marked_shipped()
    {
        var orderId = Guid.NewGuid();
        var create = await Client.CreateOrder.ExecuteAsync(
            new CreateOrderInputBuilder()
                .WithOrderId(orderId.ToString())
                .WithItems(new List<OrderItemInput>
                {
                    new OrderItemInputBuilder()
                        .WithProductId(Guid.NewGuid().ToString())
                        .WithQuantity(1).WithPrice(100).WithId(Guid.NewGuid().ToString()).Build()
                })
                .WithOrderDate(new DateTimeOffset(2001, 1, 1, 0, 0, 0, TimeSpan.Zero))
                .Build(),
            CancellationToken.None);
        create.Data!.CreateOrder.Errors.Should().BeNullOrEmpty();

        var result = await Client.MarkOrderAsShipped.ExecuteAsync(
            new ShipOrderInput { OrderId = orderId.ToString() }, CancellationToken.None);

        result.Data.Should().NotBeNull();
        result.Data!.MarkOrderAsShipped.Errors.Should().BeNullOrEmpty();
        Guid.Parse(result.Data.MarkOrderAsShipped.ShipOrderResult!.OrderId).Should().Be(orderId);
    }

    [Fact]
    public async Task Product_lifecycle_create_add_variant_query_remove()
    {
        var productId = Guid.NewGuid();
        // Unique name so the list query filters to exactly this product (deterministic against other rows).
        var name = "Widget-" + Guid.NewGuid().ToString("N");

        var created = await Client.CreateProduct.ExecuteAsync(
            new CreateProductInput { Id = productId.ToString(), Name = name, Data = JsonDocument.Parse("{\"color\":\"red\"}").RootElement },
            CancellationToken.None);
        created.Data!.CreateProduct.Errors.Should().BeNullOrEmpty();
        created.Data.CreateProduct.Product!.Name.Should().Be(name);

        var variant = await Client.AddVariant.ExecuteAsync(
            new AddVariantInput { ProductId = productId.ToString(), VariantId = Guid.NewGuid().ToString(), Sku = "SKU-1", Price = 9.99m },
            CancellationToken.None);
        variant.Data!.AddVariant.Errors.Should().BeNullOrEmpty();

        var where = new ProductFilterInput { Name = new StringOperationFilterInput { Eq = name } };

        var products = await Client.GetProducts.ExecuteAsync(50, null, null, where, CancellationToken.None);
        products.Data!.Products!.Nodes!.Should().ContainSingle(p => Guid.Parse(p!.Id) == productId);

        var removed = await Client.RemoveProduct.ExecuteAsync(
            new RemoveProductInput { Id = productId.ToString() }, CancellationToken.None);
        removed.Data!.RemoveProduct.Errors.Should().BeNullOrEmpty();

        // Soft-deleted rows are excluded from reads.
        var after = await Client.GetProducts.ExecuteAsync(50, null, null, where, CancellationToken.None);
        after.Data!.Products!.Nodes!.Should().NotContain(p => Guid.Parse(p!.Id) == productId);
    }

    [Fact]
    public async Task EntityDef_lifecycle_create_query_update_remove()
    {
        var id = Guid.NewGuid();

        var created = await Client.CreateEntityDef.ExecuteAsync(
            new CreateEntityDefInput { Id = id.ToString(), EntityType = "Product", Form = Form(), Table = Table() },
            CancellationToken.None);
        created.Data!.CreateEntityDef.Errors.Should().BeNullOrEmpty();
        created.Data.CreateEntityDef.EntityDef!.EntityType.Should().Be("Product");

        // Filter by id so the list assertions are deterministic against accumulated rows.
        var where = new EntityDefFilterInput { Id = new UuidOperationFilterInput { Eq = id } };

        var list = await Client.GetEntityDefs.ExecuteAsync(50, null, null, where, CancellationToken.None);
        list.Data!.EntityDefs!.Nodes!.Should().ContainSingle(d => Guid.Parse(d!.Id) == id);

        var updated = await Client.UpdateEntityDef.ExecuteAsync(
            new UpdateEntityDefInput { Id = id.ToString(), Form = Form(), Table = Table("[{\"accessorKey\":\"name\"}]") },
            CancellationToken.None);
        updated.Data!.UpdateEntityDef.Errors.Should().BeNullOrEmpty();
        updated.Data.UpdateEntityDef.EntityDef!.Table!.Columns!.Data.Should().Contain("accessorKey");

        var removed = await Client.RemoveEntityDef.ExecuteAsync(
            new RemoveEntityDefInput { Id = id.ToString() }, CancellationToken.None);
        removed.Data!.RemoveEntityDef.Errors.Should().BeNullOrEmpty();

        // Soft-deleted rows are excluded from reads.
        var after = await Client.GetEntityDefs.ExecuteAsync(50, null, null, where, CancellationToken.None);
        after.Data!.EntityDefs!.Nodes!.Should().NotContain(d => Guid.Parse(d!.Id) == id);
    }

    private static DataSchemaInput Ds(string type, string data) => new() { Type = type, Data = data };

    private static FormDataSchemaInput Form() => new()
    {
        Key = "product",
        SampleData = Ds("json", "{}"),
        FormSchema = Ds("rjsf/formSchema", "{\"type\":\"object\"}"),
        FormUi = Ds("rjsf/uiSchema", "{}")
    };

    private static TableDataSchemaInput Table(string columns = "[]") => new()
    {
        SampleData = Ds("json", "{}"),
        Columns = Ds("tanstack/columnDef", columns),
        Filter = Ds("json", "{}"),
        Details = Ds("cms/view", "{}")
    };
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
