using XPlatSolutions.PartyCraft.AuthorizationService.Domain.Core.Models;
using XPlatSolutions.PartyCraft.AuthorizationService.Domain.Core.Requests;
using XPlatSolutions.PartyCraft.AuthorizationService.Domain.Core.Responses;

namespace XPlatSolutions.PartyCraft.AuthorizationService.BLL.Interfaces.Services;

public interface IUserService
{
    Task<AuthenticateResponse> Authenticate(AuthenticateRequest request, string userIp);
    Task<AuthenticateResponse> RefreshToken(string refreshToken, string userIp);
    Task<RegisterResponse> Register(RegisterRequest? request);
    Task RevokeToken(string refreshToken, string userIp);
    Task<User?> GetById(string userId);
    Task<List<User>> GetAll();
    Task<bool> Verify(string verifyCode);
    Task<RestorePasswordResponse> RestorePasswordRequest(RestorePasswordRequest? request);
    Task<RestorePasswordResponse> RestorePassword(ResetPasswordRequest? request);
    Task ResendVerificationCode(User user);
}