using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RJEEC.Models
{
    public interface IEventPhotoRepository
    {
        IEnumerable<EventPhoto> GetAllEventPhotos(int eventId);
        EventPhoto GetFirstEventPhoto(int eventid);
        EventPhoto AddEventPhoto(EventPhoto newEventPhoto);
    }
}
