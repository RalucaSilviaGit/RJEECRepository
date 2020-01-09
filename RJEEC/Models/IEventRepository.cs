using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RJEEC.Models
{
    public interface IEventRepository
    {
        IEnumerable<Event> GetAllEvents();
        IEnumerable<Event> GetNext5Events();
        Event GetEvent(int id);
        Event AddEvent(Event newEvent);
        Event Update(Event eventChanges);
        Event Delete(int id);
    }
}
