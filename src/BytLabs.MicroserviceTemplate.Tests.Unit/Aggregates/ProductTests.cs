using System.Text.Json;
using BytLabs.MicroserviceTemplate.Domain.Aggregates.ProductAggregate;
using BytLabs.MicroserviceTemplate.Domain.Aggregates.ProductAggregate.DataObjects;
using BytLabs.MicroserviceTemplate.Domain.Aggregates.ProductAggregate.Events;
using BytLabs.MicroserviceTemplate.Domain.Shared.DynamicData;
using FluentAssertions;
using Xunit;

namespace BytLabs.MicroserviceTemplate.Tests.Unit.Aggregates;

public class ProductTests
{
    private static JsonElement Json(string raw) => JsonDocument.Parse(raw).RootElement;

    private static CreateProduct NewCreate(string name = "Widget", string data = "{\"color\":\"red\"}")
        => new(Guid.NewGuid(), name, Json(data), FormDataSchema.Empty());

    [Fact]
    public void Create_sets_fields_and_raises_ProductCreated()
    {
        var cmd = NewCreate();
        var product = Product.Create(cmd);

        product.Id.Should().Be(cmd.Id);
        product.Name.Should().Be("Widget");
        product.IsDeleted.Should().BeFalse();
        product.Variants.Should().BeEmpty();
        product.DomainEvents.Should().ContainSingle(e => e is ProductCreated);
    }

    [Fact]
    public void Update_merges_dynamic_data_and_overwrites_name()
    {
        var product = Product.Create(NewCreate(data: "{\"color\":\"red\",\"size\":\"M\"}"));

        product.Update(new UpdateProduct("Widget v2", Json("{\"color\":\"blue\"}")));

        product.Name.Should().Be("Widget v2");
        var data = product.Data;
        data.GetProperty("color").GetString().Should().Be("blue"); // overwritten
        data.GetProperty("size").GetString().Should().Be("M");     // preserved by merge
        product.DomainEvents.Should().Contain(e => e is ProductUpdated);
    }

    [Fact]
    public void Remove_sets_IsDeleted_and_raises_ProductRemoved()
    {
        var product = Product.Create(NewCreate());

        product.Remove();

        product.IsDeleted.Should().BeTrue();
        product.DomainEvents.Should().Contain(e => e is ProductRemoved);
    }

    [Fact]
    public void AddVariant_then_RemoveVariant_mutates_set_and_raises_events()
    {
        var product = Product.Create(NewCreate());
        var variant = ProductVariant.Create(new CreateVariant(Guid.NewGuid(), "SKU-1", 9.99m));

        product.AddVariant(new AddVariant(variant));
        product.Variants.Should().ContainSingle(v => v.Id == variant.Id);
        product.DomainEvents.Should().Contain(e => e is ProductVariantAdded);

        product.RemoveVariant(new RemoveVariant(variant.Id));
        product.Variants.Should().BeEmpty();
        product.DomainEvents.Should().Contain(e => e is ProductVariantRemoved);
    }

    [Fact]
    public void UpdateAttributesSchema_replaces_schema_and_raises_event()
    {
        var product = Product.Create(NewCreate());
        var schema = new FormDataSchema(
            "specs",
            new DataSchema("json", "{}"),
            new DataSchema("schema", "{\"type\":\"object\"}"),
            new DataSchema("ui", "{}"));

        product.UpdateAttributesSchema(schema);

        product.AttributesSchema.Key.Should().Be("specs");
        product.DomainEvents.Should().Contain(e => e is ProductAttributesSchemaUpdated);
    }
}
