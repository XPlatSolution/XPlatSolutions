using System.ComponentModel.DataAnnotations;

namespace XPlatSolutions.PartyCraft.AuthorizationService.Domain.Core.Requests;

public class AuthenticateRequest
{
    [Required]
    public string Email { get; set; }

    [Required]
    public string Password { get; set; }
}