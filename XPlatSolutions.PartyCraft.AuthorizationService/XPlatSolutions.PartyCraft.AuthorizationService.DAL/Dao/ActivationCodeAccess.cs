using Microsoft.Extensions.Options;
using MongoDB.Driver;
using XPlatSolutions.PartyCraft.AuthorizationService.DAL.Interfaces.Dao;
using XPlatSolutions.PartyCraft.AuthorizationService.Domain.Core.Classes;
using XPlatSolutions.PartyCraft.AuthorizationService.Domain.Core.Models;

namespace XPlatSolutions.PartyCraft.AuthorizationService.DAL.Dao;

public class ActivationCodeAccess : IActivationCodeAccess
{
    private readonly IMongoCollection<ActivationCode> _activationCodesCollection;
    private readonly IOptions<AppOptions> _appOptions;

    public ActivationCodeAccess(IDatabaseResolver databaseResolver, IOptions<AppOptions> appOptions)
    {
        _activationCodesCollection = databaseResolver.GetDatabase().GetCollection<ActivationCode>(
            appOptions.Value.ActivationCodeCollectionName);
        _appOptions = appOptions;
    }
    public Task<List<ActivationCode>> GetActivationCodes(string userId)
    {
        return _activationCodesCollection.Find(x => x.UserId == userId).ToListAsync();
    }

    public Task<ActivationCode> GetActivationCode(string value)
    {
        return _activationCodesCollection.Find(x => x.Value == value).FirstOrDefaultAsync();
    }

    public async Task<bool> ContainsValid(string value)
    {
        return await _activationCodesCollection
            .Find(x =>
                x.Value == value && x.CreationDateTime.AddMinutes(_appOptions.Value.ActivationCodeMinutesTTL) > DateTime.UtcNow)
            .FirstOrDefaultAsync() != null;
    }

    public Task AddActivationCode(ActivationCode code)
    {
        return _activationCodesCollection
            .InsertOneAsync(code);
    }

    public Task RemoveAllActivationCodes(string userId)
    {
        return _activationCodesCollection.DeleteManyAsync(x => x.UserId == userId);
    }
}