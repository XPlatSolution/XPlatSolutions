using XPlatSolutions.PartyCraft.AnalyticsService.DAL.Interfaces.Dao;
using XPlatSolutions.PartyCraft.AnalyticsService.Domain.Core.Classes;
using XPlatSolutions.PartyCraft.AnalyticsService.Domain.Core.Models;
using XPlatSolutions.PartyCraft.EventBus.Interfaces;

namespace XPlatSolutions.PartyCraft.AnalyticsService.DAL.Handlers;

public class ExceptionMessageEventIntegrationEventHandler : IIntegrationEventHandler<ExceptionMessageEvent>
{
    private readonly IExceptionMessageAccess _exceptionMessageAccess;
    private readonly IServiceAccess _serviceAccess;

    public ExceptionMessageEventIntegrationEventHandler(IExceptionMessageAccess exceptionMessageAccess, IServiceAccess serviceAccess)
    {
        _exceptionMessageAccess = exceptionMessageAccess;
        _serviceAccess = serviceAccess;
    }

    public async Task Handle(ExceptionMessageEvent ev)
    {
        var service = await _serviceAccess.GetService(ev.Guid);

        if (service == null)
        {
            service = new Service
            {
                Guid = ev.Guid,
                Name = ev.Service
            };
            await _serviceAccess.AddService(service);
        }

        var exceptionMessage = new ExceptionMessage
        {
            ServiceId = service.Id,
            DateTime = ev.DateTime,
            IsCritical = ev.IsCritical,
            Stacktrace = ev.Stacktrace,
            Text = ev.Text
        };

        await _exceptionMessageAccess.AddExceptionMessage(exceptionMessage);
    }
}