using System.ComponentModel.DataAnnotations;

namespace XPlatSolutions.PartyCraft.AuthorizationService.Domain.Core.Requests;

public class RevokeTokenRequest
{
    [Required]
    public string? Token { get; set; }
}