using System.Text;
using RabbitMQ.Client;
using XPlatSolutions.PartyCraft.AuthorizationService.DAL.Interfaces.External;

namespace XPlatSolutions.PartyCraft.AuthorizationService.DAL.External;

public class QueueWriter : IQueueWriter
{
    public Task WriteEmailMessageTask(string email, string message)
    {
        return Task.FromResult(() =>
        {
            var factory = new ConnectionFactory() { HostName = "rabbitmq3", UserName = "admin", Password = "XPlatQwerty12" };
            using var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();
            channel.QueueDeclare(queue: "mail",
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null);
                
            var body = Encoding.UTF8.GetBytes(message);

            channel.BasicPublish(exchange: "",
                routingKey: "hello",
                basicProperties: null,
                body: body);
        });
    }
}