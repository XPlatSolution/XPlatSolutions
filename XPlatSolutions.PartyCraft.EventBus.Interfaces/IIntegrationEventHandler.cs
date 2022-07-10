using XPlatSolutions.PartyCraft.EventBus.Interfaces.Events;

namespace XPlatSolutions.PartyCraft.EventBus.Interfaces;


public interface IIntegrationEventHandler<in TIntegrationEvent> : IIntegrationEventHandler
    where TIntegrationEvent : IntegrationEvent
{
    Task Handle(TIntegrationEvent ev);
}

public interface IIntegrationEventHandler
{
}