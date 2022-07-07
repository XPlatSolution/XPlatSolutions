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

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<AppOptions>(
    builder.Configuration.GetSection("Options"));

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<IDatabaseResolver, DatabaseResolver>();

builder.Services.AddSingleton<IQueueWriter, QueueWriter>();


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
