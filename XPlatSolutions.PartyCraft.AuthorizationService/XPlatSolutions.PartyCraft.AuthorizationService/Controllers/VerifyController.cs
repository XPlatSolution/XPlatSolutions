using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using XPlatSolutions.PartyCraft.AuthorizationService.BLL.Interfaces.Services;
using XPlatSolutions.PartyCraft.AuthorizationService.Domain.Core.Classes;
using XPlatSolutions.PartyCraft.AuthorizationService.Domain.Core.Models;
using XPlatSolutions.PartyCraft.AuthorizationService.Domain.Core.Responses;

namespace XPlatSolutions.PartyCraft.AuthorizationService.Controllers
{
    [Filters.Authorize]
    [ApiController]
    [Route("[controller]")]
    public class VerifyController : Controller
    {
        private readonly IUserService _userService;

        public VerifyController(IUserService userService, IOptions<AppOptions> appOptions)
        {
            _userService = userService;
        }

        [HttpGet("resend-activation-code")]
        [ServiceFilter(typeof(Filters.ResultFilterBase))]
        public async Task<OperationResult> ResendActivationCode()
        {
            var response = await _userService.ResendVerificationCode(HttpContext.Items["User"] as User);
            return response;
        }

        [AllowAnonymous]
        [ServiceFilter(typeof(Filters.ResultFilterBase))]
        [HttpGet("{id}")]
        public async Task<OperationResult> GetById(string id)
        {
            var user = await _userService.Verify(id);
            return user;
        }
    }
}
