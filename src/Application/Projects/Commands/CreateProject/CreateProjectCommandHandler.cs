using AutoMapper;
using MediatR;
using poc_sdd_net_core_api.Application.Common;
using poc_sdd_net_core_api.Application.DTOs;
using poc_sdd_net_core_api.Domain.Entities;
using poc_sdd_net_core_api.Domain.Interfaces;

namespace poc_sdd_net_core_api.Application.Projects.Commands.CreateProject;

public class CreateProjectCommandHandler : IRequestHandler<CreateProjectCommand, ProjectResponse>
{
    private readonly IProjectRepository _repository;
    private readonly IClientRepository _clientRepository;
    private readonly ISectorRepository _sectorRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public CreateProjectCommandHandler(
        IProjectRepository repository,
        IClientRepository clientRepository,
        ISectorRepository sectorRepository,
        IUnitOfWork unitOfWork,
        IMapper mapper)
    {
        _repository = repository;
        _clientRepository = clientRepository;
        _sectorRepository = sectorRepository;
        _unitOfWork = unitOfWork;
        _mapper = mapper;
    }

    public async Task<ProjectResponse> Handle(CreateProjectCommand request, CancellationToken cancellationToken)
    {
        var project = Project.Create(
            request.ClientId,
            request.SectorId,
            request.Title,
            request.Description,
            request.Technologies,
            request.StartDate,
            request.DurationMonths);

        var client = await _clientRepository.GetByIdAsync(request.ClientId);

        if (client is null)
            throw new ClientNotFoundException(request.ClientId);

        var sector = await _sectorRepository.GetByIdAsync(request.SectorId);

        if (sector is null)
            throw new SectorNotFoundException(request.SectorId);

        await _repository.AddAsync(project);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<ProjectResponse>(project);
    }
}
