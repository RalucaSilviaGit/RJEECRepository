using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
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
        private readonly IHostingEnvironment hostingEnvironment;

        public EventController(IEventRepository eventRepository, 
                            IHostingEnvironment hostingEnvironment)
        {
            this._eventRepository = eventRepository;
            this.hostingEnvironment = hostingEnvironment;
        }
        [AllowAnonymous]
        public IActionResult Index()
        {
            IEnumerable<Event> events = _eventRepository.GetAllEvents();
            return View(events);
        }

        [AllowAnonymous]
        public IActionResult GetNext5Events()
        {
            IEnumerable<Event> events = _eventRepository.GetNext5Events();
            return PartialView("_GetNext5Events", events);
        }

        [AllowAnonymous]
        public IActionResult Details(int? id)
        {
            Event event1 = _eventRepository.GetEvent(id ?? 1);

            if (event1 == null)
            {
                Response.StatusCode = 404;
                return View("EventNotFound", id);
            }

            return View(event1);
        }

        [HttpGet]
        [Authorize(Roles="Admin, SuperAdmin")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin, SuperAdmin")]
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
                
                if (model.Photos != null && model.Photos.Count > 0)
                {
                    foreach (IFormFile photo in model.Photos)
                    {
                        ProcessUploadedImage(newEvent, photo);
                    }
                }

                _eventRepository.AddEvent(newEvent);

                return RedirectToAction("details", new { id = newEvent.Id });
            }

            return View();
        }

        private void ProcessUploadedImage(Event newEvent, IFormFile photo)
        {
            string uploadsFolder = Path.Combine(hostingEnvironment.WebRootPath, "eventImages");
            string uniqueFileName = Guid.NewGuid().ToString() + "_" + photo.FileName;
            string filePath = Path.Combine(uploadsFolder, uniqueFileName);
            photo.CopyTo(new FileStream(filePath, FileMode.Create));
            EventPhoto newEventPhoto = new EventPhoto
            {
                EventId = newEvent.Id,
                PhotoPath = uniqueFileName
            };

            newEvent.EventPhotos.Add(newEventPhoto);
        }

        private void ProcessDeletedImage(EventPhoto eventPhoto)
        {
            string filePath = Path.Combine(hostingEnvironment.WebRootPath, "eventImages", eventPhoto.PhotoPath);
            if ((System.IO.File.Exists(filePath)))
            {
                System.IO.File.Delete(filePath);
            }
        }

        [HttpGet]
        [Authorize(Roles="Admin, SuperAdmin")]
        public IActionResult Edit(int id)
        {
            Event ev = _eventRepository.GetEvent(id);

            if (ev == null)
            {
                Response.StatusCode = 404;
                return View("EventNotFound", id);
            }

            EventEditViewModel eventEditViewModel = new EventEditViewModel
            {
                Id = ev.Id,
                Name = ev.Name,
                Location = ev.Location,
                Date = ev.Date,
                Description = ev.Description
            };
            return View(eventEditViewModel);
        }

        [HttpPost]
        [Authorize(Roles = "Admin, SuperAdmin")]
        public IActionResult Edit(EventEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                Event ev = _eventRepository.GetEvent(model.Id);
                ev.Name = model.Name;
                ev.Location = model.Location;
                ev.Date = model.Date;
                ev.Description = model.Description;

                Event updatedEvent = _eventRepository.Update(ev);

                return RedirectToAction("index");
            }

            return View(model);
        }

        [HttpGet]
        [Authorize(Roles = "Admin, SuperAdmin")]
        public IActionResult EditEventPhotos(int id)
        {
            Event ev = _eventRepository.GetEvent(id);

            if (ev == null)
            {
                Response.StatusCode = 404;
                return View("EventNotFound", id);
            }
            EventPhotosEditViewModel eventPhotosEditViewModel = new EventPhotosEditViewModel
            {
                EventId = ev.Id,
                EventPhotos = ev.EventPhotos
            };

            return View(eventPhotosEditViewModel);
        }

        [HttpPost]
        [Authorize(Roles = "Admin, SuperAdmin")]
        public IActionResult Delete(int id)
        {
            var ev = _eventRepository.GetEvent(id);

            if (ev == null)
            {
                Response.StatusCode = 404;
                return View("EventNotFound", id);
            }
            else
            {
                while (ev.EventPhotos != null && ev.EventPhotos.Count > 0)
                {
                    EventPhoto photo = ev.EventPhotos[0];
                    DeletePhoto(photo.EventId, photo.Id);
                }
                _eventRepository.Delete(id);                
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin, SuperAdmin")]
        public IActionResult DeletePhoto(int eventId, int id)
        {
            var ev = _eventRepository.GetEvent(eventId);
            var evPhoto = ev.EventPhotos.FirstOrDefault(ep => ep.Id == id);

            if (evPhoto == null)
            {
                Response.StatusCode = 404;
                return View("NotFound", id);
            }
            else
            {
                ev.EventPhotos.Remove(evPhoto);
                ProcessDeletedImage(evPhoto);
                Event updatedEvent = _eventRepository.Update(ev);
                return RedirectToAction("EditEventPhotos", new { id = ev.Id });
            }
        }

        [HttpPost]
        [Authorize(Roles = "Admin, SuperAdmin")]
        public IActionResult EditEventPhotos(EventPhotosEditViewModel model)
        {
            var ev = _eventRepository.GetEvent(model.EventId);
            if (ev == null)
            {
                Response.StatusCode = 404;
                return View("EventNotFound", model.EventId);
            }
            else
            {
                List<EventPhoto> uniqueFileNames = new List<EventPhoto>();

                if (model.NewPhotos != null && model.NewPhotos.Count > 0)
                {
                    foreach (IFormFile photo in model.NewPhotos)
                    {
                        ProcessUploadedImage(ev, photo);
                    }
                }
                Event updatedEvent = _eventRepository.Update(ev);
                return RedirectToAction("EditEventPhotos", new { id = ev.Id });
            }
        }
    }
}