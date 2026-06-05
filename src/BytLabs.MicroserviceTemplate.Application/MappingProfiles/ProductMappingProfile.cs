using AutoMapper;
using BytLabs.MicroserviceTemplate.Application.Dtos;
using BytLabs.MicroserviceTemplate.Domain.Aggregates.ProductAggregate;

namespace BytLabs.MicroserviceTemplate.Application.MappingProfiles
{
    public class ProductMappingProfile : Profile
    {
        public ProductMappingProfile()
        {
            CreateMap<Product, ProductDto>();
            CreateMap<ProductVariant, ProductVariantDto>();
        }
    }
}
