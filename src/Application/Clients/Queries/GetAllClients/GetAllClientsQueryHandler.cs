using AutoMapper;
using MediatR;
using poc_sdd_net_core_api.Application.DTOs;
using poc_sdd_net_core_api.Domain.Interfaces;

namespace poc_sdd_net_core_api.Application.Clients.Queries.GetAllClients;

public class GetAllClientsQueryHandler : IRequestHandler<GetAllClientsQuery, IReadOnlyList<ClientResponse>>
{
    private readonly IClientRepository _repository;
    private readonly IMapper _mapper;

    public GetAllClientsQueryHandler(IClientRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<ClientResponse>> Handle(GetAllClientsQuery request, CancellationToken cancellationToken)
    {
        var clients = await _repository.GetAllAsync();
        return _mapper.Map<IReadOnlyList<ClientResponse>>(clients);
    }
}
