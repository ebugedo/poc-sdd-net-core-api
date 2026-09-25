using Microsoft.EntityFrameworkCore;
using poc_sdd_net_core_api.Domain.Entities;
using poc_sdd_net_core_api.Domain.Interfaces;
using poc_sdd_net_core_api.Infrastructure.Data;

namespace poc_sdd_net_core_api.Infrastructure.Repositories;

public class ProjectRepository : IProjectRepository
{
    private readonly ApplicationDbContext _context;

    public ProjectRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Project>> GetAllAsync(Guid? clientId = null)
    {
        var query = _context.Projects.AsQueryable();

        if (clientId.HasValue)
            query = query.Where(p => p.ClientId == clientId.Value);

        return await query.OrderBy(p => p.StartDate).ToListAsync();
    }

    public async Task<Project?> GetByIdAsync(Guid id)
    {
        return await _context.Projects.FindAsync(id);
    }

    public async Task<Project> AddAsync(Project project)
    {
        await _context.Projects.AddAsync(project);
        return project;
    }

    public async Task UpdateAsync(Project project)
    {
        _context.Projects.Update(project);
        await Task.CompletedTask;
    }

    public async Task DeleteAsync(Project project)
    {
        _context.Projects.Remove(project);
        await Task.CompletedTask;
    }
}
