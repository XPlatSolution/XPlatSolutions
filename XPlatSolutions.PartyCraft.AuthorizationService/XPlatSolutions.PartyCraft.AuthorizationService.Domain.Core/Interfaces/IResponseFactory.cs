using XPlatSolutions.PartyCraft.AuthorizationService.Domain.Core.Models;
using XPlatSolutions.PartyCraft.AuthorizationService.Domain.Core.Responses;

namespace XPlatSolutions.PartyCraft.AuthorizationService.Domain.Core.Interfaces;

public interface IResponseFactory
{
    AuthenticateResponse CreateAuthenticateResponse(User user, string token, string refreshToken);
    RestorePasswordResponse CreateRestorePasswordResponse(bool success);
    RegisterResponse CreateRegisterResponse(bool success);
}