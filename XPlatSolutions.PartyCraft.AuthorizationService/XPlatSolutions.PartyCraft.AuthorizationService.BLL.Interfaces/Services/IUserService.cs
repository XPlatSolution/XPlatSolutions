using XPlatSolutions.PartyCraft.AuthorizationService.Domain.Core.Models;
using XPlatSolutions.PartyCraft.AuthorizationService.Domain.Core.Requests;
using XPlatSolutions.PartyCraft.AuthorizationService.Domain.Core.Responses;

namespace XPlatSolutions.PartyCraft.AuthorizationService.BLL.Interfaces.Services;

public interface IUserService
{
    Task<OperationResult<AuthenticateResponse>> Authenticate(AuthenticateRequest request, string userIp);
    Task<OperationResult<AuthenticateResponse>> RefreshToken(string refreshToken, string userIp);
    Task<OperationResult<RegisterResponse>> Register(RegisterRequest? request);
    Task<OperationResult> RevokeToken(string refreshToken, string userIp);
    Task<OperationResult<User?>> GetById(string userId);
    Task<OperationResult<List<User>>> GetAll();
    Task<OperationResult> Verify(string verifyCode);
    Task<OperationResult<RestorePasswordResponse>> RestorePasswordRequest(RestorePasswordRequest? request);
    Task<OperationResult<RestorePasswordResponse>> RestorePassword(ResetPasswordRequest? request);
    Task<OperationResult> ResendVerificationCode(User? user);
}