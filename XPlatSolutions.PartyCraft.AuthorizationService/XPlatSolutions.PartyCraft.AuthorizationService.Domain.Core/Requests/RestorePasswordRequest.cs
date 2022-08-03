using System.ComponentModel.DataAnnotations;

namespace XPlatSolutions.PartyCraft.AuthorizationService.Domain.Core.Requests;

public class RestorePasswordRequest
{
    [Required]
    public string Email { get; set; }
}