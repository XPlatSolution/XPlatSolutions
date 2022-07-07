using XPlatSolutions.PartyCraft.AuthorizationService.Domain.Core.Models;

namespace XPlatSolutions.PartyCraft.AuthorizationService.BLL.Interfaces.Utils;

public interface ITokenUtils
{ 
    string GenerateToken(User user);
    string? ValidateToken(string? token);
    Task<RefreshToken> GenerateRefreshToken(string ipAddress, string userId);
    Task AddToken(RefreshToken token);
    Task RemoveAllOldTokens(string userId);
    Task<string> GetUserIdByRefreshToken(string refreshToken);
    Task<RefreshToken> GetTokenByValue(string refreshToken);
    Task<List<RefreshToken>> GetTokensByUserId(string userId);
    Task UpdateToken(string tokenId, RefreshToken token);
    string GenerateIdToken(string email);
    string? ValidateIdToken(string id);
}