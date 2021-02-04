using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using RJEEC.Models;
using RJEEC.ViewModels;
using Microsoft.Extensions.Configuration;
using Document = RJEEC.Models.Document;
using MailMessage = System.Net.Mail.MailMessage;
using Microsoft.AspNetCore.Identity;
using System.Runtime.InteropServices.WindowsRuntime;

namespace RJEEC.Controllers
{
    public class ArticleController : Controller
    {
        private readonly IArticleRepository articleRepository;
        private readonly IMagazineRepository magazineRepository;
        private readonly IAuthorRepository authorRepository;
        private readonly IDocumentRepository documentRepository;
        private readonly IWebHostEnvironment hostingEnvironment;
        private readonly IConfiguration _config;
        private readonly UserManager<IdentityUser> userManager;

        public ArticleController(IArticleRepository articleRepository,
                            IMagazineRepository magazineRepository,
                            IAuthorRepository authorRepository,
                            IDocumentRepository documentRepository,
                            IWebHostEnvironment hostingEnvironment,
                            IConfiguration config,
                            UserManager<IdentityUser> userManager)
        {
            this.articleRepository = articleRepository;
            this.magazineRepository = magazineRepository;
            this.authorRepository = authorRepository;
            this.documentRepository = documentRepository;
            this.hostingEnvironment = hostingEnvironment;
            this._config = config;
            this.userManager = userManager;
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
            IEnumerable<Article> articles = articleRepository.GetAllArticlesByMagazine(magazineId).OrderBy(a => a.Order);
            GetArticlesByMagazineViewModel model = new GetArticlesByMagazineViewModel
            {
                Articles = articles.ToList(),
                ExistingCoverPath = magazineRepository.GetMagazine(magazineId).CoverPath,
                ExistingBackCoverPath = magazineRepository.GetMagazine(magazineId).BackCoverPath
            };
            return View("GetAllArticlesByMagazine", model);
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
                articles = articleRepository.GetAllArticles().Where(a => a.Authors != null && a.Authors.ToUpper().Contains(model.AuthorName.ToUpper()));
            if (model.Volume != null)
                if (articles == null)
                    articles = articleRepository.GetAllArticles().Where(a => a.Magazine?.Volume == model.Volume);
                else
                    articles = articles.Where(a => a.Magazine?.Volume == model.Volume);
            if (model.Year != null)
                if (articles == null)
                    articles = articleRepository.GetAllArticles().Where(a => a.Magazine?.PublishingYear == model.Year);
                else
                    articles = articles.Where(a => a.Magazine.PublishingYear == model.Year);
            if (!String.IsNullOrWhiteSpace(model.Keywords))
                if (articles == null)
                    articles = articleRepository.GetAllArticles().Where(a=> a.KeyWords != null && a.KeyWords.ToUpper().Contains(model.Keywords.ToUpper()));
                else
                    articles = articles.Where(a => a.KeyWords != null && a.KeyWords.ToUpper().Contains(model.Keywords.ToUpper()));
            if (articles != null && articles.Count() > 0)
            {
                model.Articles = articles.Where(a=>a.Status == ArticleStatus.Published).ToList();
            }
            return View(model);
        }

        [AllowAnonymous]
        [Route("Read/{articleId}")]
        public IActionResult Read(int? articleId, string Id)
        {
            Article article = articleRepository.GetArticle(articleId ?? 1);

            if (article == null)
            {
                Response.StatusCode = 404;
                return View("ArticleNotFound", articleId);
            }

            ArticleReadViewModel articleReadViewModel = new ArticleReadViewModel
            {
                Title = article.Title,
                Description = article.Description,
                Authors = article.Authors,
                KeyWords = article.KeyWords,
                Content = article.Documents.LastOrDefault(d => d.Type == DocumentType.ArticleContent).DocumentPath,
                Doc = PreviewFile(article.Documents.LastOrDefault(d => d.Type == DocumentType.ArticleContent).DocumentPath, "articles"),
                DOI = article.DOI
            };

            return View(articleReadViewModel);
        }

        [HttpGet]
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
                Description = article.Description,
                Authors = article.Authors,
                AuthorFirstName = authorRepository.GetAuthor(article.contactAuthorId)?.FirstName,
                AuthorLastName = authorRepository.GetAuthor(article.contactAuthorId)?.LastName,
                AuthorEmail = authorRepository.GetAuthor(article.contactAuthorId)?.Email,
                Status = article.Status,
                MagazineVolume = magazine?.Volume,
                MagazineNumber = magazine?.Number,
                MagazinePublishingYear = magazine?.PublishingYear,
                ExistingReviewerDecisionFileName = article.Documents
                    .Where(d => d.Type == DocumentType.ReviewerDecision)
                    .GroupBy(d => d.UserName)
                    .Select(d => d.LastOrDefault()?.DocumentPath)
                    .ToList(),
                //LastOrDefault(d => d.Type == DocumentType.ReviewerDecision)?.DocumentPath,
                DocumentsForArticle = article.Documents.OrderBy(d => d.Id).ToList()
            };

