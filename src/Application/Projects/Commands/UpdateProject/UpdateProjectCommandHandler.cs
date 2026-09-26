using AutoMapper;
using MediatR;
using poc_sdd_net_core_api.Application.DTOs;
using poc_sdd_net_core_api.Domain.Interfaces;

namespace poc_sdd_net_core_api.Application.Projects.Commands.UpdateProject;

public class UpdateProjectCommandHandler : IRequestHandler<UpdateProjectCommand, ProjectResponse?>
{
    private readonly IProjectRepository _repository;
    private readonly IClientRepository _clientRepository;
    private readonly ISectorRepository _sectorRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IMapper _mapper;

    public UpdateProjectCommandHandler(
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

    public async Task<ProjectResponse?> Handle(UpdateProjectCommand request, CancellationToken cancellationToken)
    {
        var project = await _repository.GetByIdAsync(request.Id);

        if (project is null)
            return null;

        project.Update(
            request.ClientId,
            request.SectorId,
            request.Title,
            request.Description,
            request.Technologies,
            request.StartDate,
            request.DurationMonths);

        var client = await _clientRepository.GetByIdAsync(request.ClientId);

        if (client is null)
            throw new Common.ClientNotFoundException(request.ClientId);

        var sector = await _sectorRepository.GetByIdAsync(request.SectorId);

        if (sector is null)
            throw new Common.SectorNotFoundException(request.SectorId);

        await _repository.UpdateAsync(project);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<ProjectResponse>(project);
    }
}
