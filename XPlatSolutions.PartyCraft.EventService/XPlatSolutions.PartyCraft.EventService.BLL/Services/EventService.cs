using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XPlatSolutions.PartyCraft.EventService.BLL.Interfaces.Services;
using XPlatSolutions.PartyCraft.EventService.DAL.Interfaces.Dao;
using XPlatSolutions.PartyCraft.EventService.Domain.Core.Models;
using XPlatSolutions.PartyCraft.EventService.Domain.Core.Requests;
using XPlatSolutions.PartyCraft.EventService.Domain.Core.Responses;

namespace XPlatSolutions.PartyCraft.EventService.BLL.Services
{
    public class EventService : IEventService
    {
        private readonly IEventsAccess _eventsAccess;

        public EventService(IEventsAccess eventsAccess)
        {
            _eventsAccess = eventsAccess;
        }

        public async Task<CreateEventResponse> CreateAsync(CreateEventRequest request)
        {
            try
            {
                if (request == null)
                    throw new Exception("Пустой запрос");

                if (string.IsNullOrEmpty(request.Name))
                    throw new Exception("Поле Name обязательное для заполнения");

                if (string.IsNullOrEmpty(request.Description))
                    throw new Exception("Поле Description обязательное для заполнения");

                var model = MapCreateEventRequestToEvent(request);

                await _eventsAccess.CreateAsync(model);
                return new CreateEventResponse { IsSuccess = true, Message = "Событие добавлено" };

            }
            catch (Exception ex)
            {
                return new CreateEventResponse { IsSuccess = false, Message = ex.Message };
            }
        }

        public async Task<List<Event>> GetAllAsync()
        {
            return await _eventsAccess.GetAllAsync();
        }

        public async Task<Event> GetById(string id)
        {
            return await _eventsAccess.GetById(id);
        }

        public static Event MapCreateEventRequestToEvent(CreateEventRequest request)
        {
            return new Event
            {
                Name = request.Name,
                Description = request.Description,
                StartDate = request.StartDate,
                Latitude = request.Latitude,
                Longitude = request.Longitude,
                Type = request.Type,
                ExactAddress = request.ExactAddress,
                ApproximateAddress = request.ApproximateAddress,
                MaxCountPeople = request.MaxCountPeople,
                Price = request.Price,
                Organizer = "CurrentTestUser",
                Status = Domain.Core.Enums.EventStatus.Actual,
                ExpirationDate = DateTime.MaxValue
            };
        }
    }
}
