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

        public IActionResult Details()
        {
            Author author = authorRepository.GetAuthor(1);
            return View(author);
        }
    }
}