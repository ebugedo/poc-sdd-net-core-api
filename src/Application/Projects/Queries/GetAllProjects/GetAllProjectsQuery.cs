using MediatR;
using poc_sdd_net_core_api.Application.DTOs;

namespace poc_sdd_net_core_api.Application.Projects.Queries.GetAllProjects;

public record GetAllProjectsQuery(Guid? ClientId = null) : IRequest<IReadOnlyList<ProjectResponse>>;
