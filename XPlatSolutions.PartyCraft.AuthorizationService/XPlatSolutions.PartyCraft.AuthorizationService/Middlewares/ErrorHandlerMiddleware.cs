using XPlatSolutions.PartyCraft.AuthorizationService.Domain.Core.Exceptions;
using System.Net;
using System.Text.Json;
using XPlatSolutions.PartyCraft.AuthorizationService.Domain.Core.Classes;
using XPlatSolutions.PartyCraft.AuthorizationService.Domain.Core.Enums;
using XPlatSolutions.PartyCraft.AuthorizationService.Domain.Core.Interfaces;
using XPlatSolutions.PartyCraft.EventBus.Interfaces;

namespace XPlatSolutions.PartyCraft.AuthorizationService.Middlewares
{
    public class ErrorHandlerMiddleware
    {
        private readonly RequestDelegate _next;

        public ErrorHandlerMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context, IEventBusResolver<EventBusTypes> eventBusResolver, IServiceInfoResolver serviceInfoResolver)
        {
            try
            {
                await _next(context);
            }
            catch (Exception error)
            {
                var response = context.Response;
                response.ContentType = "application/json";
                string? message;

                switch (error)
                {
                    case AuthenticateException e:
                        // custom application error

                        PublishExceptionMessage(error, eventBusResolver, serviceInfoResolver, false);
                        response.StatusCode = (int)HttpStatusCode.BadRequest;
                        message = e?.Message;
                        break;
                    case XPlatSolutionsException e:
                        // custom application error

                        PublishExceptionMessage(error, eventBusResolver, serviceInfoResolver, false);
                        response.StatusCode = (int)HttpStatusCode.BadRequest;
                        message = e?.Message;
                        break;
                    default:
                        // unhandled error

                        PublishExceptionMessage(error, eventBusResolver, serviceInfoResolver, true);
                        response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        message = "Something went wrong";
                        break;
                }

                var result = JsonSerializer.Serialize(new { message });
                await response.WriteAsync(result);
            }
        }

        private static void PublishExceptionMessage(Exception error, IEventBusResolver<EventBusTypes> eventBusResolver, IServiceInfoResolver serviceInfoResolver, bool isCritical)
        {
            var msg = new ExceptionMessageEvent
            {
                DateTime = DateTime.UtcNow,
                Guid = serviceInfoResolver.GetServiceGuid(),
                Service = serviceInfoResolver.GetServiceName(),
                IsCritical = isCritical,
                Stacktrace = error.StackTrace ?? string.Empty,
                Text = error.Message
            };
            eventBusResolver.Resolve(EventBusTypes.AnalyticsBus)?.Publish(msg);
        }
    }
}
