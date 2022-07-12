using XPlatSolutions.PartyCraft.AuthorizationService.Domain.Core.Interfaces;

namespace XPlatSolutions.PartyCraft.AuthorizationService;

public class ServiceInfoResolver : IServiceInfoResolver
{
    private static readonly string Guid = System.Guid.NewGuid().ToString();
    public string GetServiceName()
    {
        return "XPlatSolutions.PartyCraft.AuthorizationService";
    }

    public string GetServiceGuid()
    {
        return Guid;
    }
}