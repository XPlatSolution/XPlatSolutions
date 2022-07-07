using MongoDB.Driver;

namespace XPlatSolutions.PartyCraft.AuthorizationService.DAL.Dao;

public interface IDatabaseResolver
{
    IMongoDatabase GetDatabase();
}