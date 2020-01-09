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

                        uniqueFileNames.Add(newEventPhoto);
                    }
                }
                newEvent.EventPhotos = uniqueFileNames;

                _eventRepository.AddEvent(newEvent);

                return RedirectToAction("details", new { id = newEvent.Id });
            }

            return View();
        }
    }
}