using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RJEEC.Models
{
    public interface IEventRepository
    {
        IEnumerable<Event> GetAllEvents();
        Event GetEvent(int id);
    }
}
