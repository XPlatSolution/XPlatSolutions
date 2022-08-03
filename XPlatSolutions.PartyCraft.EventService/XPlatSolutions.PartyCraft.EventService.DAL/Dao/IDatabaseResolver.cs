using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XPlatSolutions.PartyCraft.EventService.DAL.Dao
{
    public interface IDatabaseResolver
    {
        IMongoDatabase GetDatabase();
    }
}
