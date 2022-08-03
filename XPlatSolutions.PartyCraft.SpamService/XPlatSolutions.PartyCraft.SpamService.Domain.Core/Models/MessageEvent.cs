using XPlatSolutions.PartyCraft.EventBus.Interfaces.Events;

namespace XPlatSolutions.PartyCraft.SpamService.Domain.Core.Models;

public class MessageEvent : IntegrationEvent
{
    public string Email { get; set; }
    public string Text { get; set; }
    public string Subject { get; set; }
}