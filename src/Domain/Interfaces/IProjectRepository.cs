using poc_sdd_net_core_api.Domain.Entities;

namespace poc_sdd_net_core_api.Domain.Interfaces;

public interface IProjectRepository
{
    Task<IEnumerable<Project>> GetAllAsync(Guid? clientId = null);
    Task<Project?> GetByIdAsync(Guid id);
    Task<Project> AddAsync(Project project);
    Task UpdateAsync(Project project);
    Task DeleteAsync(Project project);
}
