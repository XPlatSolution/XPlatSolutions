using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XPlatSolutions.PartyCraft.EventService.DAL.Interfaces.Dao;
using XPlatSolutions.PartyCraft.EventService.Domain.Core.Models;

namespace XPlatSolutions.PartyCraft.EventService.DAL.Dao
{
    public class EventsAccess : IEventsAccess
    {
        private readonly IMongoCollection<Event> _eventCollection;

        public EventsAccess(IDatabaseResolver databaseResolver)
        {
            _eventCollection = databaseResolver.GetDatabase().GetCollection<Event>("events");
        }

        public async Task CreateAsync(Event model)
        {
            await _eventCollection.InsertOneAsync(model);
        }

        public async Task<List<Event>> GetAllAsync()
        {
            return await _eventCollection.Find(x => true).ToListAsync();
        }

        public async Task<Event> GetById(string id)
        {
            return await _eventCollection.Find(x => x.Id == id).FirstOrDefaultAsync();
        }
    }
}
