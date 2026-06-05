using BytLabs.Application.CQS.Commands;
using BytLabs.MicroserviceTemplate.Application.Dtos;

namespace BytLabs.MicroserviceTemplate.Application.Commands.AddVariant
{
    public record AddVariantCommand(Guid ProductId, Guid VariantId, string Sku, decimal Price) : ICommand<ProductVariantDto>;
}
