﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using Microsoft.Office.Interop.Word;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RJEEC.Models;
using RJEEC.ViewModels;
using Microsoft.Extensions.Configuration;
using Document = RJEEC.Models.Document;
using MailMessage = System.Net.Mail.MailMessage;

namespace RJEEC.Controllers
{
    public class ArticleController : Controller
    {
        private readonly IArticleRepository articleRepository;
        private readonly IMagazineRepository magazineRepository;
        private readonly IAuthorRepository authorRepository;
        private readonly IHostingEnvironment hostingEnvironment;
        private readonly IConfiguration _config;

        public ArticleController(IArticleRepository articleRepository,
                            IMagazineRepository magazineRepository,
                            IAuthorRepository authorRepository,
                            IHostingEnvironment hostingEnvironment,
                            IConfiguration config)
        {
            this.articleRepository = articleRepository;
            this.magazineRepository = magazineRepository;
            this.authorRepository = authorRepository;
            this.hostingEnvironment = hostingEnvironment;
            this._config = config;
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
            if (id != null)
            {
                Article article = articleRepository.GetArticle(id ?? 1);

                if (article == null)
                {
                    Response.StatusCode = 404;
                    return View("ArticleNotFound", id);
                }
                if (article != null)
                {
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
            }
            return View(null);
            
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
                        Email = model.AuthorEmail,
                        Phone = model.AuthorPhone
                    };
                    authorRepository.AddAuthor(contactAuthor);
                }
                newArticle.contactAuthorId = contactAuthor.Id;

                List<Document> docs = new List<Document>();
                                
                Dictionary<string, DocumentType> uniqueFileNames = new Dictionary<string, DocumentType>();
                uniqueFileNames.Add(ProcessUploadedFile(model.ArticleContentDoc, "articles") ?? String.Empty, DocumentType.ArticleContent);
                if (model.PulishingAgreementDoc != null)
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

                sendArticleByEmail(newArticle);

                return RedirectToAction("details", new { id = newArticle.Id });
            }

            return View();
        }

        public void sendArticleByEmail(Article model)
        {
            string host = _config.GetSection("SMTP").GetSection("Host").Value;
            string rjeecContactMail = _config.GetSection("SMTP").GetSection("From").Value;
            string pass = _config.GetSection("SMTP").GetSection("Password").Value;

            Author author = authorRepository.GetAuthor(model.contactAuthorId);

            MailMessage mailMessage = new MailMessage();
            mailMessage.From = new MailAddress(rjeecContactMail);
            mailMessage.To.Add(rjeecContactMail);
            mailMessage.Subject = "[RJEEC] New Article Submitted By " + author.FirstName + " " + author.LastName;
            mailMessage.Body = String.Format($"<b>Article No.:</b> {model.Id} <br />" +
                $"<b>Title:</b> {model.Title} <br />" +
                $"<b>Authors:</b> {model.Authors} <br />" +
                $"<b>Abstract:</b> {model.Description} <br />" +
                $"<b>Keywords:</b> {model.KeyWords} <br />" +
                $"<b>Correspondent Author:</b> {author.FirstName} {author.LastName} <br />" +
                $"<b>Correspondence Email:</b> {author.Email} <br />" +
                $"<b>Contact Phone:</b> {author.Phone} <br />" +
                $"<b>Comments:</b> {model.Comments} <br />" );
            mailMessage.IsBodyHtml = true;
            mailMessage.Attachments.Add(new Attachment(Path.Combine(hostingEnvironment.WebRootPath, "articles", model.Documents.FirstOrDefault(d=>d.Type==DocumentType.ArticleContent).DocumentPath)));
            if (model.Documents.FirstOrDefault(d => d.Type == DocumentType.PublishingAgreement) != null)
            {
                mailMessage.Attachments.Add(new Attachment(Path.Combine(hostingEnvironment.WebRootPath, "publishingAgreements", model.Documents.FirstOrDefault(d => d.Type == DocumentType.PublishingAgreement).DocumentPath)));
            }
            foreach (var additional in model.Documents.Where(d => d.Type == DocumentType.Additional))
            {
                mailMessage.Attachments.Add(new Attachment(Path.Combine(hostingEnvironment.WebRootPath, "additionalDocuments", additional.DocumentPath)));

            }
            
            using (var smtp = new SmtpClient(host, 587))
            {
                var credential = new NetworkCredential
                {
                    UserName = rjeecContactMail,  // replace with valid value
                    Password = pass  // replace with valid value
                };
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = credential;
                smtp.EnableSsl = true;
                smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                smtp.Send(mailMessage);
            }
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

        [AllowAnonymous]
        public IActionResult DownloadFile(string fileName, string subfolder, string title)
        {
            string downloadFile = Path.Combine(hostingEnvironment.WebRootPath, subfolder, fileName);
            byte[] fileBytes = System.IO.File.ReadAllBytes(downloadFile);
            var fileExt = Path.GetExtension(fileName);
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, title + fileExt);
        }

        [AllowAnonymous]
        public IActionResult GetLast5Magazines()
        {
            IEnumerable<Magazine> magazines = magazineRepository.GetLast5Magazines();
            return PartialView("_GetLast5Magazines", magazines);
        }

        [AllowAnonymous]
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