using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XPlatSolutions.PartyCraft.EventService.Domain.Core.Enums;

namespace XPlatSolutions.PartyCraft.EventService.Domain.Core.Requests
{
    public class CreateEventRequest
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public DateTime StartDate { get; set; }
        public string Latitude { get; set; }
        public string Longitude { get; set; }
        public EventType Type { get; set; }
        public string ExactAddress { get; set; }
        public string ApproximateAddress { get; set; }
        public int MaxCountPeople { get; set; }
        public decimal Price { get; set; }
    }
}
