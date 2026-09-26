using poc_sdd_net_core_api.Domain.Entities;

namespace poc_sdd_net_core_api.Domain.Interfaces;

/// <summary>
/// Repositorio de solo lectura: el catálogo de sectores no se modifica en runtime (ADR-005).
/// </summary>
public interface ISectorRepository
{
    Task<IEnumerable<Sector>> GetAllAsync();
    Task<Sector?> GetByIdAsync(Guid id);
}
