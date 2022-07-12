using XPlatSolutions.PartyCraft.AnalyticsService.BLL.Interfaces.Services;
using XPlatSolutions.PartyCraft.AnalyticsService.DAL.Interfaces.Dao;
using XPlatSolutions.PartyCraft.AnalyticsService.Domain.Core.Models;
using XPlatSolutions.PartyCraft.AnalyticsService.Domain.Core.Responses;

namespace XPlatSolutions.PartyCraft.AnalyticsService.BLL.Services;

public class ServiceService : IServiceService
{
    private readonly IServiceAccess _serviceAccess;

    public ServiceService(IServiceAccess serviceAccess)
    {
        _serviceAccess = serviceAccess;
    }

    public async Task<ServicesResponse> GetServicesList()
    {
        return new ServicesResponse { Services = await _serviceAccess.GetServices() };
    }

    public async Task<Service> GetService(string id)
    {
        return await _serviceAccess.GetService(id);
    }
}