﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using RJEEC.Models;
using RJEEC.ViewModels;

namespace RJEEC.Controllers
{
    public class AuthorController : Controller
    {
        private readonly IAuthorRepository authorRepository;
        private readonly IWebHostEnvironment hostingEnvironment;

        public AuthorController(IAuthorRepository authorRepository, IWebHostEnvironment hostingEnvironment)
        {
            this.authorRepository = authorRepository;
            this.hostingEnvironment = hostingEnvironment;
        }

        [Authorize(Roles = "Admin, SuperAdmin")]
        public IActionResult Index()
        {
            return View(authorRepository.GetAllAuthors());
        }

        [Authorize(Roles = "Admin, SuperAdmin")]
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

        [AcceptVerbs("Get", "Post")]
        [AllowAnonymous]
        public IActionResult IsEmailInUse(string email)
        {
            var user = authorRepository.GetAllAuthors().FirstOrDefault(a=>a.Email==email);

            if (user == null)
            {
                return Json(true);
            }
            else
            {
                return Json($"Email {email} is already in use.");
            }
        }

        [HttpGet]
        [Authorize(Roles = "Admin, SuperAdmin")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin, SuperAdmin")]
        public IActionResult Create(AuthorCreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                string uniqueFileName = ProcessUploadedFile(model);
                Author newAuthor = new Author
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Email = model.Email,
                    Phone = model.Phone,
                    OrcidId = model.OrcidId,
                    ResearcherId = model.ResearcherId,
                    PhotoPath = uniqueFileName
                };
                    
                authorRepository.AddAuthor(newAuthor);
                return RedirectToAction("Details", new { id = newAuthor.Id });
            }
            return View();
        }

        [HttpGet]
        [Authorize(Roles = "Admin, SuperAdmin")]
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
                Phone = author.Phone,
                OrcidId = author.OrcidId,
                ResearcherId = author.ResearcherId,
                ExistingPhotoPath = author.PhotoPath
            };
            return View(authorEditViewModel);
        }

        [HttpPost]
        [Authorize(Roles = "Admin, SuperAdmin")]
        public IActionResult Edit(AuthorEditViewModel model)
        {
            if (ModelState.IsValid)
            {
                Author author = authorRepository.GetAuthor(model.Id);
                author.FirstName = model.FirstName;
                author.LastName = model.LastName;
                author.Email = model.Email;
                author.Phone = model.Phone;
                author.OrcidId = model.OrcidId;
                author.ResearcherId = model.ResearcherId;

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