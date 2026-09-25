namespace poc_sdd_net_core_api.Application.Common;

public class ProjectNotFoundException : Exception
{
    public ProjectNotFoundException(Guid id)
        : base($"Project with id {id} was not found")
    {
    }
}
