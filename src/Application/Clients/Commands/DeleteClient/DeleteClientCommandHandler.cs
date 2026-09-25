using MediatR;
using poc_sdd_net_core_api.Application.Common;
using poc_sdd_net_core_api.Domain.Interfaces;

namespace poc_sdd_net_core_api.Application.Clients.Commands.DeleteClient;

public class DeleteClientCommandHandler : IRequestHandler<DeleteClientCommand, Unit>
{
    private readonly IClientRepository _repository;
    private readonly IUnitOfWork _unitOfWork;

    public DeleteClientCommandHandler(IClientRepository repository, IUnitOfWork unitOfWork)
    {
        _repository = repository;
        _unitOfWork = unitOfWork;
    }

    public async Task<Unit> Handle(DeleteClientCommand request, CancellationToken cancellationToken)
    {
        var client = await _repository.GetByIdAsync(request.Id);
        if (client is null)
            throw new ClientNotFoundException(request.Id);

        await _repository.DeleteAsync(client);
        await _unitOfWork.SaveChangesAsync();

        return Unit.Value;
    }
}
