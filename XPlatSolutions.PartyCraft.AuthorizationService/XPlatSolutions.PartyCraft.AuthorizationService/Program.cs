using RabbitMQ.Client;
using XPlatSolutions.PartyCraft.AuthorizationService;
using XPlatSolutions.PartyCraft.AuthorizationService.BLL.Interfaces.Services;
using XPlatSolutions.PartyCraft.AuthorizationService.BLL.Interfaces.Utils;
using XPlatSolutions.PartyCraft.AuthorizationService.BLL.Services;
using XPlatSolutions.PartyCraft.AuthorizationService.BLL.Utils;
using XPlatSolutions.PartyCraft.AuthorizationService.DAL.Dao;
using XPlatSolutions.PartyCraft.AuthorizationService.DAL.External;
using XPlatSolutions.PartyCraft.AuthorizationService.DAL.Interfaces.Dao;
using XPlatSolutions.PartyCraft.AuthorizationService.DAL.Interfaces.External;
using XPlatSolutions.PartyCraft.AuthorizationService.Domain.Core.Classes;
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

builder.Services.AddSingleton<IDatabaseResolver, DatabaseResolver>();

builder.Services.AddSingleton<IQueueWriter, QueueWriter>();

builder.Services.AddSingleton<IConnectionFactory>(x=> new ConnectionFactory { HostName = "rabbitmqspam", UserName = "admin", Password = "XPlatQwerty12" }); //TODO

builder.Services.AddSingleton<IScope, HandlerResolver>();
builder.Services.AddSingleton<IRabbitMqPersistentConnection, DefaultRabbitMqPersistentConnection>();
builder.Services.AddSingleton<IEventBusSubscriptionsManager, InMemoryEventBusSubscriptionsManager>();

builder.Services.AddSingleton<IScope, HandlerResolver>();

builder.Services.AddSingleton<IEventBus, EventBusRmq>();

builder.Services.AddSingleton<IPasswordChangeRequestAccess, PasswordChangeRequestAccess>();
builder.Services.AddSingleton<IActivationCodeAccess, ActivationCodeAccess>();
builder.Services.AddSingleton<ITokensAccess, TokensAccess>();
builder.Services.AddSingleton<IUsersAccess, UsersAccess>();

builder.Services.AddSingleton<ITokenUtils, TokenUtils>();
builder.Services.AddSingleton<IUserService, UserService>();

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

app.Run();
