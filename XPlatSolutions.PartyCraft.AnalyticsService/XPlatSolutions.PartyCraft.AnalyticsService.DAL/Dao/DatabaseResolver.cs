using Microsoft.Extensions.Options;
using MongoDB.Driver;
using XPlatSolutions.PartyCraft.AnalyticsService.Domain.Core.Classes;

namespace XPlatSolutions.PartyCraft.AnalyticsService.DAL.Dao;

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