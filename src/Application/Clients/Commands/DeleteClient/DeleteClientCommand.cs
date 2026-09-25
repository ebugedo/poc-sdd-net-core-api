using MediatR;

namespace poc_sdd_net_core_api.Application.Clients.Commands.DeleteClient;

public record DeleteClientCommand(Guid Id) : IRequest<Unit>;
