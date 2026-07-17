using System.Text.Json;
using BytLabs.MicroserviceTemplate.Client;
using BytLabs.MicroserviceTemplate.Tests.Accpetance.Support;
using FluentAssertions;
using StrawberryShake;
using Xunit;
using ValueKind = BytLabs.MicroserviceTemplate.Client.ValueKind;

namespace BytLabs.MicroserviceTemplate.Tests.Accpetance.Graphql;

/// <summary>
/// Verifies dynamic-data (<c>data.*</c>) GraphQL <c>where</c> filtering (equals, contains, greater-than,
/// less-than) and <c>order by</c> on data properties behave identically on both stores. Each test seeds
/// five orders tagged with a unique <c>data.run</c> marker and AND-composes that marker into every query,
/// so results are deterministic regardless of other rows in the shared tenant database.
/// </summary>
/// <remarks>
/// Path conventions (they differ between filter and sort, and the tests rely on it):
/// <list type="bullet">
/// <item><description>Filter path is the bare property name (<c>"price"</c>): Mongo prepends <c>data.</c>,
/// the EF translator reads the <c>Data</c> jsonb column.</description></item>
/// <item><description>Sort path is <c>"data.&lt;prop&gt;"</c>: Mongo uses it as the BSON field path, EF
/// reads <c>data-&gt;&gt;'prop'</c>.</description></item>
/// <item><description>Numeric GT/LT filtering is numeric on both stores. Sorting is asserted on a string
/// property because EF sorts jsonb values as text (lexical), which would diverge from Mongo for
/// numbers.</description></item>
/// </list>
/// </remarks>
public abstract class DynamicDataQueryTestsBase
{
    private readonly MatrixApiFactory _factory;
    protected DynamicDataQueryTestsBase(MatrixApiFactory factory) => _factory = factory;

    private IMicroserviceTemplateGraphQLClient Client => _factory.GraphQLClient;

    private static DataOperationFilterInput Leaf(string path, FilterOperation op, ValueKind kind, string value)
        => new() { Path = path, Operation = op, ValueType = kind, Value = value };

    private static OrderFilterInput DataFilter(DataOperationFilterInput leaf) => new() { Data = leaf };

    // where = (data.run == run) AND (leaf), so only this test's five rows are considered.
    private static OrderFilterInput RunScoped(string run, DataOperationFilterInput leaf) => new()
    {
        And = new List<OrderFilterInput>
        {
            DataFilter(Leaf("run", FilterOperation.Eq, ValueKind.String, run)),
            DataFilter(leaf),
        },
    };

    private async Task<Guid> SeedAsync(string run, string name, string category, int price)
    {
        var id = Guid.NewGuid();
        var data = JsonDocument.Parse(
            $$"""{"run":"{{run}}","name":"{{name}}","category":"{{category}}","price":{{price}}}""").RootElement;
        var result = await Client.CreateOrder.ExecuteAsync(
            new CreateOrderInput
            {
                OrderId = id.ToString(),
                OrderDate = new DateTimeOffset(2001, 1, 1, 0, 0, 0, TimeSpan.Zero),
                Items = new List<OrderItemInput>(),
                Data = data,
            },
            CancellationToken.None);
        result.Data!.CreateOrder.Errors.Should().BeNullOrEmpty();
        return id;
    }

    // Five isolated orders keyed by category: a→price 10 … e→price 50, name = a Greek word per category.
    private async Task<(string run, Dictionary<string, Guid> byCategory)> SeedFiveAsync()
    {
        var run = Guid.NewGuid().ToString("N");
        var byCategory = new Dictionary<string, Guid>
        {
            ["a"] = await SeedAsync(run, "alpha", "a", 10),
            ["b"] = await SeedAsync(run, "beta", "b", 20),
            ["c"] = await SeedAsync(run, "gamma", "c", 30),
            ["d"] = await SeedAsync(run, "delta", "d", 40),
            ["e"] = await SeedAsync(run, "epsilon", "e", 50),
        };
        return (run, byCategory);
    }

    private async Task<List<Guid>> QueryAsync(OrderFilterInput where, IReadOnlyList<OrderSortInput>? order = null)
    {
        var result = await Client.GetOrders.ExecuteAsync(
            first: 50, after: null, last: null, before: null, order: order, where: where,
            cancellationToken: CancellationToken.None);
        result.EnsureNoErrors();
        return result.Data!.Orders!.Nodes!.Select(n => Guid.Parse(n!.Id)).ToList();
    }

    [Fact]
    public async Task Where_equals_on_data_property_returns_only_the_matching_row()
    {
        var (run, ids) = await SeedFiveAsync();

        var matched = await QueryAsync(RunScoped(run, Leaf("category", FilterOperation.Eq, ValueKind.String, "c")));

        matched.Should().BeEquivalentTo(new[] { ids["c"] });
    }

    [Fact]
    public async Task Where_contains_on_data_property_returns_only_the_matching_row()
    {
        var (run, ids) = await SeedFiveAsync();

        // "beta" is the only one of the five names that contains "et".
        var matched = await QueryAsync(RunScoped(run, Leaf("name", FilterOperation.Contains, ValueKind.String, "et")));

        matched.Should().BeEquivalentTo(new[] { ids["b"] });
    }

    [Fact]
    public async Task Where_greater_than_on_numeric_data_property_filters_numerically()
    {
        var (run, ids) = await SeedFiveAsync();

        var matched = await QueryAsync(RunScoped(run, Leaf("price", FilterOperation.Gt, ValueKind.Number, "30")));

        matched.Should().BeEquivalentTo(new[] { ids["d"], ids["e"] });
    }

    [Fact]
    public async Task Where_less_than_on_numeric_data_property_filters_numerically()
    {
        var (run, ids) = await SeedFiveAsync();

        var matched = await QueryAsync(RunScoped(run, Leaf("price", FilterOperation.Lt, ValueKind.Number, "30")));

        matched.Should().BeEquivalentTo(new[] { ids["a"], ids["b"] });
    }

    [Fact]
    public async Task Order_by_data_property_ascending_sorts_the_rows()
    {
        var (run, ids) = await SeedFiveAsync();

        var ordered = await QueryAsync(
            DataFilter(Leaf("run", FilterOperation.Eq, ValueKind.String, run)),
            new List<OrderSortInput> { new() { Path = "data.category", By = SortOrder.Asc } });

        ordered.Should().Equal(ids["a"], ids["b"], ids["c"], ids["d"], ids["e"]);
    }

    [Fact]
    public async Task Order_by_data_property_descending_sorts_the_rows()
    {
        var (run, ids) = await SeedFiveAsync();

        var ordered = await QueryAsync(
            DataFilter(Leaf("run", FilterOperation.Eq, ValueKind.String, run)),
            new List<OrderSortInput> { new() { Path = "data.category", By = SortOrder.Desc } });

        ordered.Should().Equal(ids["e"], ids["d"], ids["c"], ids["b"], ids["a"]);
    }
}

[Collection("Mongo")]
public sealed class DynamicDataQueryTestsMongo : DynamicDataQueryTestsBase
{
    public DynamicDataQueryTestsMongo(MongoStoreFixture fixture) : base(fixture.Factory) { }
}

[Collection("Postgres")]
public sealed class DynamicDataQueryTestsPostgres : DynamicDataQueryTestsBase
{
    public DynamicDataQueryTestsPostgres(PostgresStoreFixture fixture) : base(fixture.Factory) { }
}
