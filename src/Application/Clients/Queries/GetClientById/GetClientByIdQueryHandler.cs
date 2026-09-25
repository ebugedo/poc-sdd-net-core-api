using AutoMapper;
using MediatR;
using poc_sdd_net_core_api.Application.Common;
using poc_sdd_net_core_api.Application.DTOs;
using poc_sdd_net_core_api.Domain.Interfaces;

namespace poc_sdd_net_core_api.Application.Clients.Queries.GetClientById;

public class GetClientByIdQueryHandler : IRequestHandler<GetClientByIdQuery, ClientResponse>
{
    private readonly IClientRepository _repository;
    private readonly IMapper _mapper;

    public GetClientByIdQueryHandler(IClientRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<ClientResponse> Handle(GetClientByIdQuery request, CancellationToken cancellationToken)
    {
        var client = await _repository.GetByIdAsync(request.Id);
        if (client is null)
            throw new ClientNotFoundException(request.Id);

        return _mapper.Map<ClientResponse>(client);
    }
}
