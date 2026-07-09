using AutoMapper;
using BytLabs.Application.CQS.Commands;
using BytLabs.Application.DataAccess;
using BytLabs.MicroserviceTemplate.Application.Dtos;
using BytLabs.MicroserviceTemplate.Domain.Aggregates.ProductAggregate;

namespace BytLabs.MicroserviceTemplate.Application.Commands.UpdateProductAttributesSchema
{
    public class UpdateProductAttributesSchemaCommandHandler : ICommandHandler<UpdateProductAttributesSchemaCommand, ProductDto>
    {
        private readonly IRepository<Product, Guid> productRepository;
        private readonly IMapper mapper;

        public UpdateProductAttributesSchemaCommandHandler(IRepository<Product, Guid> productRepository, IMapper mapper)
        {
            this.productRepository = productRepository;
            this.mapper = mapper;
        }

        public async Task<ProductDto> Handle(UpdateProductAttributesSchemaCommand request, CancellationToken cancellationToken)
        {
            var product = await productRepository.GetByIdAsync(request.Id, cancellationToken);
            product.UpdateAttributesSchema(request.Schema);
            var result = await productRepository.UpdateAsync(product, cancellationToken);
            return mapper.Map<ProductDto>(result);
        }
    }
}
