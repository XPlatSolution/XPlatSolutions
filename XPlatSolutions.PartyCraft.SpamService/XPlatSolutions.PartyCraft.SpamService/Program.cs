using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using XPlatSolutions.PartyCraft.EventBus;
using XPlatSolutions.PartyCraft.EventBus.Interfaces;
using XPlatSolutions.PartyCraft.EventBus.RMQ;
using XPlatSolutions.PartyCraft.EventBus.RMQ.Interfaces;
using XPlatSolutions.PartyCraft.SpamService;
using XPlatSolutions.PartyCraft.SpamService.DAL.Handlers;
using XPlatSolutions.PartyCraft.SpamService.DAL.Interfaces.Mail;
using XPlatSolutions.PartyCraft.SpamService.DAL.Mail;
using XPlatSolutions.PartyCraft.SpamService.Domain.Core.Classes;
using XPlatSolutions.PartyCraft.SpamService.Domain.Core.Enums;
using XPlatSolutions.PartyCraft.SpamService.Domain.Core.Interfaces;
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

builder.Services.AddSingleton<IServiceInfoResolver, ServiceInfoResolver>();

builder.Services.AddSingleton<IEventBusResolver<EventBusTypes>>(x =>
{
    var eventBusResolver = new EventBusResolver<EventBusTypes>();
    return eventBusResolver;
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

var eventBusResolver = app.Services.GetRequiredService<IEventBusResolver<EventBusTypes>>();

var options = app.Services.GetRequiredService<IOptions<AppOptions>>().Value;
var factorySpam = new ConnectionFactory
{
    HostName = options.HostName,
    UserName = options.UserName,
    Password = options.PasswordRmq,
    DispatchConsumersAsync = true
};

var factoryAnalytics = new ConnectionFactory
{
    HostName = options.AnalyticsHostName,
    UserName = options.AnalyticsUserName,
    Password = options.AnalyticsPasswordRmq
};

var loggerForFactory = app.Services.GetRequiredService<ILogger<DefaultRabbitMqPersistentConnection>>();

var persistentConnectionSpam = new DefaultRabbitMqPersistentConnection(factorySpam, loggerForFactory);
var persistentConnectionAnalytics = new DefaultRabbitMqPersistentConnection(factoryAnalytics, loggerForFactory);

var logger = app.Services.GetRequiredService<ILogger<EventBusRmq>>();
var manager = app.Services.GetRequiredService<IEventBusSubscriptionsManager>();
var scope = app.Services.GetRequiredService<IScope>();

var eventBusSpam = new EventBusRmq(persistentConnectionSpam, logger, manager, scope, "mainqueue", 5);
var eventBusAnalytics = new EventBusRmq(persistentConnectionAnalytics, logger, manager, scope, "mainqueue", 5);

eventBusResolver.Register(EventBusTypes.SpamBus, eventBusSpam);
eventBusResolver.Register(EventBusTypes.AnalyticsBus, eventBusAnalytics);

eventBusResolver.Resolve(EventBusTypes.SpamBus)?.Subscribe<MessageEvent, SendMessageIntegrationEventHandler>();


app.Run();
