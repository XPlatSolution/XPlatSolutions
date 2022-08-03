using XPlatSolutions.PartyCraft.AnalyticsService.Domain.Core.Models;
using XPlatSolutions.PartyCraft.AnalyticsService.Domain.Core.Responses;

namespace XPlatSolutions.PartyCraft.AnalyticsService.BLL.Interfaces.Services;

public interface IServiceService
{
    Task<ServicesResponse> GetServicesList();
    Task<Service> GetService(string id);
}