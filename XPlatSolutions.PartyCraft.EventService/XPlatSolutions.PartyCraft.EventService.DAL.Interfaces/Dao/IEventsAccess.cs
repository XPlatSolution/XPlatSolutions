using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XPlatSolutions.PartyCraft.EventService.Domain.Core.Models;

namespace XPlatSolutions.PartyCraft.EventService.DAL.Interfaces.Dao
{
    public interface IEventsAccess
    {
        Task CreateAsync(Event model);
        Task<List<Event>> GetAllAsync();
        Task<Event> GetById(string id);
    }
}
