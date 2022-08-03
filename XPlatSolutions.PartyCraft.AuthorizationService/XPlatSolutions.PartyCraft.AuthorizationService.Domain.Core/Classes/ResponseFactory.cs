using XPlatSolutions.PartyCraft.AuthorizationService.Domain.Core.Interfaces;
using XPlatSolutions.PartyCraft.AuthorizationService.Domain.Core.Models;
using XPlatSolutions.PartyCraft.AuthorizationService.Domain.Core.Responses;

namespace XPlatSolutions.PartyCraft.AuthorizationService.Domain.Core.Classes;

public class ResponseFactory : IResponseFactory
{
    public AuthenticateResponse CreateAuthenticateResponse(User user, string token, string refreshToken)
    {
        return new AuthenticateResponse(user, token, refreshToken);
    }

    public RestorePasswordResponse CreateRestorePasswordResponse(bool success)
    {
        return new RestorePasswordResponse { Success = success };
    }

    public RegisterResponse CreateRegisterResponse(bool success)
    {
        return new RegisterResponse { Success = success };
    }
}