using XPlatSolutions.PartyCraft.AuthorizationService.DAL.Interfaces.Dao;
using XPlatSolutions.PartyCraft.AuthorizationService.Domain.Core.Classes;
using XPlatSolutions.PartyCraft.AuthorizationService.Domain.Core.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace XPlatSolutions.PartyCraft.AuthorizationService.DAL.Dao;

public class TokensAccess : ITokensAccess
{
    private readonly IMongoCollection<RefreshToken> _tokensCollection;
    private readonly IOptions<AppOptions> _appOptions;

    public TokensAccess(IOptions<AppOptions> appOptions, IDatabaseResolver databaseResolver)
    {
        _tokensCollection = databaseResolver.GetDatabase().GetCollection<RefreshToken>(
            appOptions.Value.TokensCollectionName); 

        _appOptions = appOptions;
    }

    public Task<RefreshToken> GetUserToken(string userId)
    {
        return _tokensCollection
            .Find(x =>
                x.UserId == userId)
            .FirstOrDefaultAsync();
    }

    public async Task<bool> Contains(string token)
    {
        return await _tokensCollection
            .Find(x =>
                x.Value == token)
            .FirstOrDefaultAsync() != null;
    }

    public Task AddToken(RefreshToken token)
    {
        return _tokensCollection
            .InsertOneAsync(token);
    }

    public Task RemoveAllOldTokens(string userId)
    {
        return _tokensCollection.DeleteManyAsync(x => x.RevokeDateTime != null
                                                      || x.ExpireDateTime <= DateTime.UtcNow
                                                      && x.CreationDateTime <= DateTime.UtcNow.AddDays(-_appOptions.Value
                                                          .RefreshTokenTTL));
    }

    public Task<RefreshToken> GetTokenByValue(string refreshToken)
    {
        return _tokensCollection
            .Find(x =>
                x.Value == refreshToken)
            .FirstOrDefaultAsync();
    }

    public Task<List<RefreshToken>> GetTokensByUserId(string userId)
    {
        return _tokensCollection
            .Find(x =>
                x.UserId == userId).ToListAsync();
    }

    public Task UpdateToken(string tokenId, RefreshToken token)
    {
        return _tokensCollection.ReplaceOneAsync(x => x.Id == tokenId, token);
    }
}