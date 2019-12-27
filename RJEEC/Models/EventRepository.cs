using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RJEEC.Models
{
    public class EventRepository : IEventRepository
    {
        private readonly RJEECDbContext context;

        public EventRepository(RJEECDbContext context)
        {
            this.context = context;
        }

        public Event AddEvent(Event newEvent)
        {
            context.Events.Add(newEvent);
            context.SaveChanges();
            return newEvent;
        }

        public Event Delete(int Id)
        {
            Event deletedEvent = context.Events.Find(Id);
            if (deletedEvent != null)
            {
                context.Events.Remove(deletedEvent);
                context.SaveChanges();
            }
            return deletedEvent;
        }

        public IEnumerable<Event> GetAllEvents()
        {
            return context.Events.Include(ep=>ep.EventPhotos);
        }

        public Event GetEvent(int Id)
        {
            return context.Events.Include(ep => ep.EventPhotos).FirstOrDefault(e=>e.Id==Id);
        }

        public Event Update(Event eventChanges)
        {
            var eventUpdated = context.Events.Attach(eventChanges);
            eventUpdated.State = Microsoft.EntityFrameworkCore.EntityState.Modified;
            context.SaveChanges();
            return eventChanges;
        }
    }
}
