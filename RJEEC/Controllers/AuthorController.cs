using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using RJEEC.Models;
using RJEEC.ViewModels;

namespace RJEEC.Controllers
{
    public class AuthorController : Controller
    {
        private readonly IAuthorRepository authorRepository;

        public AuthorController(IAuthorRepository authorRepository)
        {
            this.authorRepository = authorRepository;
        }
        public IActionResult Index()
        {
            return View(authorRepository.GetAllAuthors());
        }

        public IActionResult Details(int? id)
        {
            Author author = authorRepository.GetAuthor(id ?? 1);
            return View(author);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Author author)
        {
            if (ModelState.IsValid)
            {
                Author newAuthor = authorRepository.AddAuthor(author);
                return RedirectToAction("Details","Author", new { id = author.Id });
            }
            return View();
        }
    }
}