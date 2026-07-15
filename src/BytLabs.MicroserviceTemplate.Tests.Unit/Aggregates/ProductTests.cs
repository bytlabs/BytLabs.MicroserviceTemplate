using System.Text.Json;
using BytLabs.MicroserviceTemplate.Domain.Products.Aggregates;
using BytLabs.MicroserviceTemplate.Domain.Products.Entities;
using BytLabs.MicroserviceTemplate.Domain.Products.Inputs;
using BytLabs.MicroserviceTemplate.Domain.Products.Events;
using FluentAssertions;
using Xunit;

namespace BytLabs.MicroserviceTemplate.Tests.Unit.Aggregates;

public class ProductTests
{
    private static JsonElement Json(string raw) => JsonDocument.Parse(raw).RootElement;

    private static CreateProduct NewCreate(string name = "Widget", string data = "{\"color\":\"red\"}")
        => new(Guid.NewGuid(), name, Json(data));

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
    public void Create_with_variants_generates_ids()
    {
        var product = Product.Create(new CreateProduct(Guid.NewGuid(), "Widget", Json("{}"),
            new[] { new VariantData("SKU-A", 10m), new VariantData("SKU-B", 20m) }));

        product.Variants.Should().HaveCount(2);
        product.Variants.Select(v => v.Sku).Should().BeEquivalentTo(new[] { "SKU-A", "SKU-B" });
        product.Variants.Should().OnlyContain(v => v.Id != Guid.Empty); // domain-generated ids
    }

    [Fact]
    public void Update_replaces_variants_when_provided()
    {
        var product = Product.Create(new CreateProduct(Guid.NewGuid(), "Widget", Json("{}"),
            new[] { new VariantData("OLD", 1m) }));

        product.Update(new UpdateProduct("Widget", Json("{}"), new[] { new VariantData("NEW", 2m) }));

        product.Variants.Should().ContainSingle(v => v.Sku == "NEW");
    }
}
