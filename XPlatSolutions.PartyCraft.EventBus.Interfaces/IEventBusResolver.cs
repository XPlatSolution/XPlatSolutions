namespace XPlatSolutions.PartyCraft.EventBus.Interfaces;

public interface IEventBusResolver<in T> where T : Enum
{
    void Register(T type, IEventBus bus);
    IEventBus? Resolve(T type);
}