namespace XPlatSolutions.PartyCraft.AuthorizationService.Domain.Core.Requests;

public class ResetPasswordRequest
{
    public string Token { get; set; }
    public string NewPassword { get; set; }
}