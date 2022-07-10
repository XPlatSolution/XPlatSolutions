using XPlatSolutions.PartyCraft.AuthorizationService.Domain.Core.Models;
using System.Text.Json.Serialization;

namespace XPlatSolutions.PartyCraft.AuthorizationService.Domain.Core.Responses;

public class AuthenticateResponse
{
    [JsonIgnore]
    public string Id { get; set; }
    public string Name { get; set; }
    public string LastName { get; set; }
    public string? SecondName { get; set; }
    public string? Email { get; set; }
    public string? Phone { get; set; }
    public string RefreshToken { get; set; }
    public string Token { get; set; }

    public AuthenticateResponse(User user, string token, string refreshToken)
    {
        Id = user.Id;
        Name = user.Name;
        LastName = user.LastName;
        SecondName = user.SecondName;
        Email = user.Email;
        Phone = user.Phone;
        Token = token;
        RefreshToken = refreshToken;
    }
}