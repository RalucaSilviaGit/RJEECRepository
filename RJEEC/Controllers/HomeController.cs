using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System.IO;

namespace RJEEC.Controllers
{
    public class HomeController : Controller
    {
        private readonly IWebHostEnvironment hostingEnvironment;
        public HomeController(IWebHostEnvironment hostingEnvironment)
        {
            this.hostingEnvironment = hostingEnvironment;
        }
        [AllowAnonymous]
        public IActionResult Index()
        {
            return View();
        }
        [AllowAnonymous]
        [Route("EditorialBoard")]
        public IActionResult EditorialBoard()
        {
            return View();
        }
        [AllowAnonymous]
        [Route("AuthorGuidelines")]
        public IActionResult AuthorGuidelines()
        {
            return View();
        }
        [AllowAnonymous]
        [Route("PeerReviewProcess")]
        public IActionResult PeerReviewProcess()
        {
            return View();
        }

        [AllowAnonymous]
        [Route("PublishingEthics")]
        public IActionResult PublishingEthics()
        {
            return View();
        }

        [AllowAnonymous]
        [Route("PrivacyPolicy")]
        public IActionResult PrivacyPolicy()
        {
            return View();
        }

        [AllowAnonymous]
        [Route("IndexingAndAbstracting")]
        public IActionResult IndexingAndAbstracting()
        {
            return View();
        }

        [AllowAnonymous]
        [Route("CopyrightAndLicense")]
        public IActionResult CopyrightAndLicense()
        {
            return View();
        }

        [AllowAnonymous]
        [Route("JournalOpenAccessPolicy")]
        public IActionResult JournalOpenAccessPolicy()
        {
            return View();
        }

        [AllowAnonymous]
        public IActionResult DownloadFile(string fileName, string subfolder)
        {
            string downloadFile = Path.Combine(hostingEnvironment.WebRootPath, subfolder, fileName);
            byte[] fileBytes = System.IO.File.ReadAllBytes(downloadFile);
            var fileExt = Path.GetExtension(fileName);
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
        }
    }
}