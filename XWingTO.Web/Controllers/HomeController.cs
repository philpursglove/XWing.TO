using Microsoft.AspNetCore.Mvc;

namespace XWingTO.Web.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
