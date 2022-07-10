using System.Text;
using RabbitMQ.Client;
using XPlatSolutions.PartyCraft.AuthorizationService.DAL.Interfaces.External;
using XPlatSolutions.PartyCraft.EventBus.Interfaces;
using XPlatSolutions.PartyCraft.EventBus.Interfaces.Events;

namespace XPlatSolutions.PartyCraft.AuthorizationService.DAL.External;

public class QueueWriter : IQueueWriter
{
    private readonly IEventBus _eventBus;

    public QueueWriter(IEventBus eventBus)
    {
        _eventBus = eventBus;
    }

    class MessageEvent : IntegrationEvent
    {
        public string Email { get; set; }
        public string Text { get; set; }
        public string Subject { get; set; }
    }

    public void WriteEmailMessageTask(string email, string message, string subject)
    {
        _eventBus.Publish(new MessageEvent { Email = email, Text = message, Subject = subject});
        //return Task.Run(() =>
        //{
        //    var factory = new ConnectionFactory() { HostName = "rabbitmqspam", UserName = "admin", Password = "XPlatQwerty12" }; //TODO
        //    using var connection = factory.CreateConnection();
        //    using var channel = connection.CreateModel();
        //    channel.QueueDeclare(queue: "mail",
        //        durable: false,
        //        exclusive: false,
        //        autoDelete: false,
        //        arguments: null);
        //        
        //    var body = Encoding.UTF8.GetBytes(message);
        //
        //    channel.BasicPublish(exchange: "",
        //        routingKey: "hello",
        //        basicProperties: null,
        //        body: body);
        //});
    }
}