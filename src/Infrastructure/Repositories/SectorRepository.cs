using Microsoft.EntityFrameworkCore;
using poc_sdd_net_core_api.Domain.Entities;
using poc_sdd_net_core_api.Domain.Interfaces;
using poc_sdd_net_core_api.Infrastructure.Data;

namespace poc_sdd_net_core_api.Infrastructure.Repositories;

public class SectorRepository : ISectorRepository
{
    private readonly ApplicationDbContext _context;

    public SectorRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Sector>> GetAllAsync()
    {
        return await _context.Sectors.OrderBy(s => s.Name).ToListAsync();
    }

    public async Task<Sector?> GetByIdAsync(Guid id)
    {
        return await _context.Sectors.FindAsync(id);
    }
}
