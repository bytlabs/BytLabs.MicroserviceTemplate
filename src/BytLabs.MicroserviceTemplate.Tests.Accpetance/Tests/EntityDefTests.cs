using BytLabs.MicroserviceTemplate.Client;
using BytLabs.MicroserviceTemplate.Tests.Accpetance.Support;
using StrawberryShake;

namespace BytLabs.MicroserviceTemplate.Tests.Accpetance.Tests;

public class EntityDefTests
{
    private readonly IMicroserviceTemplateGraphQLClient _client;

    public EntityDefTests()
    {
        _client = new CustomWebApplicationFactory<Program>().GetGraphQLClient();
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

    [Fact]
    public async Task EntityDef_lifecycle_create_query_update_remove()
    {
        var id = Guid.NewGuid();

        var created = await _client.CreateEntityDef.ExecuteAsync(
            new CreateEntityDefInput { Id = id.ToString(), EntityType = "Product", Form = Form(), Table = Table() },
            CancellationToken.None);
        created.Data!.CreateEntityDef.Errors.Should().BeNullOrEmpty();
        created.Data.CreateEntityDef.EntityDef!.EntityType.Should().Be("Product");

        var list = await _client.GetEntityDefs.ExecuteAsync(50, null, null, null, CancellationToken.None);
        list.Data!.EntityDefs!.Nodes!.Should().Contain(d => Guid.Parse(d!.Id) == id);

        var updated = await _client.UpdateEntityDef.ExecuteAsync(
            new UpdateEntityDefInput { Id = id.ToString(), Form = Form(), Table = Table("[{\"accessorKey\":\"name\"}]") },
            CancellationToken.None);
        updated.Data!.UpdateEntityDef.Errors.Should().BeNullOrEmpty();
        updated.Data.UpdateEntityDef.EntityDef!.Table!.Columns!.Data.Should().Contain("accessorKey");

        var removed = await _client.RemoveEntityDef.ExecuteAsync(
            new RemoveEntityDefInput { Id = id.ToString() }, CancellationToken.None);
        removed.Data!.RemoveEntityDef.Errors.Should().BeNullOrEmpty();

        var after = await _client.GetEntityDefs.ExecuteAsync(50, null, null, null, CancellationToken.None);
        after.Data!.EntityDefs!.Nodes!.Should().NotContain(d => Guid.Parse(d!.Id) == id);
    }
}
