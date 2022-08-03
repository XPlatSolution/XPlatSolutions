using XPlatSolutions.PartyCraft.AnalyticsService.Domain.Core.Models;
using XPlatSolutions.PartyCraft.AnalyticsService.Domain.Core.Requests;
using XPlatSolutions.PartyCraft.AnalyticsService.Domain.Core.Responses;

namespace XPlatSolutions.PartyCraft.AnalyticsService.BLL.Interfaces.Services;

public interface IExceptionMessageService
{
    Task<ExceptionsResponse> GetExceptionMessagesList(ExceptionsRequest request);
    Task<List<ExceptionMessage>> GetExceptionMessagesList();
    Task<List<ExceptionMessage>> GetExceptionMessagesList(string id);
}