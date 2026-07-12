using BytLabs.Application.CQS.Commands;

namespace BytLabs.MicroserviceTemplate.Application.Products.Commands.RemoveVariant
{
    public record RemoveVariantCommand(Guid ProductId, Guid VariantId) : ICommand<RemoveVariantResult>;
    public record RemoveVariantResult(Guid ProductId, Guid VariantId);
}
