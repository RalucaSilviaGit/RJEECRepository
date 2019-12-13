using Microsoft.AspNetCore.Mvc;

namespace RJEEC.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        [Route("EditorialBoard")]
        public IActionResult EditorialBoard()
        {
            return View();
        }
    }
}