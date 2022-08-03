using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XPlatSolutions.PartyCraft.EventService.Domain.Core.Responses
{
    public class CreateEventResponse
    {
        public bool IsSuccess { get; set; }
        public string? Message { get; set; }
    }
}
