using XPlatSolutions.PartyCraft.AuthorizationService.Domain.Core.Models;

namespace XPlatSolutions.PartyCraft.AuthorizationService.DAL.Interfaces.Dao;

public interface ITokensAccess
{
    Task<RefreshToken> GetUserToken(string userId);
    Task<bool> Contains(string token);
    Task AddToken(RefreshToken token);
    Task RemoveAllOldTokens(string userId);
    Task<RefreshToken> GetTokenByValue(string refreshToken);
    Task<List<RefreshToken>> GetTokensByUserId(string userId);
    Task UpdateToken(string tokenId, RefreshToken token);
}