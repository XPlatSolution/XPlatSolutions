using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using XPlatSolutions.PartyCraft.AuthorizationService.Domain.Core.Classes;
using XPlatSolutions.PartyCraft.AuthorizationService.Domain.Core.Enums;
using XPlatSolutions.PartyCraft.AuthorizationService.Domain.Core.Interfaces;
using XPlatSolutions.PartyCraft.AuthorizationService.Domain.Core.Models;
using XPlatSolutions.PartyCraft.EventBus.Interfaces;

namespace XPlatSolutions.PartyCraft.AuthorizationService.Filters;

public class ResultFilter : IResultFilter
{

    private readonly IEventBusResolver<EventBusTypes> _eventBusResolver;
    private readonly IServiceInfoResolver _serviceInfoResolver;
    private readonly ILogger<ResultFilter> _logger;

    public ResultFilter(IEventBusResolver<EventBusTypes> eventBusResolver, IServiceInfoResolver serviceInfoResolver, ILogger<ResultFilter> logger)
    {
        _eventBusResolver = eventBusResolver;
        _serviceInfoResolver = serviceInfoResolver;
        _logger = logger;
    }

    public void OnResultExecuting(ResultExecutingContext context)
    {
        if (context.Result is not ObjectResult { Value: OperationResult result }) return;

        if (result.Status != StatusCode.Success)
        {
            context.Result = new JsonResult(new { message = result.Message }) { StatusCode = 400 };
        }
    }

    public void OnResultExecuted(ResultExecutedContext context)
    {
    }
    
    private static void PublishExceptionMessage(string error, IEventBusResolver<EventBusTypes> eventBusResolver, IServiceInfoResolver serviceInfoResolver, bool isCritical)
    {
        var msg = new ExceptionMessageEvent
        {
            DateTime = DateTime.UtcNow,
            Guid = serviceInfoResolver.GetServiceGuid(),
            Service = serviceInfoResolver.GetServiceName(),
            IsCritical = isCritical,
            Stacktrace = null,
            Text = error
        };
        eventBusResolver.Resolve(EventBusTypes.AnalyticsBus)?.Publish(msg);
    }
}