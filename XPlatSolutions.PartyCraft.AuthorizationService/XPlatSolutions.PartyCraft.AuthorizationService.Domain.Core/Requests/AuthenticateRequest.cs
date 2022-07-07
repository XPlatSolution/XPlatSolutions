using System.ComponentModel.DataAnnotations;

namespace XPlatSolutions.PartyCraft.AuthorizationService.Domain.Core.Requests;

public class AuthenticateRequest
{
    [Required]
    public string Login { get; set; }

    [Required]
    public string Password { get; set; }
}