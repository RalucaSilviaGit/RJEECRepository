using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using RJEEC.Models;
using RJEEC.ViewModels;

namespace RJEEC.Controllers
{
    public class AuthorController : Controller
    {
        private readonly IAuthorRepository authorRepository;
        private readonly IHostingEnvironment hostingEnvironment;

        public AuthorController(IAuthorRepository authorRepository, IHostingEnvironment hostingEnvironment)
        {
            this.authorRepository = authorRepository;
            this.hostingEnvironment = hostingEnvironment;
        }
        public IActionResult Index()
        {
            return View(authorRepository.GetAllAuthors());
        }

        public IActionResult Details(int? id)
        {
            Author author = authorRepository.GetAuthor(id ?? 1);

            if (author == null)
            {
                Response.StatusCode = 404;
                return View("AuthorNotFound", id);
            }

            return View(author);
        }

        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(AuthorCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                string uniqueFileName = null;
                if (model.Photo != null)
                {
                    string uploadsFolder = Path.Combine(hostingEnvironment.WebRootPath, "authorImages");
                    uniqueFileName = Guid.NewGuid().ToString() + "_" + model.Photo.FileName;
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                    model.Photo.CopyTo(new FileStream(filePath, FileMode.Create));
                }
                Author newAuthor = new Author
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Email = model.Email,
                    PhotoPath = uniqueFileName
                };
                    
                authorRepository.AddAuthor(newAuthor);
                return RedirectToAction("Details", new { id = newAuthor.Id });
            }
            return View();
        }

        [HttpGet]
        public IActionResult Edit(int id)
        {
            Author author = authorRepository.GetAuthor(id);

            if (author == null)
            {
                Response.StatusCode = 404;
                return View("AuthorNotFound", id);
            }

            AuthorEditViewModel authorEditViewModel = new AuthorEditViewModel
            {
                Id = author.Id,
                FirstName = author.FirstName,
                LastName = author.LastName,
                Email = author.Email,
                ExistingPhotoPath = author.PhotoPath
            };
            return View(authorEditViewModel);
        }

        [HttpPost]
        public IActionResult Edit(AuthorEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                Author author = authorRepository.GetAuthor(model.Id);
                author.FirstName = model.FirstName;
                author.LastName = model.LastName;
                author.Email = model.Email;

                if (model.Photo != null)
                {
                    if (model.ExistingPhotoPath != null)
                    {
                        string filePath = Path.Combine(hostingEnvironment.WebRootPath,
                            "authorImages", model.ExistingPhotoPath);
                        System.IO.File.Delete(filePath);
                    }
                    author.PhotoPath = ProcessUploadedFile(model);
                }

                // Call update method on the repository service passing it the
                // employee object to update the data in the database table
                Author updatedAuthor = authorRepository.Update(author);

                return RedirectToAction("index");
            }

            return View(model);
        }

        private string ProcessUploadedFile(AuthorCreateViewModel model)
        {
            string uniqueFileName = null;

            if (model.Photo != null)
            {
                string uploadsFolder = Path.Combine(hostingEnvironment.WebRootPath, "authorImages");
                uniqueFileName = Guid.NewGuid().ToString() + "_" + model.Photo.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    model.Photo.CopyTo(fileStream);
                }
            }

            return uniqueFileName;
        }
    }
}