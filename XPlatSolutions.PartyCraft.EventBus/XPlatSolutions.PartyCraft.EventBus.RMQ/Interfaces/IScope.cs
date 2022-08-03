namespace XPlatSolutions.PartyCraft.EventBus.RMQ.Interfaces;

public interface IScope
{
    object? ResolveOptional(Type type);
}