using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RJEEC.Models
{
    public class EventRepository : IEventRepository
    {
        public List<Event> _eventList { get; set; }

        public EventRepository()
        {
            _eventList = new List<Event>()
            {
                new Event() { Id = 1, Name = "Event1", Date=new DateTime(), Description = "Event mary@pragimtech.com" },
                new Event() { Id = 2, Name = "Event2", Date=new DateTime(), Description = "Event john@pragimtech.com" },
                new Event() { Id = 3, Name = "Event3", Date=new DateTime(), Description = "Event sam@pragimtech.com" },
            };
        }
        public Event GetEvent(int id)
        {
            return _eventList.FirstOrDefault(e=>e.Id == id);
        }

        public IEnumerable<Event> GetAllEvents()
        {
            return _eventList;
        }
    }
}
