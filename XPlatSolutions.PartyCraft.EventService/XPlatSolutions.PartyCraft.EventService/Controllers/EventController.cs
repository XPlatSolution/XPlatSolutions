using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using XPlatSolutions.PartyCraft.EventService.BLL.Interfaces.Services;
using XPlatSolutions.PartyCraft.EventService.Domain.Core.Models;
using XPlatSolutions.PartyCraft.EventService.Domain.Core.Requests;
using XPlatSolutions.PartyCraft.EventService.Domain.Core.Responses;

namespace XPlatSolutions.PartyCraft.EventService.Controllers
{
    [Route("[controller]")]
    [ApiController]
    public class EventController : ControllerBase
    {
        IEventService _eventService;
        public EventController(IEventService eventService)
        {
            _eventService = eventService;
        }

        [HttpPost("create-event")]
        public async Task<IActionResult> CreateEvent(CreateEventRequest createEventRequest)
        {
            var response = await _eventService.CreateAsync(createEventRequest);

            if (response.IsSuccess)
                return Ok(response);
            else
                return BadRequest(response);

        }

        [HttpGet("get-all")]
        public async Task<List<Event>> GetAll()
        {
            return await _eventService.GetAllAsync();
        }

        [HttpGet("{id}/get-by-id")]
        public async Task<Event> GetById(string id)
        {
            return await _eventService.GetById(id);

        }
    }
}
