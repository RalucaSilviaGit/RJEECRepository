using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RJEEC.Models;

namespace RJEEC.Controllers
{
    public class ArticleController : Controller
    {
        private readonly IArticleRepository articleRepository;
        private readonly IDocumentRepository documentRepository;

        public ArticleController(IArticleRepository articleRepository,
                            IDocumentRepository documentRepository)
        {
            this.articleRepository = articleRepository;
            this.documentRepository = documentRepository;
        }

        [AllowAnonymous]
        public IActionResult Index()
        {
            IEnumerable<Article> articles = articleRepository.GetAllArticles();
            foreach (var article in articles)
            {
                article.Documents = documentRepository.GetAllDocumentsForArticle(article.Id).ToList();
            }
            return View(articles);
        }

        [AllowAnonymous]
        public IActionResult Details(int? id)
        {
            Article article = articleRepository.GetArticle(id ?? 1);

            if (article == null)
            {
                Response.StatusCode = 404;
                return View("ArticleNotFound", id);
            }

            article.Documents = documentRepository.GetAllDocumentsForArticle(id ?? 1).ToList();
            return View(article);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public IActionResult Create(ArticleCreateViewModel model)
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