using System.Text;
using RabbitMQ.Client;
using XPlatSolutions.PartyCraft.AuthorizationService.DAL.Interfaces.External;
using XPlatSolutions.PartyCraft.AuthorizationService.Domain.Core.Enums;
using XPlatSolutions.PartyCraft.EventBus.Interfaces;
using XPlatSolutions.PartyCraft.EventBus.Interfaces.Events;

namespace XPlatSolutions.PartyCraft.AuthorizationService.DAL.External;

public class QueueWriter : IQueueWriter
{
    private readonly IEventBusResolver<EventBusTypes> _eventBusResolver;

    public QueueWriter(IEventBusResolver<EventBusTypes> eventBusResolver)
    {
        _eventBusResolver = eventBusResolver;
    }

    class MessageEvent : IntegrationEvent
    {
        public string Email { get; set; }
        public string Text { get; set; }
        public string Subject { get; set; }
    }

    public void WriteEmailMessageTask(string email, string message, string subject)
    {
        _eventBusResolver.Resolve(EventBusTypes.SpamBus)?.Publish(new MessageEvent { Email = email, Text = message, Subject = subject});
    }
}