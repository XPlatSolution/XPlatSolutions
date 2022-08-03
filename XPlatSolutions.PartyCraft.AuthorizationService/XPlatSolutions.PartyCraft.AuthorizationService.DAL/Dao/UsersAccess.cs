using XPlatSolutions.PartyCraft.AuthorizationService.DAL.Interfaces.Dao;
using XPlatSolutions.PartyCraft.AuthorizationService.Domain.Core.Classes;
using XPlatSolutions.PartyCraft.AuthorizationService.Domain.Core.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace XPlatSolutions.PartyCraft.AuthorizationService.DAL.Dao;

public class UsersAccess : IUsersAccess
{
    private readonly IMongoCollection<User> _usersCollection;

    public UsersAccess(IOptions<AppOptions> appOptions, IDatabaseResolver databaseResolver)
    {
        _usersCollection = databaseResolver.GetDatabase().GetCollection<User>(
            appOptions.Value.UsersCollectionName);
    }

    public async Task<User?> GetUser(string email, string password)
    {
        var user = await _usersCollection
            .Find(x =>
                    x.Email == email)
            .FirstOrDefaultAsync();

        if (user == null) return null;

        return BCrypt.Net.BCrypt.Verify(password, user.PasswordHash) ? user : null;
    }

    public Task<User?> GetUserById(string? id)
    {
        return _usersCollection.Find(x => x.Id == id).FirstOrDefaultAsync();
    }
    
    public Task<User?> GetUserByEmail(string? email)
    {
        return _usersCollection.Find(x => x.Email == email).FirstOrDefaultAsync();
    }

    public Task<List<User>> GetAll()
    {
        return _usersCollection.Find(Builders<User>.Filter.Empty).ToListAsync();
    }

    public async Task<bool> AddUser(User user)
    {
        try
        {
            await _usersCollection.InsertOneAsync(user);
            return true;
        }
        catch
        {
            return false;
        }
    }

    public Task SetVerified(string userId)
    {
        var filter = Builders<User>.Filter.Where(x => x.Id == userId);
        var update = Builders<User>.Update.Set(x => x.EmailVerified, true);
        var options = new FindOneAndUpdateOptions<User>();
        return _usersCollection.FindOneAndUpdateAsync(filter, update, options);
    }

    public Task ChangePassword(string userId, string passwordHash)
    {
        var filter = Builders<User>.Filter.Where(x => x.Id == userId);
        var update = Builders<User>.Update.Set(x => x.PasswordHash, passwordHash);
        var options = new FindOneAndUpdateOptions<User>();
        return _usersCollection.FindOneAndUpdateAsync(filter, update, options);
    }

    public Task CreateUser(User user)
    {
        return _usersCollection
            .InsertOneAsync(user);
    }
}