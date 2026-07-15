using AutoMapper;
using BytLabs.MicroserviceTemplate.Application.EntityDefs.Dtos;
using BytLabs.MicroserviceTemplate.Domain.EntityDefs.Aggregates;

namespace BytLabs.MicroserviceTemplate.Application.EntityDefs.Mapping
{
    public class EntityDefMappingProfile : Profile
    {
        public EntityDefMappingProfile()
        {
            CreateMap<EntityDef, EntityDefDto>();
        }
    }
}
