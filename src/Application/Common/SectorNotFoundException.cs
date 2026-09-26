namespace poc_sdd_net_core_api.Application.Common;

public class SectorNotFoundException : Exception
{
    public SectorNotFoundException(Guid id)
        : base($"Sector with id {id} was not found")
    {
    }
}
