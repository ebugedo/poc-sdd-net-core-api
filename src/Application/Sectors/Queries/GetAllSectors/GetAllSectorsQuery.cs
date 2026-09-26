using MediatR;
using poc_sdd_net_core_api.Application.DTOs;

namespace poc_sdd_net_core_api.Application.Sectors.Queries.GetAllSectors;

public record GetAllSectorsQuery : IRequest<IReadOnlyList<SectorResponse>>;
