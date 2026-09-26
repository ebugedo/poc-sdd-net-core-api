using AutoMapper;
using MediatR;
using poc_sdd_net_core_api.Application.Common;
using poc_sdd_net_core_api.Application.DTOs;
using poc_sdd_net_core_api.Domain.Interfaces;

namespace poc_sdd_net_core_api.Application.Sectors.Queries.GetSectorById;

public class GetSectorByIdQueryHandler : IRequestHandler<GetSectorByIdQuery, SectorResponse>
{
    private readonly ISectorRepository _repository;
    private readonly IMapper _mapper;

    public GetSectorByIdQueryHandler(ISectorRepository repository, IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<SectorResponse> Handle(GetSectorByIdQuery request, CancellationToken cancellationToken)
    {
        var sector = await _repository.GetByIdAsync(request.Id);

        if (sector is null)
            throw new SectorNotFoundException(request.Id);

        return _mapper.Map<SectorResponse>(sector);
    }
}
