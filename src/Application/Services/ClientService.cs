using poc_sdd_net_core_api.Application.DTOs;
using poc_sdd_net_core_api.Domain.Entities;
using poc_sdd_net_core_api.Domain.Interfaces;

namespace poc_sdd_net_core_api.Application.Services;

public class ClientService
{
    private readonly IClientRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public ClientService(IClientRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<ClientResponse>> GetAllAsync()
    {
        var clients = await _repository.GetAllAsync();
        return clients.Select(MapToResponse);
    }

    public async Task<ClientResponse?> GetByIdAsync(Guid id)
    {
        var client = await _repository.GetByIdAsync(id);
        return client is null ? null : MapToResponse(client);
    }

    public async Task<ClientResponse> CreateAsync(CreateClientRequest request)
    {
        var client = Client.Create(request.Name, request.Email, request.Phone);
        await _repository.AddAsync(client);
        await _unitOfWork.SaveChangesAsync();
        return MapToResponse(client);
    }

    public async Task<ClientResponse?> UpdateAsync(Guid id, UpdateClientRequest request)
    {
        var client = await _repository.GetByIdAsync(id);
        if (client is null)
            return null;

        client.Update(request.Name, request.Email, request.Phone);
        await _repository.UpdateAsync(client);
        await _unitOfWork.SaveChangesAsync();
        return MapToResponse(client);
    }

    public async Task<bool> DeleteAsync(Guid id)
    {
        var client = await _repository.GetByIdAsync(id);
        if (client is null)
            return false;

        await _repository.DeleteAsync(client);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    private static ClientResponse MapToResponse(Client client)
    {
        return new ClientResponse
        {
            Id = client.Id,
            Name = client.Name,
            Email = client.Email,
            Phone = client.Phone,
            CreatedAt = client.CreatedAt
        };
    }
}
