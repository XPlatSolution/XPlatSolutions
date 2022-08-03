using Microsoft.Extensions.Options;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XPlatSolutions.PartyCraft.EventService.Domain.Core.Classes;

namespace XPlatSolutions.PartyCraft.EventService.DAL.Dao
{
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
}
