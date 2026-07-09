using AutoMapper;
using BytLabs.MicroserviceTemplate.Application.Dtos;
using BytLabs.MicroserviceTemplate.Domain.Aggregates.EntityDefAggregate;

namespace BytLabs.MicroserviceTemplate.Application.MappingProfiles
{
    public class EntityDefMappingProfile : Profile
    {
        public EntityDefMappingProfile()
        {
            CreateMap<EntityDef, EntityDefDto>();
        }
    }
}
