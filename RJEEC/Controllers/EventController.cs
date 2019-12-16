using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RJEEC.Models;
using RJEEC.ViewModels;

namespace RJEEC.Controllers
{
    public class EventController : Controller
    {
        private readonly IEventRepository _eventRepository;
        private readonly IEventPhotoRepository eventPhotoRepository;
        private readonly IHostingEnvironment hostingEnvironment;

        public EventController(IEventRepository eventRepository, 
                            IEventPhotoRepository eventPhotoRepository, 
                            IHostingEnvironment hostingEnvironment)
        {
            this._eventRepository = eventRepository;
            this.eventPhotoRepository = eventPhotoRepository;
            this.hostingEnvironment = hostingEnvironment;
        }

        public IActionResult Index()
        {
            IEnumerable<Event> events = _eventRepository.GetAllEvents();
            foreach (var ev in events)
            {
                ev.EventPhotos = eventPhotoRepository.GetAllEventPhotos(ev.Id).ToList();
            }
            return View(events);
        }

        public IActionResult Details(int? id)
        {
            Event event1 = _eventRepository.GetEvent(id ?? 1);

            if (event1 == null)
            {
                Response.StatusCode = 404;
                return View("EventNotFound", id);
            }

            event1.EventPhotos = eventPhotoRepository.GetAllEventPhotos(id ?? 1).ToList();
            return View(event1);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(EventCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                Event newEvent = new Event
                {
                    Name = model.Name,
                    Date = model.Date,
                    Location = model.Location,
                    Description = model.Description
                };

                _eventRepository.AddEvent(newEvent);

                List<EventPhoto> uniqueFileNames = new List<EventPhoto>();
                string uniqueFileName = null;

                if (model.Photos != null && model.Photos.Count > 0)
                {
                    foreach (IFormFile photo in model.Photos)
                    {
                        string uploadsFolder = Path.Combine(hostingEnvironment.WebRootPath, "eventImages");
                        uniqueFileName = Guid.NewGuid().ToString() + "_" + photo.FileName;
                        string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                        photo.CopyTo(new FileStream(filePath, FileMode.Create));
                        EventPhoto newEventPhoto = new EventPhoto
                        {
                            EventId = newEvent.Id,
                            PhotoPath = uniqueFileName
                        };

                        eventPhotoRepository.AddEventPhoto(newEventPhoto);
                    }
                }

                
                return RedirectToAction("details", new { id = newEvent.Id });
            }

            return View();
        }
    }
}