using BytLabs.Hotchocolate.ObjectTypes;
using BytLabs.MicroserviceTemplate.Application.Products.Dtos;
using HotChocolate.Types;

namespace BytLabs.MicroserviceTemplate.Infrastructure.HotChocolate
{
    // RECIPE: custom DtoType adding a computed field resolved from the parent DTO.
    public class ProductDtoType : DtoType<ProductDto>
    {
        protected override void Configure(IObjectTypeDescriptor<ProductDto> descriptor)
        {
            base.Configure(descriptor);

            descriptor.Field("variantCount")
                .Type<NonNullType<IntType>>()
                .Resolve(context => context.Parent<ProductDto>().Variants?.Count ?? 0);
        }
    }
}
