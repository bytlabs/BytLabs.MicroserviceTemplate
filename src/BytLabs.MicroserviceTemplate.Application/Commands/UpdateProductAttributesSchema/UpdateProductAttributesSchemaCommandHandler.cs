using AutoMapper;
using BytLabs.Application.CQS.Commands;
using BytLabs.Application.DataAccess;
using BytLabs.MicroserviceTemplate.Application.Dtos;
using BytLabs.MicroserviceTemplate.Domain.Aggregates.ProductAggregate;
using BytLabs.MicroserviceTemplate.Domain.Shared.DynamicData;

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
            var s = request.Schema;
            var schema = new FormDataSchema(
                s.Key,
                new DataSchema(s.SampleData.Type, s.SampleData.Data),
                new DataSchema(s.FormSchema.Type, s.FormSchema.Data),
                new DataSchema(s.FormUi.Type, s.FormUi.Data));

            var product = await productRepository.GetByIdAsync(request.Id, cancellationToken);
            product.UpdateAttributesSchema(schema);
            var result = await productRepository.UpdateAsync(product, cancellationToken);
            return mapper.Map<ProductDto>(result);
        }
    }
}
