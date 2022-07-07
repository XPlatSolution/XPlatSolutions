using XPlatSolutions.PartyCraft.AuthorizationService.Domain.Core.Exceptions;
using System.Net;
using System.Text.Json;

namespace XPlatSolutions.PartyCraft.AuthorizationService.Middlewares
{
    public class ErrorHandlerMiddleware
    {
        private readonly RequestDelegate _next;

        public ErrorHandlerMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
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
                        response.StatusCode = (int)HttpStatusCode.BadRequest;
                        message = e?.Message;
                        break;
                    case XPlatSolutionsException e:
                        // custom application error
                        response.StatusCode = (int)HttpStatusCode.BadRequest;
                        message = e?.Message;
                        break;
                    default:
                        // unhandled error
                        response.StatusCode = (int)HttpStatusCode.InternalServerError;
                        message = "Something went wrong";
                        break;
                }

                var result = JsonSerializer.Serialize(new { message });
                await response.WriteAsync(result);
            }
        }
    }
}
