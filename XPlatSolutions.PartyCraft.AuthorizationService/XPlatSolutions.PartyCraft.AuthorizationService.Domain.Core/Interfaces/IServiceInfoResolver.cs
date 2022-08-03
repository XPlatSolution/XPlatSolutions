namespace XPlatSolutions.PartyCraft.AuthorizationService.Domain.Core.Interfaces;

public interface IServiceInfoResolver
{
    string GetServiceName();
    string GetServiceGuid();
}