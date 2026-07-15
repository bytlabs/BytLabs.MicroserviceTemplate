using System.Text.Json;
using BytLabs.Application.CQS.Commands;
using BytLabs.MicroserviceTemplate.Application.Products.Dtos;
using BytLabs.MicroserviceTemplate.Domain.Products.DataObjects;

namespace BytLabs.MicroserviceTemplate.Application.Products.Commands.CreateProduct
{
    // Variants is optional (id-less VariantData rows); the domain generates each ProductVariant id.
    public record CreateProductCommand(Guid Id, string Name, JsonElement Data, IEnumerable<VariantData>? Variants = null) : ICommand<ProductDto>;
}
