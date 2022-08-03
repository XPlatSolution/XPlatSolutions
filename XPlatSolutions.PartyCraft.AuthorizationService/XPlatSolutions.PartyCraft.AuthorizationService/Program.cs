using Microsoft.Extensions.Options;
using RabbitMQ.Client;
using XPlatSolutions.PartyCraft.AuthorizationService;
using XPlatSolutions.PartyCraft.AuthorizationService.BLL.Interfaces.Services;
using XPlatSolutions.PartyCraft.AuthorizationService.BLL.Interfaces.Utils;
using XPlatSolutions.PartyCraft.AuthorizationService.BLL.Services;
using XPlatSolutions.PartyCraft.AuthorizationService.BLL.Utils;
using XPlatSolutions.PartyCraft.AuthorizationService.BLL.Validators;
using XPlatSolutions.PartyCraft.AuthorizationService.DAL.Dao;
using XPlatSolutions.PartyCraft.AuthorizationService.DAL.Decorators;
using XPlatSolutions.PartyCraft.AuthorizationService.DAL.External;
using XPlatSolutions.PartyCraft.AuthorizationService.DAL.Interfaces.Dao;
using XPlatSolutions.PartyCraft.AuthorizationService.DAL.Interfaces.External;
using XPlatSolutions.PartyCraft.AuthorizationService.Domain.Core.Classes;
using XPlatSolutions.PartyCraft.AuthorizationService.Domain.Core.Enums;
using XPlatSolutions.PartyCraft.AuthorizationService.Domain.Core.Interfaces;
using XPlatSolutions.PartyCraft.AuthorizationService.Filters;
using XPlatSolutions.PartyCraft.AuthorizationService.Middlewares;
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

builder.Services.AddStackExchangeRedisCache(options =>
{
    var appOptions = builder.Configuration.GetSection("Options").Get<AppOptions>();
    options.Configuration = $"{appOptions.RedisHost}:{appOptions.RedisPort},password={appOptions.RedisPassword}";
}); 

builder.Services.AddSingleton(typeof(ResultFilter<>), typeof(ResultFilter<>));
builder.Services.AddSingleton<ResultFilterBase>();
builder.Services.AddSingleton<IResponseFactory, ResponseFactory>();
builder.Services.AddSingleton<IOperationResultFactory, OperationResultFactory>();

builder.Services.AddSingleton<IServiceInfoResolver, ServiceInfoResolver>();
builder.Services.AddSingleton<IDatabaseResolver, DatabaseResolver>();

builder.Services.AddSingleton<IQueueWriter, QueueWriter>();

builder.Services.AddSingleton<IScope, HandlerResolver>();
builder.Services.AddSingleton<IEventBusSubscriptionsManager, InMemoryEventBusSubscriptionsManager>();

builder.Services.AddSingleton<IScope, HandlerResolver>();

builder.Services.AddSingleton<IPasswordChangeRequestAccess, PasswordChangeRequestAccess>();
builder.Services.AddSingleton<IActivationCodeAccess, ActivationCodeAccess>();
builder.Services.AddSingleton<ITokensAccess, TokensAccess>();
builder.Services.AddSingleton<IUsersAccess, UsersAccess>();
builder.Services.Decorate<IUsersAccess, UserAccessDecorator>();

builder.Services.AddSingleton<ITokenUtils, TokenUtils>();
builder.Services.AddSingleton<IUserService, UserService>();
builder.Services.Decorate<IUserService, UserServiceValidator>();

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

app.UseMiddleware<ErrorHandlerMiddleware>();
app.UseMiddleware<TokenMiddleware>();

app.MapControllers();

var eventBusResolver = app.Services.GetRequiredService<IEventBusResolver<EventBusTypes>>();

var options = app.Services.GetRequiredService<IOptions<AppOptions>>().Value;
var factorySpam = new ConnectionFactory
{
    HostName = options.HostName,
    UserName = options.UserName,
    Password = options.PasswordRmq
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

app.Run();
