using System.Net;
using System.Text;
using System.Text.Json;
using BytLabs.MicroserviceTemplate.Tests.Accpetance.Support;
using FluentAssertions;
using Xunit;

namespace BytLabs.MicroserviceTemplate.Tests.Accpetance.Rest;

// REST/OData acceptance behaviours over the /odata surface, run against each store via the derived
// classes below. Assertions are id-scoped so tests share the Postgres "Test" database without collisions.
public abstract class RestTestsBase
{
    private readonly MatrixApiFactory _factory;
    protected RestTestsBase(MatrixApiFactory factory) => _factory = factory;

    private HttpClient Client() => _factory.CreateTenantClient();

    private static StringContent Json(string body) => new(body, Encoding.UTF8, "application/json");

    [Fact]
    public async Task Metadata_lists_the_entity_sets()
    {
        var body = await Client().GetStringAsync("/odata/$metadata");
        body.Should().Contain("Orders").And.Contain("Products").And.Contain("EntityDefs");
    }

    [Fact]
    public async Task Create_then_read_order_round_trips_data_and_items()
    {
        var http = Client();
        var id = Guid.NewGuid();
        var itemId = Guid.NewGuid();
        var post = await http.PostAsync("/odata/Orders", Json(
            $$"""
            { "Id":"{{id}}", "OrderDate":"2000-01-01T00:00:00Z", "Data":"{\"priority\":\"high\"}",
              "Items":[ { "Id":"{{itemId}}", "ProductId":"{{Guid.NewGuid()}}", "Quantity":2, "Price":100 } ] }
            """));
        post.StatusCode.Should().Be(HttpStatusCode.Created);

        using var doc = JsonDocument.Parse(await http.GetStringAsync($"/odata/Orders({id})"));
        var root = doc.RootElement;
        root.GetProperty("OrderDate").GetString().Should().StartWith("2000-01-01T00:00:00");
        root.GetProperty("Data").GetString().Should().Contain("priority");
        var items = root.GetProperty("Items");
        items.GetArrayLength().Should().Be(1);
        items[0].GetProperty("Id").GetGuid().Should().Be(itemId);
        items[0].GetProperty("Quantity").GetInt32().Should().Be(2);
    }

    [Fact]
    public async Task Removed_order_is_excluded_from_reads()
    {
        var http = Client();
        var id = Guid.NewGuid();
        (await http.PostAsync("/odata/Orders", Json(
            $$"""{ "Id":"{{id}}", "OrderDate":"2001-01-01T00:00:00Z", "Data":"{}", "Items":[] }""")))
            .StatusCode.Should().Be(HttpStatusCode.Created);

        (await http.DeleteAsync($"/odata/Orders({id})")).StatusCode.Should().Be(HttpStatusCode.NoContent);

        (await http.GetAsync($"/odata/Orders({id})")).StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task Create_product_with_variant_round_trips()
    {
        var http = Client();
        var id = Guid.NewGuid();
        var variantId = Guid.NewGuid();
        (await http.PostAsync("/odata/Products", Json(
            $$"""
            { "Id":"{{id}}", "Name":"Widget", "Data":"{\"color\":\"red\"}",
              "Variants":[ { "Id":"{{variantId}}", "Sku":"SKU-1", "Price":9.99 } ] }
            """)))
            .StatusCode.Should().Be(HttpStatusCode.Created);

        using var doc = JsonDocument.Parse(await http.GetStringAsync($"/odata/Products({id})"));
        var root = doc.RootElement;
        root.GetProperty("Name").GetString().Should().Be("Widget");
        root.GetProperty("Data").GetString().Should().Contain("color");
        var variants = root.GetProperty("Variants");
        variants.GetArrayLength().Should().Be(1);
        variants[0].GetProperty("Sku").GetString().Should().Be("SKU-1");
    }
}

[Collection("Mongo")]
public sealed class RestTestsMongo : RestTestsBase
{
    public RestTestsMongo(MongoStoreFixture fixture) : base(fixture.Factory) { }
}

[Collection("Postgres")]
public sealed class RestTestsPostgres : RestTestsBase
{
    public RestTestsPostgres(PostgresStoreFixture fixture) : base(fixture.Factory) { }
}
