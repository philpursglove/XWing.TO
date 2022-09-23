using Microsoft.AspNetCore.Mvc;
using XWingTO.Web.ViewModels.Tournament;

namespace XWingTO.Web.Controllers
{
    public class Tournament : Controller
    {
        public IActionResult Search()
        {
            return View();
        }

        public IActionResult MyEvents()
        {
            return View();
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(CreateTournamentViewModel model)
        {
            if (ModelState.IsValid)
            {
                return RedirectToAction("Edit", "Tournament");
            }

            return View(model);
        }

        public IActionResult Edit()
        {
            return View();
        }
    }
}
