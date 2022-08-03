using RabbitMQ.Client;

namespace XPlatSolutions.PartyCraft.EventBus.RMQ.Interfaces;

public interface IRabbitMqPersistentConnection : IDisposable
{
    bool IsConnected { get; }

    bool TryConnect();

    IModel CreateModel();
}