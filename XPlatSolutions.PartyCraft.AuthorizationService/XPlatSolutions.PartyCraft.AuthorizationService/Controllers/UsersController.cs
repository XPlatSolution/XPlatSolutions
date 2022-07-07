using XPlatSolutions.PartyCraft.AuthorizationService.BLL.Interfaces.Services;
using XPlatSolutions.PartyCraft.AuthorizationService.Domain.Core.Exceptions;
using XPlatSolutions.PartyCraft.AuthorizationService.Domain.Core.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using XPlatSolutions.PartyCraft.AuthorizationService.Domain.Core.Classes;

namespace XPlatSolutions.PartyCraft.AuthorizationService.Controllers
{
    [Filters.Authorize]
    [ApiController]
    [Route("[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IOptions<AppOptions> _options;

        public UsersController(IUserService userService, IOptions<AppOptions> appOptions)
        {
            _userService = userService;
            _options = appOptions;
        }

        [AllowAnonymous]
        [HttpPost("authenticate")]
        public async Task<IActionResult> Authenticate(AuthenticateRequest request)
        {
            var response = await _userService.Authenticate(request, GetUserIp());
            SetTokenCookie(response.RefreshToken);
            return Ok(response);
        }

        [AllowAnonymous]
        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken()
        {
            var refreshToken = Request.Cookies["refreshToken"] ?? string.Empty;
            var response = await _userService.RefreshToken(refreshToken, GetUserIp());
            SetTokenCookie(response.RefreshToken);
            return Ok(response);
        }
        
        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterRequest? model)
        {
            var response = await _userService.Register(model);
            return Ok(response);
        }
        
        [AllowAnonymous]
        [HttpPost("restore-password")]
        public async Task<IActionResult> RestorePassword(RestorePasswordRequest? model)
        {
            var response = await _userService.RestorePasswordRequest(model);
            return Ok(response);
        }
        
        [HttpPost("revoke-token")]
        public async Task<IActionResult> RevokeToken(RevokeTokenRequest? model)
        {
            var token = model?.Token ?? Request.Cookies["refreshToken"];

            if (string.IsNullOrEmpty(token))
                return BadRequest(new { message = "Token is required" });

            await _userService.RevokeToken(token, GetUserIp());
            return Ok(new { message = "Token revoked" });
        }

        [EmailVerifiedRequire]
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var users = await _userService.GetAll();
            return Ok(users);
        }

        [EmailVerifiedRequire]
        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var user = await _userService.GetById(id);
            return Ok(user);
        }

        [EmailVerifiedRequire]
        [HttpGet("{id}/refresh-tokens")]
        public async Task<IActionResult> GetRefreshTokens(string id)
        {
            var user = await _userService.GetById(id);
            return Ok(user);
        }

        private void SetTokenCookie(string token)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = DateTime.UtcNow.AddDays(_options.Value.CookieTTL)
            };
            Response.Cookies.Append("refreshToken", token, cookieOptions);
        }

        private string GetUserIp()
        {
            if (Request.Headers.ContainsKey("X-Forwarded-For"))
                return Request.Headers["X-Forwarded-For"];

            return HttpContext.Connection.RemoteIpAddress?.MapToIPv4().ToString() ?? string.Empty;
        }
    }
}
