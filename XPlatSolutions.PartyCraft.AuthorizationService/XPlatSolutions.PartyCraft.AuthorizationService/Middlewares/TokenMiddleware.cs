using XPlatSolutions.PartyCraft.AuthorizationService.BLL.Interfaces.Services;
using XPlatSolutions.PartyCraft.AuthorizationService.BLL.Interfaces.Utils;
using XPlatSolutions.PartyCraft.AuthorizationService.Domain.Core.Classes;
using Microsoft.Extensions.Options;

namespace XPlatSolutions.PartyCraft.AuthorizationService.Middlewares;

public class TokenMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IOptions<AppOptions> _appOptions;

    public TokenMiddleware(RequestDelegate next, IOptions<AppOptions> appOptions)
    {
        _next = next;
        _appOptions = appOptions;
    }

    public async Task Invoke(HttpContext context, IUserService userService, ITokenUtils tokenUtils)
    {
        var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
        var userId = tokenUtils.ValidateToken(token);
        if (userId != null)
        {
            context.Items["User"] = await userService.GetById(userId);
        }

        await _next(context);
    }
}