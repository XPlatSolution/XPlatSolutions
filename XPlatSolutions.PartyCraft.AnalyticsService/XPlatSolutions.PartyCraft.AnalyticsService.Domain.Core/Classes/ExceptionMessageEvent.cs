using XPlatSolutions.PartyCraft.EventBus.Interfaces.Events;

namespace XPlatSolutions.PartyCraft.AnalyticsService.Domain.Core.Classes;

public class ExceptionMessageEvent : IntegrationEvent
{
    public string Text { get; set; }
    public string Stacktrace { get; set; }
    public string Service { get; set; }
    public string Guid { get; set; }
    public bool IsCritical { get; set; }
    public DateTime DateTime { get; set; }
}