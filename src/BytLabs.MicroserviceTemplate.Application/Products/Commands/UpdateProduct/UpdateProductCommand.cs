using System.Text.Json;
using BytLabs.Application.CQS.Commands;
using BytLabs.MicroserviceTemplate.Application.Products.Dtos;
using BytLabs.MicroserviceTemplate.Domain.Products.Inputs;

namespace BytLabs.MicroserviceTemplate.Application.Products.Commands.UpdateProduct
{
    // Variants, when provided, replaces the product's variant collection (id-less rows; domain generates ids).
    public record UpdateProductCommand(Guid Id, string Name, JsonElement Data, IEnumerable<VariantData>? Variants = null) : ICommand<ProductDto>;
}
