using XPlatSolutions.PartyCraft.AuthorizationService.Domain.Core.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using XPlatSolutions.PartyCraft.AuthorizationService.Domain.Core.Classes;

namespace XPlatSolutions.PartyCraft.AuthorizationService.Filters;

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
public class AuthorizeAttribute : Attribute, IAuthorizationFilter
{
    public void OnAuthorization(AuthorizationFilterContext context)
    {
        var hasAnonymousAttribute = context.ActionDescriptor.EndpointMetadata.OfType<AllowAnonymousAttribute>().Any();
        if (hasAnonymousAttribute)
            return;

        if (context.HttpContext.Items["User"] is not User user)
        {
            context.Result = new JsonResult(new { message = "Unauthorized" }) { StatusCode = 401 };
            return;
        }
        
        if(user.EmailVerified == false && context.ActionDescriptor.EndpointMetadata.OfType<EmailVerifiedRequire>().Any())
            context.Result = new JsonResult(new { message = "Email not verified" }) { StatusCode = 401 };
    }
}