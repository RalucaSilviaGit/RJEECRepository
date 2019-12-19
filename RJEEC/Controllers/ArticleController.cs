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
    public class ArticleController : Controller
    {
        private readonly IArticleRepository articleRepository;
        private readonly IDocumentRepository documentRepository;
        private readonly IHostingEnvironment hostingEnvironment;

        public ArticleController(IArticleRepository articleRepository,
                            IDocumentRepository documentRepository,
                            IHostingEnvironment hostingEnvironment)
        {
            this.articleRepository = articleRepository;
            this.documentRepository = documentRepository;
            this.hostingEnvironment = hostingEnvironment;
        }

        [AllowAnonymous]
        public IActionResult Index()
        {
            IEnumerable<Article> articles = articleRepository.GetAllArticles().ToList();
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

            ArticleDetailsViewModel articleDetailsViewModel = new ArticleDetailsViewModel
            {
                Id = article.Id,
                Title = article.Title,
                AuthorFirstName = article.contactAuthor != null ? article.contactAuthor.FirstName : String.Empty,
                AuthorLastName = article.contactAuthor != null ? article.contactAuthor.LastName : String.Empty,
                AuthorEmail = article.contactAuthor != null ? article.contactAuthor.Email : String.Empty,
                Status = article.Status,
                Magazine = article.Magazine
            };

            return View(articleDetailsViewModel);
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
                Article newArticle = new Article
                {
                    Title = model.Title,
                    AgreePublishingEthics=model.AgreePublishingEthics,
                    Authors=model.Authors,
                    KeyWords=model.KeyWords,
                    Description = model.Description,
                    contactAuthor = new Author
                    {
                        FirstName = model.AuthorFirstName,
                        LastName = model.AuthorLastName,
                        Email = model.AuthorEmail
                    },
                    Comments = model.Comments                    
                };

                articleRepository.AddArticle(newArticle);

                Dictionary<string, DocumentType> uniqueFileNames = new Dictionary<string, DocumentType>();
                uniqueFileNames.Add(ProcessUploadedFile(model.ArticleContentDoc, "articles") ?? String.Empty, DocumentType.ArticleContent);
                uniqueFileNames.Add(ProcessUploadedFile(model.PulishingAgreementDoc, "publishingAgreements") ?? String.Empty, DocumentType.PublishingAgreement);
                uniqueFileNames.Add(ProcessUploadedFile(model.AdditionalDoc1, "additionalDocuments") ?? String.Empty, DocumentType.Additional);
                uniqueFileNames.Add(ProcessUploadedFile(model.AdditionalDoc2, "additionalDocuments") ?? String.Empty, DocumentType.Additional);

                foreach (var fileName in uniqueFileNames)
                {
                    if (!String.IsNullOrWhiteSpace(fileName.Key))
                    {
                        Document newDocument = new Document
                        {
                            ArticleId = newArticle.Id,
                            Type = fileName.Value,
                            DocumentPath = fileName.Key
                        };
                        documentRepository.AddDocument(newDocument);
                    }
                }

                return RedirectToAction("details", new { id = newArticle.Id });
            }

            return View();
        }

        private string ProcessUploadedFile(IFormFile file, string subfolder)
        {
            string uniqueFileName = null;

            if (file != null)
            {
                string uploadsFolder = Path.Combine(hostingEnvironment.WebRootPath, subfolder);
                uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    file.CopyTo(fileStream);
                }
            }

            return uniqueFileName;
        }
    }
}