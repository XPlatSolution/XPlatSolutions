using Microsoft.AspNetCore.Mvc;
using XPlatSolutions.PartyCraft.EventBus.Interfaces;
using XPlatSolutions.PartyCraft.SpamService.DAL.Handlers;
using XPlatSolutions.PartyCraft.SpamService.Domain.Core.Models;

namespace XPlatSolutions.PartyCraft.SpamService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };


        public WeatherForecastController(IEventBus eventBus)
        {
        }

        [HttpGet(Name = "GetWeatherForecast")]
        public string Get()
        {
            return "";
        }
    }
}