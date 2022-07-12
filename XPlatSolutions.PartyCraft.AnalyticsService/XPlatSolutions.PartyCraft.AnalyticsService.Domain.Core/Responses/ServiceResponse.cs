using XPlatSolutions.PartyCraft.AnalyticsService.Domain.Core.Models;

namespace XPlatSolutions.PartyCraft.AnalyticsService.Domain.Core.Responses;

public class ServiceResponse
{
    public Service Service { get; set; }
    public List<ExceptionMessage> ExceptionMessages { get; set; }
}