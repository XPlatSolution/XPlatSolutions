using XPlatSolutions.PartyCraft.EventService.BLL.Interfaces.Services;
using XPlatSolutions.PartyCraft.EventService.BLL.Services;
using XPlatSolutions.PartyCraft.EventService.DAL.Dao;
using XPlatSolutions.PartyCraft.EventService.DAL.Interfaces.Dao;
using XPlatSolutions.PartyCraft.EventService.Domain.Core.Classes;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<AppOptions>(
    builder.Configuration.GetSection("Options"));

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<IDatabaseResolver, DatabaseResolver>();
builder.Services.AddSingleton<IEventsAccess, EventsAccess>();
builder.Services.AddSingleton<IEventService, EventService>();

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

app.Run();
