using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RJEEC.Models
{
    public class EventPhotoRepository : IEventPhotoRepository
    {
        private readonly RJEECDbContext context;

        public EventPhotoRepository(RJEECDbContext context)
        {
            this.context = context;
        }

        public EventPhoto AddEventPhoto(EventPhoto newEventPhoto)
        {
            context.EventPhotos.Add(newEventPhoto);
            context.SaveChanges();
            return newEventPhoto;
        }

        public IEnumerable<EventPhoto> GetAllEventPhotos(int eventId)
        {
            return context.EventPhotos.Where( ep => ep.EventId == eventId);
        }

        public EventPhoto GetFirstEventPhoto(int eventId)
        {
            return context.EventPhotos.FirstOrDefault(ep => ep.EventId == eventId);
        }
    }
}
