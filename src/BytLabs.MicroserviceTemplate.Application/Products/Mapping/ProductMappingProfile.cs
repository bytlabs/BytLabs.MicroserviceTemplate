using AutoMapper;
using BytLabs.MicroserviceTemplate.Application.Products.Dtos;
using BytLabs.MicroserviceTemplate.Domain.Products.Aggregates;
using BytLabs.MicroserviceTemplate.Domain.Products.Entities;

namespace BytLabs.MicroserviceTemplate.Application.Products.Mapping
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
