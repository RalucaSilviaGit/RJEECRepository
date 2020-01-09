using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Office.Interop.Word;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RJEEC.Models;
using RJEEC.ViewModels;
using TheArtOfDev.HtmlRenderer.PdfSharp;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using Document = RJEEC.Models.Document;

namespace RJEEC.Controllers
{
    public class ArticleController : Controller
    {
        private readonly IArticleRepository articleRepository;
        private readonly IMagazineRepository magazineRepository;
        private readonly IAuthorRepository authorRepository;
        private readonly IHostingEnvironment hostingEnvironment;


        public ArticleController(IArticleRepository articleRepository,
                            IMagazineRepository magazineRepository,
                            IAuthorRepository authorRepository,
                            IHostingEnvironment hostingEnvironment)
        {
            this.articleRepository = articleRepository;
            this.magazineRepository = magazineRepository;
            this.authorRepository = authorRepository;
            this.hostingEnvironment = hostingEnvironment;
        }

        [AllowAnonymous]
        public IActionResult Index()
        {
            IEnumerable<Article> articles = articleRepository.GetAllArticles().ToList();
            return View(articles);
        }

        [AllowAnonymous]
        public IActionResult GetAllArticlesByMagazineId(int magazineId)
        {
            IEnumerable<Article> articles = articleRepository.GetAllArticlesByMagazine(magazineId).ToList();
            return View("Index", articles);
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult Search()
        {
            SearchViewModel model = new SearchViewModel();
            return View(model);
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult Search(SearchViewModel model)
        {
            IEnumerable<Article> articles = null;
            if (!String.IsNullOrWhiteSpace(model.AuthorName))
                articles = articleRepository.GetAllArticles().Where(a => a.Authors.ToUpper().Contains(model.AuthorName.ToUpper()));
            if (model.Volume != null)
                if(articles == null)
                    articles = articleRepository.GetAllArticles().Where(a => a.Magazine?.Volume == model.Volume);
                else
                    articles = articles.Where(a => a.Magazine.Volume == model.Volume);
            if (model.Year != null)
                if (articles == null)
                    articles = articleRepository.GetAllArticles().Where(a => a.Magazine?.PublishingYear == model.Year);
                else
                    articles = articles.Where(a => a.Magazine.PublishingYear == model.Year);
            if (!String.IsNullOrWhiteSpace(model.Keywords))
                if(articles == null)
                    articles = articleRepository.GetAllArticles().Where(a => a.KeyWords.ToUpper().Contains(model.Keywords.ToUpper()));
                else
                    articles = articles.Where(a => a.KeyWords.ToUpper().Contains(model.Keywords.ToUpper()));
            if (articles != null)
            {
                model.Articles = articles.ToList();
            }
            return View(model);
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
                Content = article.Documents.FirstOrDefault(d => d.Type == DocumentType.ArticleContent).DocumentPath
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

                List<Document> docs = new List<Document>();
                                
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
                        RJEEC.Models.Document newDocument = new RJEEC.Models.Document
                        {
                            Type = fileName.Value,
                            DocumentPath = fileName.Key
                        };
                        docs.Add(newDocument);
                    }
                }

                newArticle.Documents = docs;

                articleRepository.AddArticle(newArticle);

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
                MagazineVolume = article.Magazine?.Volume,
                MagazineNumber = article.Magazine?.Number,
                MagazinePublishingYear = article.Magazine?.PublishingYear
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
                article.Magazine = magazineRepository.GetMagazineByVolumeNumberYear(model.MagazineVolume, model.MagazineNumber, model.MagazinePublishingYear);
                article.MagazineId = magazineRepository.GetMagazineByVolumeNumberYear(model.MagazineVolume, model.MagazineNumber, model.MagazinePublishingYear)?.Id;
                if (article.Magazine == null && model.MagazineVolume != null && model.MagazineNumber != null && model.MagazinePublishingYear != null)
                {
                    Magazine magazine = new Magazine {
                        Volume = model.MagazineVolume ?? 0,
                        Number = model.MagazineNumber ?? 0,
                        PublishingYear = model.MagazinePublishingYear?? 0
                    };
                    magazineRepository.AddMagazine(magazine);
                    article.MagazineId = magazine.Id;
                    article.Magazine = magazine;
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

        //public IActionResult DownloadAbstract(int id)
        //{
        //    Article art = articleRepository.GetArticle(id);
        //    string html = "<html><body style='font-size:20px'>" + art.Description + "</body></html>";

        //    Byte[] res = null;
        //    PdfDocument d = PdfReader.Open(new MemoryStream(html, 0, html.Length));
        //    res = ms.ToArray();

        //    return File(res, System.Net.Mime.MediaTypeNames.Application.Octet, art.Title + ".pdf");
        //}

        [AllowAnonymous]
        public IActionResult GetLast5Magazines()
        {
            IEnumerable<Magazine> magazines = magazineRepository.GetLast5Magazines();
            return PartialView("_GetLast5Magazines", magazines);
        }

        public IActionResult PreviewFile(string fileName, string subfolder)
        {
            string downloadFile = Path.Combine(hostingEnvironment.WebRootPath, subfolder, fileName);
            var fileExt = Path.GetExtension(fileName);
            switch (fileExt)
            {
                case ".pdf":
                    var stream = new FileStream(downloadFile, FileMode.Open);
                    return new FileStreamResult(stream, "application/pdf");
                case ".doc":
                case ".docx":
                    string newFileName = Path.GetFileNameWithoutExtension(downloadFile) + "ToPdf.pdf";
                    Application word = new Application();
                    Microsoft.Office.Interop.Word.Document doc = word.Documents.Open(downloadFile);
                    doc.Activate();
                    downloadFile = Path.Combine(hostingEnvironment.WebRootPath, subfolder, newFileName);
                    doc.SaveAs2(downloadFile, WdSaveFormat.wdFormatPDF);
                    doc.Close();
                    goto case ".pdf";
                default:
                    return null;
            }
        }
    }
}