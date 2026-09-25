using MediatR;

namespace poc_sdd_net_core_api.Application.Projects.Commands.DeleteProject;

public record DeleteProjectCommand(Guid Id) : IRequest<Unit>;
