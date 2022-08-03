using XPlatSolutions.PartyCraft.AnalyticsService.Domain.Core.Models;

namespace XPlatSolutions.PartyCraft.AnalyticsService.DAL.Interfaces.Dao;

public interface IServiceAccess
{
    Task<List<Service>> GetServices();
    Task<Service> GetService(string guid);
    Task AddService(Service message);
    Task RemoveService(string serviceId);
}