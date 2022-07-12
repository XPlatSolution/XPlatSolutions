using XPlatSolutions.PartyCraft.EventBus.Interfaces;

namespace XPlatSolutions.PartyCraft.EventBus;

public class EventBusResolver<T> : IEventBusResolver<T> where T : Enum
{
    private static readonly Dictionary<T, IEventBus> EventBuses = new();

    public void Register(T type, IEventBus bus)
    {
        if (!EventBuses.ContainsKey(type))
            EventBuses.Add(type, bus);
    }

    public IEventBus? Resolve(T type)
    {
        return EventBuses.ContainsKey(type) ? EventBuses[type] : null;
    }
}