using AutoMapper;
using MediatR;
using poc_sdd_net_core_api.Application.DTOs;
using poc_sdd_net_core_api.Domain.Interfaces;

namespace poc_sdd_net_core_api.Application.Sectors.Queries.GetAllSectors;

public class GetAllSectorsQueryHandler : IRequestHandler<GetAllSectorsQuery, IReadOnlyList<SectorResponse>>
{
    private readonly ISectorRepository _repository;
    private readonly IMapper _mapper;

    public GetAllSectorsQueryHandler(ISectorRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<IReadOnlyList<SectorResponse>> Handle(GetAllSectorsQuery request, CancellationToken cancellationToken)
    {
        var sectors = await _repository.GetAllAsync();
        return _mapper.Map<IReadOnlyList<SectorResponse>>(sectors);
    }
}
