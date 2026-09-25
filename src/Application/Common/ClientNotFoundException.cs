namespace poc_sdd_net_core_api.Application.Common;

public class ClientNotFoundException : Exception
{
    public ClientNotFoundException(Guid id)
        : base($"Client with id {id} not found")
    {
    }
}
