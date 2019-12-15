using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RJEEC.Models;

namespace RJEEC.Controllers
{
    public class EventController : Controller
    {
        private readonly IEventRepository _eventRepository;

        public EventController(IEventRepository eventRepository)
        {
            _eventRepository = eventRepository;
        }

        public IActionResult Index()
        {
            return View(_eventRepository.GetAllEvents());
        }

        public IActionResult Details(int? id)
        {
            Event event1 = _eventRepository.GetEvent(id ?? 1);
            return View(event1);
        }

        public IActionResult Create()
        {
            return View();
        }
    }
}