            return View(articleDetailsViewModel);
        }

        [HttpPost]
        public IActionResult Details(ArticleDetailsViewModel model)
        {
            if (ModelState.IsValid && (model.Status != ArticleStatus.Published || User.IsInRole("Admin") || User.IsInRole("SuperAdmin") || User.IsInRole("Editor")))
            {
                if (model.AdditionalDoc1 != null || model.ArticleContentNewDoc != null || model.AdditionalDoc2 != null)
                {
                    if (model.ArticleContentNewDoc != null)
                    {
                        string fileName = ProcessUploadedFile(model.ArticleContentNewDoc, "articles");
                        if (!String.IsNullOrWhiteSpace(fileName))
                        {
                            Document newArticleContent = new Document
                            {
                                ArticleId = model.Id,
                                Type = DocumentType.ArticleContent,
                                DocumentPath = fileName
                            };
                            documentRepository.AddDocument(newArticleContent);
                        }
                    }

                    if (model.AdditionalDoc1 != null)
                    {
                        string fileName = ProcessUploadedFile(model.AdditionalDoc1, "additionalDocuments");
                        if (!String.IsNullOrWhiteSpace(fileName))
                        {
                            Document newAdditional = new Document
                            {
                                ArticleId = model.Id,
                                Type = DocumentType.Additional,
                                DocumentPath = fileName
                            };
                            documentRepository.AddDocument(newAdditional);
                        }
                    }

                    if (model.AdditionalDoc2 != null)
                    {
                        string fileName = ProcessUploadedFile(model.AdditionalDoc2, "additionalDocuments");
                        if (!String.IsNullOrWhiteSpace(fileName))
                        {
                            Document newAdditional = new Document
                            {
                                ArticleId = model.Id,
                                Type = DocumentType.Additional,
                                DocumentPath = fileName
                            };
                            documentRepository.AddDocument(newAdditional);
                        }
                    }

                    sendArticleUpdatedByByEmail(articleRepository.GetArticle(model.Id));
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "No new documents have been added. Please choose the documents you want to submit!");
                    return View(model);
                }
                return RedirectToAction("details", new { id = model.Id });
            }

            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Create()
        {
            if(User.Identity.IsAuthenticated == false)
            {
                return RedirectToAction("AccessDenied", "Account");
            }
            Author author = authorRepository.GetAuthorByEmail(User?.Identity?.Name);
            ArticleCreateViewModel model = new ArticleCreateViewModel
            {
                AuthorEmail = User?.Identity?.Name,
                AuthorFirstName = author?.FirstName,
                AuthorLastName = author?.LastName,
                AuthorPhone = author?.Phone
            };
            return View(model);
        }

        [HttpPost]
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
                } else if (model.AuthorEmail != null) {
                    Author auth = authorRepository.GetAuthorByEmail(model.AuthorEmail);
                    auth.FirstName = model.AuthorFirstName;
                    auth.LastName = model.AuthorLastName;
                    auth.Phone = model.AuthorPhone;
                    contactAuthor = authorRepository.Update(auth);
                }
                newArticle.contactAuthorId = contactAuthor.Id;
                newArticle.contactAuthor = contactAuthor;

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

        public void sendStatusChangeByByEmail(Article model)
        {
            string host = _config.GetSection("SMTP").GetSection("Host").Value;
            string rjeecContactMail = _config.GetSection("SMTP").GetSection("From").Value;
            string pass = _config.GetSection("SMTP").GetSection("Password").Value;

            Author author = authorRepository.GetAuthor(model.contactAuthorId);

            MailMessage mailMessage = new MailMessage();
            mailMessage.From = new MailAddress(rjeecContactMail);
            mailMessage.To.Add(author.Email);            
            mailMessage.To.Add(rjeecContactMail);

            mailMessage.Subject = "[RJEEC] Your article " + model.Title + " was reviewed.";
            mailMessage.Body = String.Format($"<b>Article No.:</b> {model.Id} <br />" +
                $"<b>Title:</b> {model.Title} <br />" +
                $"<b>Authors:</b> {model.Authors} <br />" +
                $"<b>Abstract:</b> {model.Description} <br />" +
                $"<b>Keywords:</b> {model.KeyWords} <br />" +
                $"<b>Review result:</b> {model.Status} <br />" );
            if(model.Status == ArticleStatus.Accepted)
            {
                mailMessage.Body += String.Format($"<b>Will be published in volume:</b> {model?.Magazine?.Volume}, " +
                $"<b>No:</b> {model?.Magazine?.Number}, " +
                $"<b>Year:</b> {model?.Magazine?.PublishingYear} <br />");
            }

            mailMessage.Body += String.Format($"You can follow the status of the article or update additional documents by logging into your account on www.rjeec.ro.<br />" +
                $"If you don't remember your password, please reset it by using the password reset option.<br />");

            mailMessage.IsBodyHtml = true;
            if (model.Documents.LastOrDefault(d => d.Type == DocumentType.ReviewerDecision) != null)
            {
                mailMessage.Attachments.Add(new Attachment(Path.Combine(hostingEnvironment.WebRootPath, "reviewerDecision", model.Documents.LastOrDefault(d => d.Type == DocumentType.ReviewerDecision).DocumentPath)));

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

        public void sendArticleUpdatedByByEmail(Article model)
        {
            string host = _config.GetSection("SMTP").GetSection("Host").Value;
            string rjeecContactMail = _config.GetSection("SMTP").GetSection("From").Value;
            string pass = _config.GetSection("SMTP").GetSection("Password").Value;

            Author author = authorRepository.GetAuthor(model.contactAuthorId);

            MailMessage mailMessage = new MailMessage();
            mailMessage.From = new MailAddress(rjeecContactMail);
            mailMessage.To.Add(author.Email);
            mailMessage.To.Add(rjeecContactMail);

            mailMessage.Subject = "[RJEEC] Article " + model.Title + " was updated.";
            mailMessage.Body = String.Format($"<b>Article No.:</b> {model.Id} <br />" +
                $"<b>Title:</b> {model.Title} <br />" +
                $"<b>Authors:</b> {model.Authors} <br />" +
                $"<b>Abstract:</b> {model.Description} <br />" +
                $"<b>Keywords:</b> {model.KeyWords} <br />" +
                $"<b>Review result:</b> {model.Status} <br />");
            if (model.Status == ArticleStatus.Accepted)
            {
                mailMessage.Body += String.Format($"<b>Will be published in volume:</b> {model.Magazine.Volume}, " +
                $"<b>No:</b> {model.Magazine.Number}, " +
                $"<b>Year:</b> {model.Magazine.PublishingYear} <br />");
            }

            mailMessage.Body += String.Format($"<b>We confirm that the article was updated.</b><br /><br />  " +
                $"You can follow the status of your article or update additional documents by logging into your account on www.rjeec.ro.<br />" +
                $"If you don't remember your password, please reset it by using the password reset option.<br />");

            mailMessage.IsBodyHtml = true;

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

        public void sendArticleByEmail(Article model)
        {
            string host = _config.GetSection("SMTP").GetSection("Host").Value;
            string rjeecContactMail = _config.GetSection("SMTP").GetSection("From").Value;
            string pass = _config.GetSection("SMTP").GetSection("Password").Value;

            Author author = authorRepository.GetAuthor(model.contactAuthorId);

            MailMessage mailMessage = new MailMessage();
            mailMessage.From = new MailAddress(rjeecContactMail);
            mailMessage.To.Add(rjeecContactMail);
            mailMessage.CC.Add(author.Email);
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

            mailMessage.Body += String.Format($"<b>We confirm that your article has been registered in our database.</b><br /><br />  " +
                $"You can follow the status of your article or update additional documents by logging into your account on www.rjeec.ro. <br />" +
                $"If you don't remember your password, please reset it by using the password reset option.<br />");

            mailMessage.IsBodyHtml = true;
            mailMessage.Attachments.Add(new Attachment(Path.Combine(hostingEnvironment.WebRootPath, "articles", model.Documents.LastOrDefault(d=>d.Type==DocumentType.ArticleContent).DocumentPath)));
            if (model.Documents.LastOrDefault(d => d.Type == DocumentType.PublishingAgreement) != null)
            {
                mailMessage.Attachments.Add(new Attachment(Path.Combine(hostingEnvironment.WebRootPath, "publishingAgreements", model.Documents.LastOrDefault(d => d.Type == DocumentType.PublishingAgreement).DocumentPath)));
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

        public IActionResult GetArticlesInStatus(int? id)
        {            
            ArticlesInStatusViewModel model = new ArticlesInStatusViewModel();

            if (id != null)
            {
                model.StatusId = id;
                if (User.IsInRole("Researcher"))
                    model.Articles = articleRepository.GetAllArticlesForAuthor(authorRepository.GetAuthorByEmail(User.Identity.Name)?.Id ?? -1).Where(a => (int)a.Status == id).ToList();
                else if(User.IsInRole("Editor") || User.IsInRole("Admin") || User.IsInRole("SuperAdmin"))
                    model.Articles = articleRepository.GetAllArticlesByStatus(model.StatusId ?? 0).ToList();
            }
            else
            {
                if (User.IsInRole("Researcher"))
                    model.Articles = articleRepository.GetAllArticlesForAuthor(authorRepository.GetAuthorByEmail(User.Identity.Name)?.Id ?? -1).ToList();
                else if (User.IsInRole("Editor") || User.IsInRole("Admin") || User.IsInRole("SuperAdmin"))
                    model.Articles = articleRepository.GetAllArticlesByStatus(model.StatusId ?? 0).ToList();
            }

            return View("ViewReviewStatus", model);
        }

        [Authorize(Roles = "Editor, Admin, SuperAdmin")]
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
                Description = article.Description,
                Authors = article.Authors,
                AuthorFirstName = authorRepository.GetAuthor(article.contactAuthorId)?.FirstName,
                AuthorLastName = authorRepository.GetAuthor(article.contactAuthorId)?.LastName,
                AuthorEmail = authorRepository.GetAuthor(article.contactAuthorId)?.Email,
                Status = article.Status,
                MagazineVolume = article.Magazine?.Volume,
                MagazineNumber = article.Magazine?.Number,
                MagazinePublishingYear = article.Magazine?.PublishingYear,
                ExistingReviewerDecisionFileName = article.Documents
                    .Where(d => d.Type == DocumentType.ReviewerDecision)
                    .GroupBy(d => d.UserName)
                    .Select(d => d.LastOrDefault()?.DocumentPath)
                    .ToList(),
                    //.LastOrDefault(d => d.Type == DocumentType.ReviewerDecision)?.DocumentPath,
                DocumentsForArticle = article.Documents.OrderBy(d=>d.Id).ToList()
            };

            return View(articleDetailsViewModel);
        }

        [HttpPost]
        [Authorize(Roles = "Editor, Admin, SuperAdmin")]
        public IActionResult Review(ArticleDetailsViewModel model)
        {
            if (ModelState.IsValid)
            {
                Article article = articleRepository.GetArticle(model.Id);
                article.Status = model.Status;
                article.Title = model.Title;
                article.Description = model.Description;
                article.Authors = model.Authors;
                article.Magazine = magazineRepository.GetMagazineByVolumeNumberYear(model.MagazineVolume, model.MagazineNumber, model.MagazinePublishingYear);
                article.MagazineId = magazineRepository.GetMagazineByVolumeNumberYear(model.MagazineVolume, model.MagazineNumber, model.MagazinePublishingYear)?.Id;

                if (model.ReviewerDecision != null)
                {
                    var uniqueFileName = ProcessUploadedFile(model.ReviewerDecision, "reviewerDecision") ?? String.Empty;
                    if (!String.IsNullOrWhiteSpace(uniqueFileName))
                    {
                        Document newDocument = new Document
                        {
                            Type = DocumentType.ReviewerDecision,
                            DocumentPath = uniqueFileName,
                            UserName = User.Identity.Name
                        };
                        article.Documents.Add(newDocument);
                    }
                } 

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
                sendStatusChangeByByEmail(updatedArticle);

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
            title = String.Join("/", title.Split("/").Select(s => System.Net.WebUtility.UrlEncode(s)));
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, System.Web.HttpUtility.HtmlEncode(title) + fileExt);
        }

        [AllowAnonymous]
        public IActionResult GetLast5Magazines()
        {
            IEnumerable<Magazine> magazines = magazineRepository.GetLast5Magazines().Where(m => m.Published == true);
            return PartialView("_GetLast5Magazines", magazines);
        }

        [AllowAnonymous]
        public string PreviewFile(string fileName, string subfolder)
        {
            string downloadFile = Path.Combine(hostingEnvironment.WebRootPath, subfolder, fileName);
            var fileExt = Path.GetExtension(fileName);
            switch (fileExt)
            {
                case ".pdf":
                    return fileName;
                case ".doc":
                case ".docx":
                    string newFileName = Path.GetFileNameWithoutExtension(downloadFile) + "ToPdf.pdf";
                    Spire.Doc.Document document = new Spire.Doc.Document(downloadFile);
                    downloadFile = Path.Combine(hostingEnvironment.WebRootPath, subfolder, newFileName);
                    document.SaveToFile(downloadFile, Spire.Doc.FileFormat.PDF);
                    document.Close();
                    return newFileName;
                default:
                    return null;
            }
        }
    }
}