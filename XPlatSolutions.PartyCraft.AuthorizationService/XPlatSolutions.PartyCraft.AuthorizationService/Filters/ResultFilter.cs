using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using XPlatSolutions.PartyCraft.AuthorizationService.Domain.Core.Classes;
using XPlatSolutions.PartyCraft.AuthorizationService.Domain.Core.Enums;
using XPlatSolutions.PartyCraft.AuthorizationService.Domain.Core.Interfaces;
using XPlatSolutions.PartyCraft.AuthorizationService.Domain.Core.Models;
using XPlatSolutions.PartyCraft.EventBus.Interfaces;

namespace XPlatSolutions.PartyCraft.AuthorizationService.Filters;

public class ResultFilterBase : IResultFilter
{
    public void OnResultExecuting(ResultExecutingContext context)
    {
        if (context.Result is not ObjectResult { Value: OperationResult result }) return;

        if (result.Status != StatusCode.Success)
        {
            context.Result = new JsonResult(new { message = result.Message }) { StatusCode = 400 };
            return;
        }

        var data = GetAdditionalResult(result);
        if (data != null)
        {
            context.Result = data;
            return;
        }

        context.Result = new JsonResult(new { success = true });
    }

    public virtual IActionResult? GetAdditionalResult(OperationResult result)
    {
        return null;
    }

    public void OnResultExecuted(ResultExecutedContext context)
    {
    }
}

public class ResultFilter<T> : ResultFilterBase
{
    private readonly IEventBusResolver<EventBusTypes> _eventBusResolver;
    private readonly IServiceInfoResolver _serviceInfoResolver;

    public ResultFilter(IEventBusResolver<EventBusTypes> eventBusResolver, IServiceInfoResolver serviceInfoResolver)
    {
        _eventBusResolver = eventBusResolver;
        _serviceInfoResolver = serviceInfoResolver;
    }
    

    public override IActionResult? GetAdditionalResult(OperationResult result)
    {
        if (result is OperationResult<T> genericResult)
        {
            return genericResult.Result != null
                ? new JsonResult(genericResult.Result)
                : new JsonResult(new { success = true });
            ;
        }
        return null;
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