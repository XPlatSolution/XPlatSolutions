using XPlatSolutions.PartyCraft.EventBus.RMQ.Interfaces;

namespace XPlatSolutions.PartyCraft.AuthorizationService;

public class HandlerResolver : IScope
{
    private List<object> _handlers = new();
    public HandlerResolver()
    {
        
    }

    public object? ResolveOptional(Type type)
    {
        return _handlers.FirstOrDefault(x => x.GetType() == type);
    }
}