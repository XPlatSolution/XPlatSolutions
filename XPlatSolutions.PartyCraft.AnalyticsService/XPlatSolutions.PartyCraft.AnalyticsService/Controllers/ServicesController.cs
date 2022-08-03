using Microsoft.AspNetCore.Mvc;
using XPlatSolutions.PartyCraft.AnalyticsService.BLL.Interfaces.Services;
using XPlatSolutions.PartyCraft.AnalyticsService.Domain.Core.Requests;
using XPlatSolutions.PartyCraft.AnalyticsService.Domain.Core.Responses;

namespace XPlatSolutions.PartyCraft.AnalyticsService.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ServicesController : Controller
    {
        private readonly IServiceService _serviceService;
        private readonly IExceptionMessageService _exceptionMessageService;

        public ServicesController(IServiceService serviceService, IExceptionMessageService exceptionMessageService)
        {
            _serviceService = serviceService;
            _exceptionMessageService = exceptionMessageService;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(string id)
        {
            var service = await _serviceService.GetService(id);

            if (service == null)
                return BadRequest(new { message = "Wrong service id" });

            var exceptions = await _exceptionMessageService.GetExceptionMessagesList(service.Id);

            return Ok(new ServiceResponse { ExceptionMessages = exceptions, Service = service });
        }

        [HttpPost("get-with-filters")]
        public async Task<IActionResult> GetWithFilters(ExceptionsRequest? request)
        {
            if (request == null)
                return BadRequest(new { message = "Wrong params" });

            if (request.ServiceId == null)
                return BadRequest(new { message = "Wrong params" });

            var service = await _serviceService.GetService(request.ServiceId);

            if (service == null)
                return BadRequest(new { message = "Wrong service id" });

            var exceptions = (await _exceptionMessageService.GetExceptionMessagesList(request)).Exceptions;

            return Ok(new ServiceResponse { ExceptionMessages = exceptions, Service = service });
        }


        [HttpGet("get-all")]
        public async Task<IActionResult> GetAll()
        {
            return Ok(new { services = await _serviceService.GetServicesList() });
        }
    }
}
