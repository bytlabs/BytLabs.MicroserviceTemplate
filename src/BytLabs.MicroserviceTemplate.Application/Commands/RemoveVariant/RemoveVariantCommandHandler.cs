using BytLabs.Application.CQS.Commands;
using BytLabs.Application.DataAccess;
using BytLabs.MicroserviceTemplate.Domain.Aggregates.ProductAggregate;
using BytLabs.MicroserviceTemplate.Domain.Aggregates.ProductAggregate.DataObjects;

namespace BytLabs.MicroserviceTemplate.Application.Commands.RemoveVariant
{
    public class RemoveVariantCommandHandler : ICommandHandler<RemoveVariantCommand, RemoveVariantResult>
    {
        private readonly IRepository<Product, Guid> productRepository;

        public RemoveVariantCommandHandler(IRepository<Product, Guid> productRepository)
        {
            this.productRepository = productRepository;
        }

        public async Task<RemoveVariantResult> Handle(RemoveVariantCommand request, CancellationToken cancellationToken)
        {
            var product = await productRepository.GetByIdAsync(request.ProductId, cancellationToken);
            product.RemoveVariant(new(request.VariantId));
            await productRepository.UpdateAsync(product, cancellationToken);
            return new RemoveVariantResult(request.ProductId, request.VariantId);
        }
    }
}
