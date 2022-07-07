using Microsoft.Extensions.Options;
using MongoDB.Driver;
using XPlatSolutions.PartyCraft.AuthorizationService.Domain.Core.Classes;
using XPlatSolutions.PartyCraft.AuthorizationService.Domain.Core.Models;

namespace XPlatSolutions.PartyCraft.AuthorizationService.DAL.Dao;

public class DatabaseResolver : IDatabaseResolver
{
    private readonly IMongoDatabase _database;
    public DatabaseResolver(IOptions<AppOptions> appOptions)
    {
        var mongoClient = new MongoClient(
            appOptions.Value.ConnectionString);

        _database = mongoClient.GetDatabase(
            appOptions.Value.DatabaseName);
    }

    public IMongoDatabase GetDatabase()
    {
        return _database;
    }
}