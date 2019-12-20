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
        private readonly IMagazineRepository magazineRepository;
        private readonly IAuthorRepository authorRepository;
        private readonly IHostingEnvironment hostingEnvironment;


        public ArticleController(IArticleRepository articleRepository,
                            IDocumentRepository documentRepository,
                            IMagazineRepository magazineRepository,
                            IAuthorRepository authorRepository,
                            IHostingEnvironment hostingEnvironment)
        {
            this.articleRepository = articleRepository;
            this.documentRepository = documentRepository;
            this.magazineRepository = magazineRepository;
            this.authorRepository = authorRepository;
            this.hostingEnvironment = hostingEnvironment;
        }

        [AllowAnonymous]
        public IActionResult Index()
        {
            IEnumerable<Article> articles = articleRepository.GetAllArticles().ToList();
            foreach (var article in articles)
            {
                article.Documents = documentRepository.GetAllDocumentsForArticle(article.Id).ToList();
            }
            return View(articles);
        }

        [AllowAnonymous]        
        public IActionResult Read(int? id)
        {
            Article article = articleRepository.GetArticle(id ?? 1);

            if (article == null)
            {
                Response.StatusCode = 404;
                return View("ArticleNotFound", id);
            }

            ArticleReadViewModel articleReadViewModel = new ArticleReadViewModel
            {
                Title = article.Title,
                Description = article.Description,
                Authors = article.Authors,
                KeyWords = article.KeyWords,
                Content = documentRepository.GetDocumentByArticleAndType(article.Id, DocumentType.ArticleContent).DocumentPath
            };

            return View(articleReadViewModel);
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

            Magazine magazine = magazineRepository.GetMagazine(article.MagazineId ?? 0);

            ArticleDetailsViewModel articleDetailsViewModel = new ArticleDetailsViewModel
            {
                Id = article.Id,
                Title = article.Title,
                AuthorFirstName = authorRepository.GetAuthor(article.contactAuthorId)?.FirstName,
                AuthorLastName = authorRepository.GetAuthor(article.contactAuthorId)?.LastName,
                AuthorEmail = authorRepository.GetAuthor(article.contactAuthorId)?.Email,
                Status = article.Status,
                MagazineVolume = magazine?.Volume,
                MagazineNumber = magazine?.Number,
                MagazinePublishingYear = magazine?.PublishingYear
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
                    Status = ArticleStatus.New,
                    Comments = model.Comments                    
                };

                Author contactAuthor = authorRepository.GetAuthorByEmail(model.AuthorEmail);
                if(contactAuthor == null && model.AuthorEmail != null)
                {
                    contactAuthor = new Author
                    {
                        FirstName = model.AuthorFirstName,
                        LastName = model.AuthorLastName,
                        Email = model.AuthorEmail
                    };
                    authorRepository.AddAuthor(contactAuthor);
                }
                newArticle.contactAuthorId = contactAuthor.Id;
                articleRepository.AddArticle(newArticle);

                Dictionary<string, DocumentType> uniqueFileNames = new Dictionary<string, DocumentType>();
                uniqueFileNames.Add(ProcessUploadedFile(model.ArticleContentDoc, "articles") ?? String.Empty, DocumentType.ArticleContent);
                uniqueFileNames.Add(ProcessUploadedFile(model.PulishingAgreementDoc, "publishingAgreements") ?? String.Empty, DocumentType.PublishingAgreement);
                if(model.AdditionalDoc1 != null)
                    uniqueFileNames.Add(ProcessUploadedFile(model.AdditionalDoc1, "additionalDocuments") ?? String.Empty, DocumentType.Additional);
                if (model.AdditionalDoc2 != null)
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

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Review(int? id)
        {
            Article article = articleRepository.GetArticle(id ?? 1);

            if (article == null)
            {
                Response.StatusCode = 404;
                return View("ArticleNotFound", id);
            }

            Magazine magazine = magazineRepository.GetMagazine(article.MagazineId ?? 0);

            ArticleDetailsViewModel articleDetailsViewModel = new ArticleDetailsViewModel
            {
                Id = article.Id,
                Title = article.Title,
                AuthorFirstName = authorRepository.GetAuthor(article.contactAuthorId)?.FirstName,
                AuthorLastName = authorRepository.GetAuthor(article.contactAuthorId)?.LastName,
                AuthorEmail = authorRepository.GetAuthor(article.contactAuthorId)?.Email,
                Status = article.Status,
                MagazineVolume = magazine?.Volume,
                MagazineNumber = magazine?.Number,
                MagazinePublishingYear = magazine?.PublishingYear
            };

            return View(articleDetailsViewModel);
        }
        [HttpPost]
        [AllowAnonymous]
        public IActionResult Review(ArticleDetailsViewModel model)
        {
            if (ModelState.IsValid)
            {
                Article article = articleRepository.GetArticle(model.Id);
                article.Status = model.Status;
                article.MagazineId = magazineRepository.GetMagazineByVolumeNumberYear(model.MagazineVolume, model.MagazineNumber, model.MagazinePublishingYear)?.Id;
                if(article.MagazineId == null && model.MagazineVolume != null && model.MagazineNumber != null && model.MagazinePublishingYear != null)
                {
                    Magazine magazine = new Magazine {
                        Volume = model.MagazineVolume ?? 0,
                        Number = model.MagazineNumber ?? 0,
                        PublishingYear = model.MagazinePublishingYear?? 0
                    };
                    magazineRepository.AddMagazine(magazine);
                    article.MagazineId = magazine.Id; 
                }

                Article updatedArticle = articleRepository.Update(article);

                return RedirectToAction("details", new { id = article.Id});
            }

            return View(model);
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

        public IActionResult DownloadFile(string fileName, string subfolder, string title)
        {
            string downloadFile = Path.Combine(hostingEnvironment.WebRootPath, subfolder, fileName);
            byte[] fileBytes = System.IO.File.ReadAllBytes(downloadFile);
            var fileExt = Path.GetExtension(fileName);
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, title + fileExt);
        }

        public IActionResult PreviewFile(string fileName, string subfolder)
        {
            string downloadFile = Path.Combine(hostingEnvironment.WebRootPath, subfolder, fileName);
            var stream = new FileStream(downloadFile, FileMode.Open);            
            var fileExt = Path.GetExtension(fileName);
            switch (fileExt)
            {
                case ".pdf":
                    return new FileStreamResult(stream, "application/pdf");
                case ".doc":
                    return new FileStreamResult(stream, "application/msword");
                case ".docx":
                    return new FileStreamResult(stream, "application/vnd.openxmlformats-officedocument.wordprocessingml.document");
                default:
                    return null;
            }
        }
    }
}