using MediatR;
using poc_sdd_net_core_api.Application.DTOs;

namespace poc_sdd_net_core_api.Application.Projects.Commands.UpdateProject;

public record UpdateProjectCommand(
    Guid Id,
    Guid ClientId,
    string Title,
    string Description,
    string Technologies,
    DateTime StartDate,
    int? DurationMonths) : IRequest<ProjectResponse?>;
