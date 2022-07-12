using XPlatSolutions.PartyCraft.SpamService.Domain.Core.Interfaces;

namespace XPlatSolutions.PartyCraft.SpamService;

public class ServiceInfoResolver : IServiceInfoResolver
{
    private static readonly string Guid = System.Guid.NewGuid().ToString();
    public string GetServiceName()
    {
        return "XPlatSolutions.PartyCraft.SpamService";
    }

    public string GetServiceGuid()
    {
        return Guid;
    }
}