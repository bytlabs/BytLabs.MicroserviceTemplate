using BytLabs.Application.CQS.Commands;
using BytLabs.MicroserviceTemplate.Application.Products.Dtos;

namespace BytLabs.MicroserviceTemplate.Application.Products.Commands.AddVariant
{
    public record AddVariantCommand(Guid ProductId, Guid VariantId, string Sku, decimal Price) : ICommand<ProductVariantDto>;
}
