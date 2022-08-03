using Microsoft.Extensions.Options;
using MongoDB.Driver;
using XPlatSolutions.PartyCraft.AuthorizationService.DAL.Interfaces.Dao;
using XPlatSolutions.PartyCraft.AuthorizationService.Domain.Core.Classes;
using XPlatSolutions.PartyCraft.AuthorizationService.Domain.Core.Models;

namespace XPlatSolutions.PartyCraft.AuthorizationService.DAL.Dao;

public class PasswordChangeRequestAccess : IPasswordChangeRequestAccess
{
    private readonly IMongoCollection<PasswordChangeRequest> _passwordChangeRequestCollection;

    public PasswordChangeRequestAccess(IDatabaseResolver databaseResolver, IOptions<AppOptions> appOptions)
    {
        _passwordChangeRequestCollection = databaseResolver.GetDatabase().GetCollection<PasswordChangeRequest>(
            appOptions.Value.PasswordChangeRequestCollectionName);
    }

    public Task<List<PasswordChangeRequest>> GetPasswordChangeRequests(string userId)
    {
        return _passwordChangeRequestCollection.Find(x => x.UserId == userId).ToListAsync();
    }

    public Task<PasswordChangeRequest> GetPasswordChangeRequest(string id)
    {
        return _passwordChangeRequestCollection.Find(x => x.Id == id).FirstOrDefaultAsync();
    }

    public Task AddPasswordChangeRequests(PasswordChangeRequest code)
    {
        return _passwordChangeRequestCollection
            .InsertOneAsync(code);
    }

    public Task RemoveAllPasswordChangeRequests(string userId)
    {
        return _passwordChangeRequestCollection.DeleteManyAsync(x => x.UserId == userId);
    }
}