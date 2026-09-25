using MediatR;
using poc_sdd_net_core_api.Application.DTOs;

namespace poc_sdd_net_core_api.Application.Clients.Queries.GetClientById;

public record GetClientByIdQuery(Guid Id) : IRequest<ClientResponse?>;
