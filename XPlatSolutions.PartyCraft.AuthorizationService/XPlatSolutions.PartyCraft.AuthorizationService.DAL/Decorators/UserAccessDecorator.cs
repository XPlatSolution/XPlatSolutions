using System.Text;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using XPlatSolutions.PartyCraft.AuthorizationService.DAL.Interfaces.Dao;
using XPlatSolutions.PartyCraft.AuthorizationService.Domain.Core.Classes;
using XPlatSolutions.PartyCraft.AuthorizationService.Domain.Core.Models;

namespace XPlatSolutions.PartyCraft.AuthorizationService.DAL.Decorators;

public class UserAccessDecorator : IUsersAccess
{
    private readonly IUsersAccess _usersAccess;
    private readonly IOptions<AppOptions> _appOptions;
    private readonly IDistributedCache _cache;

    public UserAccessDecorator(IUsersAccess usersAccess, IOptions<AppOptions> appOptions, IDistributedCache cache)
    {
        _usersAccess = usersAccess;
        _appOptions = appOptions;
        _cache = cache;
    }


    public Task<User?> GetUser(string email, string password)
    {
        return _usersAccess.GetUser(email, password);
    }

    public async Task<User?> GetUserById(string? id)
    {
        //TODO: потом пересмотреть, сделано для примера работы с Redis
        if (!_appOptions.Value.IsCacheEnabled)
            return await _usersAccess.GetUserById(id);

        User? user;

        if (string.IsNullOrWhiteSpace(id)) return null;

        var cachedData = await _cache.GetAsync(id);
        if (cachedData != null)
        {
            var cachedDataString = Encoding.UTF8.GetString(cachedData);
            user = JsonConvert.DeserializeObject<User>(cachedDataString);
        }
        else
        {
            user = await _usersAccess.GetUserById(id);

            if(user == null) return null;

            var dataToCache = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(user));
            
            var options = new DistributedCacheEntryOptions()
                .SetAbsoluteExpiration(DateTime.Now.AddMinutes(5))
                .SetSlidingExpiration(TimeSpan.FromMinutes(3));
            
            await _cache.SetAsync(user.Id, dataToCache, options);
        }

        return user;
    }

    public Task<User?> GetUserByEmail(string? email)
    {
        return _usersAccess.GetUserByEmail(email);
    }

    public Task<List<User>> GetAll()
    {
        return _usersAccess.GetAll();
    }

    public Task<bool> AddUser(User user)
    {
        return _usersAccess.AddUser(user);
    }

    public Task SetVerified(string userId)
    {
        return _usersAccess.SetVerified(userId);
    }

    public Task ChangePassword(string userId, string passwordHash)
    {
        return _usersAccess.ChangePassword(userId, passwordHash);
    }
}