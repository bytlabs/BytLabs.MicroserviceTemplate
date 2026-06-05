using AutoMapper;
using BytLabs.Application.CQS.Commands;
using BytLabs.Application.DataAccess;
using BytLabs.MicroserviceTemplate.Application.Dtos;
using BytLabs.MicroserviceTemplate.Domain.Aggregates.ProductAggregate;
using BytLabs.MicroserviceTemplate.Domain.Aggregates.ProductAggregate.DataObjects;

namespace BytLabs.MicroserviceTemplate.Application.Commands.AddVariant
{
    public class AddVariantCommandHandler : ICommandHandler<AddVariantCommand, ProductVariantDto>
    {
        private readonly IRepository<Product, Guid> productRepository;
        private readonly IMapper mapper;

        public AddVariantCommandHandler(IRepository<Product, Guid> productRepository, IMapper mapper)
        {
            this.productRepository = productRepository;
            this.mapper = mapper;
        }

        public async Task<ProductVariantDto> Handle(AddVariantCommand request, CancellationToken cancellationToken)
        {
            var variant = ProductVariant.Create(new CreateVariant(request.VariantId, request.Sku, request.Price));
            var product = await productRepository.GetByIdAsync(request.ProductId, cancellationToken);
            product.AddVariant(new(variant));
            await productRepository.UpdateAsync(product, cancellationToken);
            return mapper.Map<ProductVariantDto>(variant);
        }
    }
}
