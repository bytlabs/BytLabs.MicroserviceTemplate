using AutoMapper;
using BytLabs.MicroserviceTemplate.Domain.Aggregates.OrderAggregate;
using BytLabs.MicroserviceTemplate.Application.Dtos;

namespace BytLabs.MicroserviceTemplate.Application.MappingProfiles
{
    public class OrderMappingProfile : Profile
    {
        public OrderMappingProfile()
        {
            CreateMap<Order, OrderDto>();
            CreateMap<OrderItem, OrderItemDto>();
        }
    }
}
