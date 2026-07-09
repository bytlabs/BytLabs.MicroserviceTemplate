using BytLabs.MicroserviceTemplate.Domain.Aggregates.EntityDefAggregate;
using BytLabs.MicroserviceTemplate.Domain.Aggregates.EntityDefAggregate.DataObjects;
using BytLabs.MicroserviceTemplate.Domain.Aggregates.EntityDefAggregate.Events;
using BytLabs.MicroserviceTemplate.Domain.Shared.DynamicData;
using FluentAssertions;
using Xunit;

namespace BytLabs.MicroserviceTemplate.Tests.Unit.Aggregates;

public class EntityDefTests
{
    private static FormDataSchema Form(string schema = "{\"type\":\"object\"}") => new(
        "product",
        new DataSchema("json", "{}"),
        new DataSchema("rjsf/formSchema", schema),
        new DataSchema("rjsf/uiSchema", "{}"));

    private static TableDataSchema Table(string columns = "[]") => new(
        new DataSchema("json", "{}"),
        new DataSchema("tanstack/columnDef", columns),
        new DataSchema("json", "{}"),
        new DataSchema("cms/view", "{}"));

    private static CreateEntityDef NewCreate(string entityType = "Product")
        => new(Guid.NewGuid(), entityType, Form(), Table());

    [Fact]
    public void Create_sets_fields_and_raises_EntityDefCreated()
    {
        var cmd = NewCreate();

        var def = EntityDef.Create(cmd);

        def.Id.Should().Be(cmd.Id);
        def.EntityType.Should().Be("Product");
        def.Form.Should().Be(cmd.Form);
        def.Table.Should().Be(cmd.Table);
        def.IsDeleted.Should().BeFalse();
        def.DomainEvents.Should().ContainSingle(e => e is EntityDefCreated);
    }

    [Fact]
    public void Update_replaces_form_and_table_and_raises_EntityDefUpdated()
    {
        var def = EntityDef.Create(NewCreate());
        var newForm = Form("{\"type\":\"object\",\"properties\":{}}");
        var newTable = Table("[{\"accessorKey\":\"name\"}]");

        def.Update(new UpdateEntityDef(newForm, newTable));

        def.Form.Should().Be(newForm);
        def.Table.Should().Be(newTable);
        def.DomainEvents.Should().Contain(e => e is EntityDefUpdated);
    }

    [Fact]
    public void Remove_sets_IsDeleted_and_raises_EntityDefRemoved()
    {
        var def = EntityDef.Create(NewCreate());

        def.Remove();

        def.IsDeleted.Should().BeTrue();
        def.DomainEvents.Should().Contain(e => e is EntityDefRemoved);
    }
}
