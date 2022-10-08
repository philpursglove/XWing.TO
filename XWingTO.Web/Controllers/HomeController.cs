using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using XWingTO.Core;
using XWingTO.Data;

namespace XWingTO.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IRepository<Tournament, Guid> _tournamentRepository;

	    public HomeController(UserManager<ApplicationUser> userManager, IRepository<Tournament, Guid> tournamentRepository)
	    {
            _userManager = userManager;
	    }

        public async Task<IActionResult> Index()
        {
            if (HttpContext.User.Identity.IsAuthenticated)
            {
	            ApplicationUser user = await _userManager.GetUserAsync(HttpContext.User);
                Guid userId = user.Id;

                var myEvents = _tournamentRepository.Query().Where(t => t.TOId == userId || t.Players.Select(p => p.PlayerId).Contains(userId));

				return View("MyHome");
            }
            else
            {
                return View();
            }
        }
    }
}
