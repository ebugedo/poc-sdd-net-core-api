using AutoMapper;
using MediatR;
using poc_sdd_net_core_api.Application.DTOs;
using poc_sdd_net_core_api.Domain.Entities;
using poc_sdd_net_core_api.Domain.Interfaces;

namespace poc_sdd_net_core_api.Application.Clients.Commands.CreateClient;

public class CreateClientCommandHandler : IRequestHandler<CreateClientCommand, ClientResponse>
{
    private readonly IClientRepository _repository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreateClientCommandHandler(
        IClientRepository repository,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ClientResponse> Handle(CreateClientCommand request, CancellationToken cancellationToken)
    {
        var client = Client.Create(request.Name, request.Email, request.Phone);

        await _repository.AddAsync(client);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<ClientResponse>(client);
    }
}
