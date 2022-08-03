namespace XPlatSolutions.PartyCraft.AnalyticsService.Domain.Core.Requests;

public class ExceptionsRequest
{
    public DateTime? From { get; set; }
    public DateTime? To { get; set; }
    public string? ServiceId { get; set; }
}