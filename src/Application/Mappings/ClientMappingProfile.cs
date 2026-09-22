using AutoMapper;
using poc_sdd_net_core_api.Application.DTOs;
using poc_sdd_net_core_api.Domain.Entities;

namespace poc_sdd_net_core_api.Application.Mappings;

public class ClientMappingProfile : Profile
{
    public ClientMappingProfile()
    {
        CreateMap<Client, ClientResponse>();
        CreateMap<CreateClientRequest, Client>()
            .ConstructUsing(src => Client.Create(src.Name, src.Email, src.Phone));
        CreateMap<UpdateClientRequest, Client>()
            .ForAllMembers(opts => opts.Ignore());
    }
}
