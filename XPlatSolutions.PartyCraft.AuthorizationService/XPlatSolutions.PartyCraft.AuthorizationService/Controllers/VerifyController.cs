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
        public async Task<IActionResult> ResendActivationCode()
        {
            if (HttpContext.Items["User"] is not User user)
                return Unauthorized();

            await _userService.ResendVerificationCode(user);
            return Ok(new RegisterResponse { Success = true });
        }

        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var user = await _userService.Verify(id);
            return Ok(user);
        }
    }
}
