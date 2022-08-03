using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using XPlatSolutions.PartyCraft.AnalyticsService;
using XPlatSolutions.PartyCraft.AnalyticsService.BLL.Interfaces.Services;
using XPlatSolutions.PartyCraft.AnalyticsService.BLL.Services;
using XPlatSolutions.PartyCraft.AnalyticsService.DAL.Dao;
using XPlatSolutions.PartyCraft.AnalyticsService.DAL.Handlers;
using XPlatSolutions.PartyCraft.AnalyticsService.DAL.Interfaces.Dao;
using XPlatSolutions.PartyCraft.AnalyticsService.Domain.Core.Classes;
using XPlatSolutions.PartyCraft.EventBus;
using XPlatSolutions.PartyCraft.EventBus.Interfaces;
using XPlatSolutions.PartyCraft.EventBus.RMQ;
using XPlatSolutions.PartyCraft.EventBus.RMQ.Interfaces;

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

builder.Services.AddSingleton<IDatabaseResolver, DatabaseResolver>();

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

builder.Services.AddSingleton<ExceptionMessageEventIntegrationEventHandler>();

builder.Services.AddSingleton<IScope, HandlerResolver>();
builder.Services.AddSingleton<IRabbitMqPersistentConnection, DefaultRabbitMqPersistentConnection>();
builder.Services.AddSingleton<IEventBusSubscriptionsManager, InMemoryEventBusSubscriptionsManager>();

builder.Services.AddSingleton<IEventBus, EventBusRmq>();

builder.Services.AddSingleton<IExceptionMessageAccess, ExceptionMessageAccess>();
builder.Services.AddSingleton<IServiceAccess, ServiceAccess>();

builder.Services.AddSingleton<IServiceService, ServiceService>();
builder.Services.AddSingleton<IExceptionMessageService, ExceptionMessageService>();

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
eventBus.Subscribe<ExceptionMessageEvent, ExceptionMessageEventIntegrationEventHandler>();

app.Run();
