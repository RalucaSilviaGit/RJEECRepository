using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RJEEC.Models
{
    public class EventRepository : IEventRepository
    {
        public List<Event> _eventList { get; set; }
        public Event GetEvent(int id)
        {
            return _eventList.FirstOrDefault(e=>e.Id == id);
        }
    }
}
