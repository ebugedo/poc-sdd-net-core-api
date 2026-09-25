using MediatR;
using poc_sdd_net_core_api.Application.DTOs;

namespace poc_sdd_net_core_api.Application.Clients.Commands.UpdateClient;

public record UpdateClientCommand(Guid Id, string Name, string Email, string? Phone, string? Logo) : IRequest<ClientResponse?>;
