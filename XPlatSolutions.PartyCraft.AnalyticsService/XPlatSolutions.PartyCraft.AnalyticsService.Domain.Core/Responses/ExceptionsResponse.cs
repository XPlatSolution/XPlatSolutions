using XPlatSolutions.PartyCraft.AnalyticsService.Domain.Core.Models;

namespace XPlatSolutions.PartyCraft.AnalyticsService.Domain.Core.Responses;

public class ExceptionsResponse
{
    public List<ExceptionMessage> Exceptions { get; set; }
}