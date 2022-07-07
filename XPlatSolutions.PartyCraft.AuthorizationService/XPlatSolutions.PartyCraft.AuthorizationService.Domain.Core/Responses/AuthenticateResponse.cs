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
        this.Id = user.Id;
        this.Name = user.Name;
        this.LastName = user.LastName;
        this.SecondName = user.SecondName;
        this.Email = user.Email;
        this.Phone = user.Phone;
        this.Token = token;
        this.RefreshToken = refreshToken;
    }
}