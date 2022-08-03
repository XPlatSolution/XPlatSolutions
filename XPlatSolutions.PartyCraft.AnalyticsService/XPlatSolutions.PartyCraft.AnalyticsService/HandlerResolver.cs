using XPlatSolutions.PartyCraft.AnalyticsService.DAL.Handlers;
using XPlatSolutions.PartyCraft.EventBus.RMQ.Interfaces;

namespace XPlatSolutions.PartyCraft.AnalyticsService;

public class HandlerResolver : IScope
{
    private readonly List<object> _handlers = new();
    public HandlerResolver(ExceptionMessageEventIntegrationEventHandler eventHandler)
    {
        _handlers.Add(eventHandler);
    }

    public object? ResolveOptional(Type type)
    {
        return _handlers.FirstOrDefault(x => x.GetType() == type);
    }
}