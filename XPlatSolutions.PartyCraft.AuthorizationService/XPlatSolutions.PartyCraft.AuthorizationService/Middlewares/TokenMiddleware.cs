﻿using XPlatSolutions.PartyCraft.AuthorizationService.BLL.Interfaces.Services;
using XPlatSolutions.PartyCraft.AuthorizationService.BLL.Interfaces.Utils;
using XPlatSolutions.PartyCraft.AuthorizationService.Domain.Core.Classes;
using Microsoft.Extensions.Options;

namespace XPlatSolutions.PartyCraft.AuthorizationService.Middlewares;

public class TokenMiddleware
{
    private readonly RequestDelegate _next;

    public TokenMiddleware(RequestDelegate next, IOptions<AppOptions> appOptions)
    {
        _next = next;
    }

    public async Task Invoke(HttpContext context, IUserService userService, ITokenUtils tokenUtils)
    {
        var token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
        var userId = tokenUtils.ValidateToken(token);
        if (userId != null)
        {
            context.Items["User"] = (await userService.GetById(userId)).Result;
        }

        await _next(context);
    }
}