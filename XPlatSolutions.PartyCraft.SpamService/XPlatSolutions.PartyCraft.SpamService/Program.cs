using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using XPlatSolutions.PartyCraft.EventBus;
using XPlatSolutions.PartyCraft.EventBus.Interfaces;
using XPlatSolutions.PartyCraft.EventBus.RMQ;
using XPlatSolutions.PartyCraft.EventBus.RMQ.Interfaces;
using XPlatSolutions.PartyCraft.SpamService;
using XPlatSolutions.PartyCraft.SpamService.DAL.External;
using XPlatSolutions.PartyCraft.SpamService.DAL.Handlers;
using XPlatSolutions.PartyCraft.SpamService.DAL.Interfaces.External;
using XPlatSolutions.PartyCraft.SpamService.DAL.Interfaces.Mail;
using XPlatSolutions.PartyCraft.SpamService.DAL.Mail;
using XPlatSolutions.PartyCraft.SpamService.Domain.Core.Classes;
using XPlatSolutions.PartyCraft.SpamService.Domain.Core.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<AppOptions>(
    builder.Configuration.GetSection("Options"));

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddSingleton<IConnectionFactory>(x =>
{
    var options = x.GetRequiredService<IOptions<AppOptions>>().Value;
    return new ConnectionFactory
    {
        HostName = options.HostName,
        UserName = options.UserName,
        Password = options.PasswordRmq,
        DispatchConsumersAsync = true
    };
}); //TODO

builder.Services.AddSingleton<ISmtpSenderService, SmtpSenderService>();

builder.Services.AddSingleton<SendMessageIntegrationEventHandler>();

builder.Services.AddSingleton<IScope, HandlerResolver>();
builder.Services.AddSingleton<IRabbitMqPersistentConnection, DefaultRabbitMqPersistentConnection>();
builder.Services.AddSingleton<IEventBusSubscriptionsManager, InMemoryEventBusSubscriptionsManager>();

builder.Services.AddSingleton<IQueueReader, QueueReader>();

builder.Services.AddSingleton<IScope, HandlerResolver>();

builder.Services.AddSingleton<IEventBus>(x =>
{
    var persistentConnection = x.GetRequiredService<IRabbitMqPersistentConnection>();
    var logger = x.GetRequiredService<ILogger<EventBusRmq>>();
    var manager = x.GetRequiredService<IEventBusSubscriptionsManager>();
    var scope = x.GetRequiredService<IScope>();
    var eventBus = new EventBusRmq(persistentConnection, logger, manager, scope, "mainqueue", 5);
    return eventBus;
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

var eventBus = app.Services.GetRequiredService<IEventBus>();
eventBus.Subscribe<MessageEvent, SendMessageIntegrationEventHandler>();

app.Run();
