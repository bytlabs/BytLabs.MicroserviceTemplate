using BytLabs.MicroserviceTemplate.Client;
using BytLabs.MicroserviceTemplate.Tests.Accpetance.Support;
using FluentAssertions;
using StrawberryShake;
using Xunit;

namespace BytLabs.MicroserviceTemplate.Tests.Accpetance.Graphql;

/// <summary>
/// Scalar <c>where</c> filtering over the GraphQL query root (e.g. <c>entityType: { eq }</c>) — the
/// filtering the console's useEntityDef relies on to load one entity type's definition.
/// Passes on Mongo (the aggregate query applies HotChocolate's filter). Currently fails on the
/// EF/Postgres path, where scalar filtering is a no-op (only the dynamic <c>data</c> filter is
/// translated), so the whole set is returned instead of the requested type.
/// </summary>
public abstract class ScalarFilterTestsBase
{
    private readonly MatrixApiFactory _factory;
    protected ScalarFilterTestsBase(MatrixApiFactory factory) => _factory = factory;
    private IMicroserviceTemplateGraphQLClient Client => _factory.GraphQLClient;

    private static DataSchemaInput Ds(string t, string d) => new() { Type = t, Data = d };
    private static FormDataSchemaInput Form(string key) => new()
    { Key = key, SampleData = Ds("json", "{}"), FormSchema = Ds("rjsf/formSchema", "{\"type\":\"object\"}"), FormUi = Ds("rjsf/uiSchema", "{}") };
    private static TableDataSchemaInput Table() => new()
    { SampleData = Ds("json", "{}"), Columns = Ds("tanstack/columnDef", "[]"), Filter = Ds("json", "{}"), Details = Ds("cms/view", "{}") };

    private async Task Create(string type)
    {
        var r = await Client.CreateEntityDef.ExecuteAsync(
            new CreateEntityDefInput { Id = Guid.NewGuid().ToString(), EntityType = type, Form = Form(type), Table = Table() },
            CancellationToken.None);
        r.Data!.CreateEntityDef.Errors.Should().BeNullOrEmpty();
    }

    [Fact]
    public async Task EntityDefs_scalar_where_filters_by_entityType()
    {
        var run = Guid.NewGuid().ToString("N")[..8];
        var typeA = $"ScalarA_{run}";
        var typeB = $"ScalarB_{run}";
        await Create(typeA);
        await Create(typeB);

        var res = await Client.GetEntityDefs.ExecuteAsync(
            first: 50, after: null, order: null,
            where: new EntityDefFilterInput { EntityType = new StringOperationFilterInput { Eq = typeA } },
            cancellationToken: CancellationToken.None);
        res.EnsureNoErrors();

        var types = res.Data!.EntityDefs!.Nodes!.Select(n => n!.EntityType).ToList();

        types.Should().Contain(typeA);
        types.Should().OnlyContain(t => t == typeA, "the scalar entityType filter must exclude every other type");
    }
}

[Collection("Mongo")]
public sealed class ScalarFilterTestsMongo : ScalarFilterTestsBase
{
    public ScalarFilterTestsMongo(MongoStoreFixture f) : base(f.Factory) { }
}

[Collection("Postgres")]
public sealed class ScalarFilterTestsPostgres : ScalarFilterTestsBase
{
    public ScalarFilterTestsPostgres(PostgresStoreFixture f) : base(f.Factory) { }
}
