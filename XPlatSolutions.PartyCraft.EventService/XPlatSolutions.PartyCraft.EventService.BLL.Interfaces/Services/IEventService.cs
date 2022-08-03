using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XPlatSolutions.PartyCraft.EventService.Domain.Core.Models;
using XPlatSolutions.PartyCraft.EventService.Domain.Core.Requests;
using XPlatSolutions.PartyCraft.EventService.Domain.Core.Responses;

namespace XPlatSolutions.PartyCraft.EventService.BLL.Interfaces.Services
{
    public interface IEventService
    {
        Task<CreateEventResponse> CreateAsync(CreateEventRequest request);
        Task<List<Event>> GetAllAsync();
        Task<Event> GetById(string id);
    }
}
