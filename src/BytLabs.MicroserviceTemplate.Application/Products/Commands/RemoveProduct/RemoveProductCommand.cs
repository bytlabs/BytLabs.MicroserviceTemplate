using BytLabs.Application.CQS.Commands;

namespace BytLabs.MicroserviceTemplate.Application.Products.Commands.RemoveProduct
{
    public record RemoveProductCommand(Guid Id) : ICommand<RemoveProductResult>;
    public record RemoveProductResult(Guid Id);
}
