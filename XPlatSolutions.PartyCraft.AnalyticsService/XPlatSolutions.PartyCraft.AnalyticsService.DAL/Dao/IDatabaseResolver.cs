using MongoDB.Driver;

namespace XPlatSolutions.PartyCraft.AnalyticsService.DAL.Dao;

public interface IDatabaseResolver
{
    IMongoDatabase GetDatabase();
}