using System.ComponentModel.DataAnnotations;

namespace XPlatSolutions.PartyCraft.AuthorizationService.Domain.Core.Requests;

public class RegisterRequest
{
    [Required]
    public string Name { get; set; }
    [Required]
    public string LastName { get; set; }
    public DateTime BirthdayDateTime { get; set; }
    public string? SecondName { get; set; }
    [Required]
    public string Email { get; set; }
    public string? Phone { get; set; }
    [Required]
    public string Password { get; set; }
}