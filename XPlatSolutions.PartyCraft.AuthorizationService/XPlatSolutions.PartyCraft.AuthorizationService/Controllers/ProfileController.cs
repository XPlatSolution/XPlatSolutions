using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using XPlatSolutions.PartyCraft.AuthorizationService.BLL.Interfaces.Services;
using XPlatSolutions.PartyCraft.AuthorizationService.Domain.Core.Classes;
using XPlatSolutions.PartyCraft.AuthorizationService.Domain.Core.Models;
using XPlatSolutions.PartyCraft.AuthorizationService.Domain.Core.Requests;
using XPlatSolutions.PartyCraft.AuthorizationService.Domain.Core.Responses;

namespace XPlatSolutions.PartyCraft.AuthorizationService.Controllers
{
    [Filters.Authorize]
    [ApiController]
    [Route("[controller]")]
    public class ProfileController : Controller
    {
        private readonly IUserService _userService;

        public ProfileController(IUserService userService, IOptions<AppOptions> appOptions)
        {
            _userService = userService;
        }

        [AllowAnonymous]
        [ServiceFilter(typeof(Filters.ResultFilter<RestorePasswordResponse>))]
        [HttpPost("ResetPassword")]
        public async Task<OperationResult<RestorePasswordResponse>> ResetPassword(ResetPasswordRequest request)
        {
            var response = await _userService.RestorePassword(request);

            return response;
        }
    }
}
