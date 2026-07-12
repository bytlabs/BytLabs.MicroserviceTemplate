using BytLabs.Application.CQS.Commands;
using BytLabs.Application.DataAccess;
using BytLabs.MicroserviceTemplate.Domain.Products;

namespace BytLabs.MicroserviceTemplate.Application.Products.Commands.RemoveProduct
{
    public class RemoveProductCommandHandler : ICommandHandler<RemoveProductCommand, RemoveProductResult>
    {
        private readonly IRepository<Product, Guid> productRepository;

        public RemoveProductCommandHandler(IRepository<Product, Guid> productRepository)
        {
            this.productRepository = productRepository;
        }

        public async Task<RemoveProductResult> Handle(RemoveProductCommand request, CancellationToken cancellationToken)
        {
            var product = await productRepository.GetByIdAsync(request.Id, cancellationToken);
            product.Remove(); // soft delete
            await productRepository.UpdateAsync(product, cancellationToken);
            return new RemoveProductResult(request.Id);
        }
    }
}
