using AutoMapper;
using poc_sdd_net_core_api.Application.DTOs;
using poc_sdd_net_core_api.Domain.Entities;

namespace poc_sdd_net_core_api.Application.Mappings;

public class SectorMappingProfile : Profile
{
    public SectorMappingProfile()
    {
        CreateMap<Sector, SectorResponse>();
    }
}
