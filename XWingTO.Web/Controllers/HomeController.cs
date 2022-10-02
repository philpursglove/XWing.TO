using Microsoft.AspNetCore.Mvc;

namespace XWingTO.Web.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            if (HttpContext.User.Identity.IsAuthenticated)
            {
                return View("MyHome");
            }
            else
            {
                return View();
            }
        }
    }
}
