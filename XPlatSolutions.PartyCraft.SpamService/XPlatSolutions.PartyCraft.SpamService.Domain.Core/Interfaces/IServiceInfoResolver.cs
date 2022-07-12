namespace XPlatSolutions.PartyCraft.SpamService.Domain.Core.Interfaces;

public interface IServiceInfoResolver
{
    string GetServiceName();
    string GetServiceGuid();
}