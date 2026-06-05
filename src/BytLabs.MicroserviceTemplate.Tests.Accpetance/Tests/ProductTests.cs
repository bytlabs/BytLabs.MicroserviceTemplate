using System.Text.Json;
using BytLabs.MicroserviceTemplate.Client;
using BytLabs.MicroserviceTemplate.Tests.Accpetance.Support;
using StrawberryShake;

namespace BytLabs.MicroserviceTemplate.Tests.Accpetance.Tests;

public class ProductTests
{
    private readonly IMicroserviceTemplateGraphQLClient _client;

    public ProductTests()
    {
        _client = new CustomWebApplicationFactory<Program>().GetGraphQLClient();
    }

    [Fact]
    public async Task Product_lifecycle_create_add_variant_query_remove()
    {
        var productId = Guid.NewGuid();

        // create
        var createInput = new CreateProductInput
        {
            Id = productId.ToString(),
            Name = "Widget",
            Data = JsonDocument.Parse("{\"color\":\"red\"}").RootElement
        };
        var created = await _client.CreateProduct.ExecuteAsync(createInput, CancellationToken.None);
        created.Data!.CreateProduct.Errors.Should().BeNullOrEmpty();
        created.Data.CreateProduct.Product!.Name.Should().Be("Widget");

        // add a variant
        var addVariant = new AddVariantInput
        {
            ProductId = productId.ToString(),
            VariantId = Guid.NewGuid().ToString(),
            Sku = "SKU-1",
            Price = 9.99m
        };
        var variantResult = await _client.AddVariant.ExecuteAsync(addVariant, CancellationToken.None);
        variantResult.Data!.AddVariant.Errors.Should().BeNullOrEmpty();

        // query (excludes soft-deleted)
        var products = await _client.GetProducts.ExecuteAsync(50, null, null, null, CancellationToken.None);
        products.Data!.Products!.Nodes!.Should().Contain(p => Guid.Parse(p!.Id) == productId);

        // soft-delete
        var removed = await _client.RemoveProduct.ExecuteAsync(
            new RemoveProductInput { Id = productId.ToString() }, CancellationToken.None);
        removed.Data!.RemoveProduct.Errors.Should().BeNullOrEmpty();

        // verify excluded from query after soft-delete
        var after = await _client.GetProducts.ExecuteAsync(50, null, null, null, CancellationToken.None);
        after.Data!.Products!.Nodes!.Should().NotContain(p => Guid.Parse(p!.Id) == productId);
    }
}
