using XPlatSolutions.PartyCraft.EventBus.RMQ.Interfaces;
using XPlatSolutions.PartyCraft.SpamService.DAL.Handlers;

namespace XPlatSolutions.PartyCraft.SpamService;

public class HandlerResolver : IScope
{
    private List<object> _handlers = new();
    public HandlerResolver(SendMessageIntegrationEventHandler messageHandler)
    {
        _handlers.Add(messageHandler);
    }

    public object? ResolveOptional(Type type)
    {
        return _handlers.FirstOrDefault(x => x.GetType() == type);
    }
}