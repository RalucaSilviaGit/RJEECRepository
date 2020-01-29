using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using RJEEC.Models;
using RJEEC.ViewModels;

namespace RJEEC.Controllers
{
    public class MagazineController : Controller
    {
        private readonly IMagazineRepository magazineRepository;
        private readonly IArticleRepository articleRepository;
        private readonly IHostingEnvironment hostingEnvironment;

        public MagazineController(IMagazineRepository magazineRepository,
            IArticleRepository articleRepository,
            IHostingEnvironment hostingEnvironment)
        {
            this.magazineRepository = magazineRepository;
            this.articleRepository = articleRepository;
            this.hostingEnvironment = hostingEnvironment;
        }
        public IActionResult Index()
        {
            IEnumerable<Magazine> magazines = magazineRepository.GetAllMagazines().Where(m =>m.Articles!=null && m.Articles.Count>0);
            return View(magazines);
        }

        [AllowAnonymous]
        public IActionResult GetPublishedMagazine(GetPublishedMagazineViewModel model)
        {
            if (model == null)
            {
                model = new GetPublishedMagazineViewModel();
            }

            IEnumerable<Magazine> magazines = magazineRepository.GetAllMagazines().Where(m => m.Published == true);
            model.PublishedMagazines = magazines.Select(m =>
                                  new SelectListItem()
                                  {
                                      Value = m.Id.ToString(),
                                      Text = "Vol." + m.Volume + ", No." + m.Number + ", " + m.PublishingYear
                                  }).ToList();

            if (model.MagazineId != null)
            {
                Magazine magazine = magazineRepository.GetMagazine(model.MagazineId ?? 0);
                model.Articles = magazine?.Articles;
            }           

            return View(model);
        }

        [HttpGet]
        [Authorize(Roles = "Editor, Admin, SuperAdmin")]
        public IActionResult Publish(int id)
        {
            Magazine magazine = magazineRepository.GetMagazine(id);

            if (magazine == null)
            {
                Response.StatusCode = 404;
                return View("MagazineNotFound", id);
            }

            MagazinePublishViewModel magazinePublishViewModel = new MagazinePublishViewModel
            {
                Id = magazine.Id,
                Volume = magazine.Volume,
                Number = magazine.Number,
                PublishingYear = magazine.PublishingYear,
                Published = magazine.Published,
                Articles=magazine.Articles.OrderBy(a=>a.Order).ToList(),
                ExistingCoverPath = magazine.CoverPath
            };
            return View(magazinePublishViewModel);
        }

        [HttpPost]
        [Authorize(Roles = "Editor, Admin, SuperAdmin")]
        public IActionResult Publish(MagazinePublishViewModel model)
        {
            if (ModelState.IsValid)
            {
                Magazine magazine = magazineRepository.GetMagazine(model.Id);
                magazine.Published = true;
                foreach (var article in model.Articles)
                {
                    Article art = articleRepository.GetArticle(article.Id);
                    art.Order = article.Order;
                    art.Status = ArticleStatus.Published;
                    Article updatedArticle = articleRepository.Update(art);
                }        

                if (model.Cover != null)
                {
                    if (model.ExistingCoverPath != null)
                    {
                        string filePath = Path.Combine(hostingEnvironment.WebRootPath,
                            "coverImages", model.ExistingCoverPath);
                        System.IO.File.Delete(filePath);
                    }
                    magazine.CoverPath = ProcessUploadedFile(model);
                }

                Magazine updatedMagazine = magazineRepository.UpdateMagazine(magazine);

                return RedirectToAction("publish");
            }

            return View(model);
        }

        private string ProcessUploadedFile(MagazinePublishViewModel model)
        {
            string uniqueFileName = null;

            if (model.Cover != null)
            {
                string uploadsFolder = Path.Combine(hostingEnvironment.WebRootPath, "coverImages");
                uniqueFileName = Guid.NewGuid().ToString() + "_" + model.Cover.FileName;
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    model.Cover.CopyTo(fileStream);
                }
            }

            return uniqueFileName;
        }
    }
}