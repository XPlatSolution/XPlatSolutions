using XPlatSolutions.PartyCraft.AuthorizationService.BLL.Interfaces.Services;
using XPlatSolutions.PartyCraft.AuthorizationService.Domain.Core.Exceptions;
using XPlatSolutions.PartyCraft.AuthorizationService.Domain.Core.Requests;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using XPlatSolutions.PartyCraft.AuthorizationService.Domain.Core.Classes;
using XPlatSolutions.PartyCraft.AuthorizationService.Domain.Core.Enums;
using XPlatSolutions.PartyCraft.AuthorizationService.Domain.Core.Models;
using XPlatSolutions.PartyCraft.AuthorizationService.Domain.Core.Responses;
using XPlatSolutions.PartyCraft.EventBus.Interfaces;

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
        [ServiceFilter(typeof(Filters.ResultFilter<AuthenticateResponse>))]
        [HttpPost("authenticate")]
        public async Task<OperationResult<AuthenticateResponse>> Authenticate(AuthenticateRequest request)
        {
            var response = await _userService.Authenticate(request, GetUserIp());

            if(response.Result!= null)
                SetTokenCookie(response.Result.RefreshToken);

            return response;
        }

        [AllowAnonymous]
        [ServiceFilter(typeof(Filters.ResultFilter<AuthenticateResponse>))]
        [HttpPost("refresh-token")]
        public async Task<OperationResult<AuthenticateResponse>> RefreshToken()
        {
            var refreshToken = Request.Cookies["refreshToken"] ?? string.Empty;
            var response = await _userService.RefreshToken(refreshToken, GetUserIp());
            
            if (response.Result != null)
                SetTokenCookie(response.Result.RefreshToken);

            return response;
        }

        [AllowAnonymous]
        [ServiceFilter(typeof(Filters.ResultFilter<RegisterResponse>))]
        [HttpPost("register")]
        public async Task<OperationResult<RegisterResponse>> Register(RegisterRequest? model)
        {
            var response = await _userService.Register(model);
            
            return response;
        }

        [AllowAnonymous]
        [ServiceFilter(typeof(Filters.ResultFilter<RestorePasswordResponse>))]
        [HttpPost("restore-password")]
        public async Task<OperationResult<RestorePasswordResponse>> RestorePassword(RestorePasswordRequest? model)
        {
            var response = await _userService.RestorePasswordRequest(model);
            
            return response;
        }

        [ServiceFilter(typeof(Filters.ResultFilter<>))]
        [HttpPost("revoke-token")]
        public async Task<OperationResult> RevokeToken(RevokeTokenRequest? model)
        {
            var token = model?.Token ?? Request.Cookies["refreshToken"];
            
            var result = await _userService.RevokeToken(token ?? "", GetUserIp());

            return result;
        }

        [EmailVerifiedRequire]
        [ServiceFilter(typeof(Filters.ResultFilter<List<User>>))]
        [HttpGet]
        public async Task<OperationResult<List<User>>> GetAll()
        {
            var response = await _userService.GetAll();
            
            return response;
        }

        [EmailVerifiedRequire]
        [ServiceFilter(typeof(Filters.ResultFilter<User>))]
        [HttpGet("{id}")]
        public async Task<OperationResult<User>> GetById(string id)
        {
            var response = await _userService.GetById(id);
            
            return response;
        }

        [EmailVerifiedRequire]
        [ServiceFilter(typeof(Filters.ResultFilter<User>))]
        [HttpGet("{id}/refresh-tokens")]
        public async Task<OperationResult<User?>> GetRefreshTokens(string id)
        {
            var response = await _userService.GetById(id);
            
            return response;
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
